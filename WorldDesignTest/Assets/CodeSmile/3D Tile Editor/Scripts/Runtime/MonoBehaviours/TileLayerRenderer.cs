﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	/// <summary>
	///     Draws tile world layers using game objects.
	///     Game objects within view range (configurable) are instantiated or destroyed as needed.
	///     Note: pooling will eventually be added to this.
	///     Note: no need to keep state, when loading scene, instantiate all visible game objects initially all at once
	///     How it works:
	///     For each layer, get the tileset prefabs.
	///     enumerating tiles in visible range => change to rectangular XZ view range
	///     try get tile for each coordinate from the layer TileContainer
	///     keep a collection of currently instantiated tiles (game object + coord)
	///     or: rely on object naming
	///     enumerate that collection for visibility (not necessarily every frame)
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(TileWorld))]
	public sealed partial class TileLayerRenderer : MonoBehaviour
	{
		private const int MinDrawDistance = 2;
		private const int MaxDrawDistance = 128;

		[Range(MinDrawDistance, MaxDrawDistance)] [SerializeField] private int m_DrawDistance = MinDrawDistance;
		[SerializeField] private float m_VisibleRectDistance = 5f;

		private TileWorld m_World;
		private GameObject m_TilePoolParent;
		private GameObject m_TilePrefab;
		private ObjectPool<Tile> m_TilePool;

		[NonSerialized] private IDictionary<GridCoord, TileData> m_GizmosVisibleTiles;

		[NonSerialized] private GridRect m_VisibleRect;
		[NonSerialized] private GridRect m_PrevVisibleRect;
		[NonSerialized] private int m_PrevDrawDistance;

		private void OnEnable()
		{
			m_World = GetComponent<TileWorld>();

			ClampDrawDistance();
			m_PrevDrawDistance = m_DrawDistance;
			Debug.Log($"OnEnable draw distance is: {m_DrawDistance}");

			CreateTilePrefabOnce();
			RecreateTilePool();

			RegisterTileWorldEvents();
		}

		private void OnDisable()
		{
			UnregisterTileWorldEvents();

			DisposeTilePool();
		}

		private void OnRenderObject()
		{
			if (CameraExt.IsSceneViewOrGameCamera(Camera.current) == false)
				return;

			UpdateTileProxiesInVisibleRect();
		}

		private void OnValidate()
		{
			ClampDrawDistance();
			if (m_PrevDrawDistance != m_DrawDistance)
			{
				Debug.Log($"new draw distance is: {m_DrawDistance}");
				m_PrevDrawDistance = m_DrawDistance;
				StopAllCoroutines();
				StartCoroutine(WaitThenRecreateTilePool());
			}
		}

		private void ClampDrawDistance() => m_DrawDistance = math.clamp(m_DrawDistance, MinDrawDistance, MaxDrawDistance);

		private GridRect GetVisibleRect(TileLayer layer) => layer.Grid.GetCameraRect(Camera.current, m_DrawDistance, m_VisibleRectDistance);

		private void RegisterTileWorldEvents()
		{
			var layer = m_World.ActiveLayer;
			layer.OnClearTiles += OnClearActiveLayer;
			layer.OnSetTile += SetOrReplaceTile;
			layer.OnSetTiles += SetOrReplaceTiles;
			layer.OnSetTileFlags += SetTileFlags;

#if UNITY_EDITOR
			Undo.undoRedoPerformed += UndoRedoPerformed;
#endif
		}

		private void UnregisterTileWorldEvents()
		{
			var layer = m_World.ActiveLayer;
			layer.OnClearTiles -= OnClearActiveLayer;
			layer.OnSetTile -= SetOrReplaceTile;
			layer.OnSetTiles -= SetOrReplaceTiles;
			layer.OnSetTileFlags -= SetTileFlags;

#if UNITY_EDITOR
			Undo.undoRedoPerformed -= UndoRedoPerformed;
#endif
		}

		private void UndoRedoPerformed() => RecreateTilePool();
		private void OnClearActiveLayer() => RecreateTilePool();
		private void SetOrReplaceTiles(GridRect dirtyRect) => UpdateTileProxiesInDirtyRect(dirtyRect);
		private void SetOrReplaceTile(GridCoord coord, TileData tileData)
		{
			var dirtyRect = new GridRect(coord.ToCoord2d(), new Vector2Int(1, 1));
			UpdateTileProxiesInDirtyRect(dirtyRect);
		}

		private void SetTileFlags(GridCoord coord, TileFlags flags) => Debug.LogWarning("SetTileFlags not implemented");

		private void RecreateTilePool()
		{
			var poolSize = m_DrawDistance * m_DrawDistance;
			Debug.Log($"RecreateTilePool pool with {poolSize} instances");

			DisposeTilePool();
			m_TilePoolParent = gameObject.FindOrCreateChild("TilePool", Global.TileRenderHideFlags);
			if (m_TilePrefab == null || m_TilePrefab.IsMissing())
				throw new Exception("Tile prefab null or missing");

			m_TilePool = new ObjectPool<Tile>(m_TilePrefab, m_TilePoolParent.transform, poolSize);
			if (m_TilePool.InactiveInstances.Count != poolSize)
				throw new Exception("pool objects should all be initially inactive");

			m_PrevVisibleRect = new GridRect();
			m_PrevDrawDistance = m_DrawDistance;
		}

		private void DisposeTilePool()
		{
			if (m_TilePool != null)
			{
				m_TilePool.Dispose();
				m_TilePool = null;
			}
			if (m_TilePoolParent != null)
			{
				m_TilePoolParent.DestroyInAnyMode();
				m_TilePoolParent = null;
			}
		}

		private void CreateTilePrefabOnce()
		{
			if (m_TilePrefab != null)
				return;

			m_TilePrefab = gameObject.FindOrCreateChild("TilePrefab", Global.TileRenderHideFlags);
			var tile = m_TilePrefab.GetOrAddComponent<Tile>();
			tile.Layer = m_World.ActiveLayer;
		}

		private IEnumerator WaitThenRecreateTilePool()
		{
			yield return null;

			RecreateTilePool();
			m_VisibleRect = new GridRect();
			UpdateTileProxiesInVisibleRect();
		}

		private void UpdateTileProxiesInVisibleRect()
		{
			if (CameraExt.IsSceneViewOrGameCamera(Camera.current) == false)
				return;

			var layer = m_World.ActiveLayer;
			var visibleRect = GetVisibleRect(layer);
			if (visibleRect.Equals(m_VisibleRect))
				return;

			m_PrevVisibleRect = m_VisibleRect;
			m_VisibleRect = visibleRect;
			m_VisibleRect.Intersects(m_PrevVisibleRect, out var staysUnchangedRect);
			UpdateVisibleTiles(layer, m_VisibleRect, staysUnchangedRect);
		}

		private void UpdateTileProxiesInDirtyRect(RectInt dirtyRect)
		{
			// mark visible rect as requiring update
			SetTilesInRectAsDirty(dirtyRect);
			m_VisibleRect.Intersects(dirtyRect, out var dirtyInsideVisibleRect);
			UpdateVisibleTiles(m_World.ActiveLayer, dirtyInsideVisibleRect, new GridRect());
		}

		private void UpdateVisibleTiles(TileLayer layer, GridRect dirtyRect, RectInt unchangedRect)
		{
			if (dirtyRect.width == 0 || dirtyRect.height == 0)
				return;

			// a few of them need updates
			// => compare prev and current visible rect
			// tiles in prev but not in current => reusable
			// tiles in current but not prev => must be updated

			//Debug.Log($"dirty: {dirtyRect}, unchanged: {unchangedRect}");

			// find tiles that are no longer visible
			var tiles = m_TilePool.AllInstances;
			for (var i = 0; i < tiles.Count; i++)
			{
				var tile = tiles[i];
				if (tile == null)
					continue;
				if (tile.gameObject.activeSelf == false)
					continue;

				if (m_VisibleRect.Contains(tile.Coord.ToCoord2d()) == false)
					m_TilePool.ReturnToPool(tile);
			}

			m_GizmosVisibleTiles = layer.TileDataContainer.GetTilesInRect(dirtyRect);
			foreach (var coord in m_GizmosVisibleTiles.Keys)
			{
				if (unchangedRect.Contains(coord.ToCoord2d()))
					continue;

				var tile = m_TilePool.GetPooledObject();
				tile.Layer = m_World.ActiveLayer;
				tile.SetCoordAndTile(coord, m_GizmosVisibleTiles[coord]);
			}
		}

		private void SetTilesInRectAsDirty(RectInt dirtyRect)
		{
			// set tile proxies within dirty rect as inactive
			var tiles = m_TilePool.AllInstances;
			for (var i = 0; i < tiles.Count; i++)
			{
				var tile = tiles[i];
				if (tile == null)
					continue;
				if (tile.gameObject.activeSelf == false)
					continue;

				if (dirtyRect.Contains(tile.Coord.ToCoord2d()))
					m_TilePool.ReturnToPool(tile);
			}
		}
	}
}