// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
		private const int MaxDrawDistance = 100;

		[Range(MinDrawDistance, MaxDrawDistance)] [SerializeField] private int m_DrawDistance = MinDrawDistance;
		[SerializeField] private float m_VisibleRectDistance = 20f;

		private TileWorld m_World;
		private GameObject m_TileProxyPoolParent;
		private GameObject m_TileProxyPrefab;
		private ObjectPool<Tile> m_TileProxyPool;

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

			CreateTileProxyPrefabOnce();
			RecreateTileProxyPool();

			RegisterTileWorldEvents();
		}

		private void OnDisable()
		{
			UnregisterTileWorldEvents();

			DisposeTileProxyPool();
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
				StartCoroutine(WaitThenRecreateTileProxyPool());
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
		}

		private void UnregisterTileWorldEvents()
		{
			var layer = m_World.ActiveLayer;
			layer.OnClearTiles -= OnClearActiveLayer;
			layer.OnSetTile -= SetOrReplaceTile;
			layer.OnSetTiles -= SetOrReplaceTiles;
			layer.OnSetTileFlags -= SetTileFlags;
		}

		private void OnClearActiveLayer() => RecreateTileProxyPool();

		private void SetOrReplaceTiles(GridRect dirtyRect) => UpdateTileProxiesInDirtyRect(dirtyRect);

		private void SetOrReplaceTile(GridCoord coord, TileData tileData)
		{
			var dirtyRect = new GridRect(coord.ToCoord2d(), new Vector2Int(1, 1));
			UpdateTileProxiesInDirtyRect(dirtyRect);
		}

		private void SetTileFlags(GridCoord coord, TileFlags flags) => Debug.LogWarning("SetTileFlags not implemented");

		private void RecreateTileProxyPool()
		{
			var poolSize = m_DrawDistance * m_DrawDistance;
			Debug.Log($"RecreateTileProxyPool pool with {poolSize} instances");

			DisposeTileProxyPool();
			m_TileProxyPoolParent = gameObject.FindOrCreateChild("TileProxy(Pool)", Global.TileRenderHideFlags);
			if (m_TileProxyPrefab == null || m_TileProxyPrefab.IsMissing())
				throw new Exception("TileProxy prefab null or missing");

			m_TileProxyPool = new ObjectPool<Tile>(m_TileProxyPrefab, m_TileProxyPoolParent.transform, poolSize);
			if (m_TileProxyPool.InactiveInstances.Count != poolSize)
				throw new Exception("pool objects should all be initially inactive");

			m_PrevVisibleRect = new GridRect();
			m_PrevDrawDistance = m_DrawDistance;
		}

		private void DisposeTileProxyPool()
		{
			if (m_TileProxyPool != null)
			{
				m_TileProxyPool.Dispose();
				m_TileProxyPool = null;
			}
			if (m_TileProxyPoolParent != null)
			{
				m_TileProxyPoolParent.DestroyInAnyMode();
				m_TileProxyPoolParent = null;
			}
		}

		private void CreateTileProxyPrefabOnce()
		{
			if (m_TileProxyPrefab != null)
				return;

			m_TileProxyPrefab = gameObject.FindOrCreateChild("TileProxy(Prefab)", Global.TileRenderHideFlags);
			var tileProxy = m_TileProxyPrefab.GetOrAddComponent<Tile>();
			tileProxy.Layer = m_World.ActiveLayer;
		}

		private IEnumerator WaitThenRecreateTileProxyPool()
		{
			yield return null;

			RecreateTileProxyPool();
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
			UpdateTileProxies(layer, m_VisibleRect, staysUnchangedRect);
		}

		private void UpdateTileProxiesInDirtyRect(RectInt dirtyRect)
		{
			// mark visible rect as requiring update
			SetTilesInRectAsDirty(dirtyRect);
			m_VisibleRect.Intersects(dirtyRect, out var dirtyInsideVisibleRect);
			UpdateTileProxies(m_World.ActiveLayer, dirtyInsideVisibleRect, new GridRect());
		}

		private void UpdateTileProxies(TileLayer layer, GridRect dirtyRect, RectInt unchangedRect)
		{
			if (dirtyRect.width == 0 || dirtyRect.height == 0)
				return;

			// a few of them need updates
			// => compare prev and current visible rect
			// tiles in prev but not in current => reusable
			// tiles in current but not prev => must be updated

			//Debug.Log($"dirty: {dirtyRect}, unchanged: {unchangedRect}");

			// find tiles that are no longer visible
			var proxies = m_TileProxyPool.AllInstances;
			for (var i = 0; i < proxies.Count; i++)
			{
				var proxy = proxies[i];
				if (proxy == null)
					continue;
				if (proxy.gameObject.activeSelf == false)
					continue;

				if (m_VisibleRect.Contains(proxy.Coord.ToCoord2d()) == false)
					m_TileProxyPool.ReturnToPool(proxy);
			}

			m_GizmosVisibleTiles = layer.TileContainer.GetTilesInRect(dirtyRect);
			foreach (var coord in m_GizmosVisibleTiles.Keys)
			{
				if (unchangedRect.Contains(coord.ToCoord2d()))
					continue;

				var proxy = m_TileProxyPool.GetPooledObject();
				proxy.Layer = m_World.ActiveLayer;
				proxy.SetCoordAndTile(coord, m_GizmosVisibleTiles[coord]);
			}
		}

		private void SetTilesInRectAsDirty(RectInt dirtyRect)
		{
			// set tile proxies within dirty rect as inactive
			var proxies = m_TileProxyPool.AllInstances;
			for (var i = 0; i < proxies.Count; i++)
			{
				var proxy = proxies[i];
				if (proxy == null)
					continue;
				if (proxy.gameObject.activeSelf == false)
					continue;

				if (dirtyRect.Contains(proxy.Coord.ToCoord2d()))
					m_TileProxyPool.ReturnToPool(proxy);
			}
		}
	}
}