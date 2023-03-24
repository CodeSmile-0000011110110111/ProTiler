// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
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
	[RequireComponent(typeof(TileLayer))]
	public sealed partial class TileLayerRenderer : MonoBehaviour
	{
		private const int MinDrawDistance = 2;
		private const int MaxDrawDistance = 128;

		[Range(MinDrawDistance, MaxDrawDistance)]
		[SerializeField] private int m_DrawDistance = 20;
		[SerializeField] private float m_VisibleRectDistance = 5f;

		private TileLayer m_Layer;
		private GameObject m_TilePoolParent;
		private GameObject m_TilePrefab;
		private ComponentPool<TileDataProxy> m_TilePool;

		[NonSerialized] private IDictionary<GridCoord, TileData> m_GizmosVisibleTiles;
		[NonSerialized] private GridRect m_VisibleRect;
		[NonSerialized] private GridRect m_PrevVisibleRect;
		[NonSerialized] private int m_PrevDrawDistance;

		private void OnEnable()
		{
			m_Layer = GetComponent<TileLayer>();
			if (m_Layer == null)
				throw new NullReferenceException("layer is null");

			ClampDrawDistance();
			m_PrevDrawDistance = m_DrawDistance;

			CreateTilePrefabOnce();
			RecreateTilePool();
			ForceUpdateTilesInVisibleRect();

#if UNITY_EDITOR
			Undo.undoRedoPerformed -= DelayedForceRepaint;
			Undo.undoRedoPerformed += DelayedForceRepaint;
#endif
		}

		private void OnDisable()
		{
#if UNITY_EDITOR
			Undo.undoRedoPerformed -= DelayedForceRepaint;
#endif

			DisposeTilePool();
		}

		private void OnRenderObject()
		{
			if (CameraExt.IsSceneViewOrGameCamera(Camera.current) == false)
				return;

			UpdateTilesInVisibleRect();
		}

		private void OnValidate()
		{
			ClampDrawDistance();
			if (m_PrevDrawDistance != m_DrawDistance)
			{
				m_PrevDrawDistance = m_DrawDistance;
				DelayedForceRepaint();
			}
		}

		private void DelayedForceRepaint()
		{
			if (isActiveAndEnabled)
			{
				StopAllCoroutines();
				StartCoroutine(new WaitForFramesElapsed(1, () => ForceRedraw()));
			}
		}

		private void ClampDrawDistance() => m_DrawDistance = math.clamp(m_DrawDistance, MinDrawDistance, MaxDrawDistance);

		private GridRect GetVisibleRect(TileLayer layer) => layer.Grid.GetCameraRect(Camera.current, m_DrawDistance, m_VisibleRectDistance);

		private void RecreateTilePool()
		{
			var poolSize = m_DrawDistance * m_DrawDistance;
			//Debug.Log($"RecreateTilePool pool with {poolSize} instances");

			DisposeTilePool();
			m_TilePoolParent = gameObject.FindOrCreateChild("TilePool", Global.TileHideFlags);
			if (m_TilePrefab == null || m_TilePrefab.IsMissing())
				throw new Exception("Tile prefab null or missing");

			m_TilePool = new ComponentPool<TileDataProxy>(m_TilePrefab, m_TilePoolParent.transform, poolSize);

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

			m_TilePrefab = gameObject.FindOrCreateChild("TilePrefab", Global.TileHideFlags);
			var tile = m_TilePrefab.GetOrAddComponent<TileDataProxy>();

			if (m_Layer == null)
				throw new NullReferenceException();

			tile.Layer = m_Layer;
		}

		private void ForceUpdateTilesInVisibleRect()
		{
			m_VisibleRect = new GridRect();
			UpdateTilesInVisibleRect();
		}

		private void UpdateTilesInVisibleRect()
		{
			if (CameraExt.IsSceneViewOrGameCamera(Camera.current) == false)
				return;

			var visibleRect = GetVisibleRect(m_Layer);
			if (visibleRect.Equals(m_VisibleRect))
				return;

			m_PrevVisibleRect = m_VisibleRect;
			m_VisibleRect = visibleRect;
			m_VisibleRect.Intersects(m_PrevVisibleRect, out var staysUnchangedRect);
			UpdateVisibleTiles(m_Layer, m_VisibleRect, staysUnchangedRect);
		}

		private void UpdateTilesInDirtyRect(RectInt dirtyRect)
		{
			// mark visible rect as requiring update
			SetTilesInRectAsDirty(dirtyRect);
			m_VisibleRect.Intersects(dirtyRect, out var dirtyInsideVisibleRect);
			UpdateVisibleTiles(m_Layer, dirtyInsideVisibleRect, new GridRect());
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

			m_GizmosVisibleTiles = layer.GetTilesInRect(dirtyRect);
			foreach (var coord in m_GizmosVisibleTiles.Keys)
			{
				if (unchangedRect.Contains(coord.ToCoord2d()))
					continue;

				var tile = m_TilePool.GetPooledObject();
				tile.Layer = m_Layer;
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

		internal void ForceRedraw()
		{
			RecreateTilePool();
			ForceUpdateTilesInVisibleRect();
		}

		internal void OnSetTilesInRect(GridRect dirtyRect) => UpdateTilesInDirtyRect(dirtyRect);

		internal void RedrawTiles(IReadOnlyList<GridCoord> coords, IReadOnlyList<TileData> tiles)
		{
			for (var i = 0; i < coords.Count; i++)
				OnSetTile(coords[i], tiles[i]);
		}

		internal void OnSetTile(GridCoord coord, TileData tileData)
		{
			// FIXME: access and change that tile instance directly
			var dirtyRect = new GridRect(coord.ToCoord2d(), new Vector2Int(1, 1));
			UpdateTilesInDirtyRect(dirtyRect);
		}

		internal void UpdateTileFlagsAndRedraw(GridCoord coord, TileFlags flags)
		{
			//Debug.LogWarning("SetTileFlags not implemented");
			var dirtyRect = new GridRect(coord.ToCoord2d(), new Vector2Int(1, 1));
			UpdateTilesInDirtyRect(dirtyRect);
		}
	}
}