// Copyright (C) 2021-2023 Steffen Itterheim
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
	public partial class TileRenderer
	{
		private GameObject m_TileProxyPoolParent;
		private GameObject m_TileProxyPrefab;
		private GameObjectPool m_TileProxyPool;

		[NonSerialized] private GridRect m_GizmosPrevVisibleRect;
		[NonSerialized] private List<TileProxy> m_GizmosReusableProxies;
		[NonSerialized] private IDictionary<GridCoord, Tile> m_GizmosVisibleTiles;

		private void RecreateTileProxyPool()
		{
			var poolSize = m_DrawDistance * m_DrawDistance;
			Debug.Log($"INIT TileProxy pool with {poolSize} instances");

			DisposeTileProxyPool();
			m_TileProxyPoolParent = FindOrCreateGameObject("TileProxy(Pool)", m_PersistentObjectHideFlags);

			m_TileProxyPool = new GameObjectPool(m_TileProxyPrefab, m_TileProxyPoolParent.transform, poolSize);
			InitializeTileProxyObjects(m_World.ActiveLayer, GetCameraRect());
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
				throw new Exception($"{nameof(m_TileProxyPrefab)} already initialized!");
			if (m_World == null)
				throw new ArgumentNullException("TileWorld is null");
			if (m_World.ActiveLayer == null)
				throw new ArgumentNullException("TileWorld.ActiveLayer is null");

			m_TileProxyPrefab = FindOrCreateGameObject("TileProxy(Prefab)", m_PersistentObjectHideFlags);

			var tileProxy = m_TileProxyPrefab.GetOrAddComponent<TileProxy>();
			tileProxy.Layer = m_World.ActiveLayer;
		}

		private IEnumerator WaitForEndOfFrameThenRecreateTileProxyPool()
		{
			yield return null;

			ClampDrawDistance();
			RecreateTileProxyPool();
		}

		private void DrawProxyPoolGizmos()
		{
			var gridSize = m_World.ActiveLayer.Grid.Size;
			GizmosDrawRect(m_GizmosPrevVisibleRect.ToWorldRect(gridSize), Color.yellow);
			GizmosDrawRect(m_VisibleRect.ToWorldRect(gridSize), Color.green);
			//GizmosDrawLastReusedTiles(gridSize.x / 2f);
			//GizmosDrawVisibleTiles();
		}

		private void GizmosDrawVisibleTiles()
		{
			var tileset = m_World.ActiveLayer.TileSet;
			var grid = m_World.ActiveLayer.Grid;
			if (m_GizmosVisibleTiles != null)
			{
				foreach (var coord in m_GizmosVisibleTiles.Keys)
				{
					var tile = m_GizmosVisibleTiles[coord];
					var pos = grid.ToWorldPosition(coord);
					Handles.Label(pos + new float3(0f, 3f, 0f), tile.TileSetIndex.ToString());
				}
			}
		}

		private void GizmosDrawLastReusedTiles(float radius)
		{
			if (m_GizmosReusableProxies != null)
			{
				foreach (var proxy in m_GizmosReusableProxies)
				{
					if (proxy == null)
						continue;

					var pos = proxy.transform.position;
					Gizmos.DrawSphere(pos, radius);
					Gizmos.DrawLine(pos, pos + new Vector3(0f, radius + 0.5f, 0f));
				}
			}
		}

		private void GizmosDrawRect(Rect rect, Color color)
		{
			var prevColor = Gizmos.color;
			Gizmos.color = color;
			Gizmos.DrawWireCube(new Vector3(rect.center.x, 0f, rect.center.y),
				new Vector3(rect.size.x, 1f, rect.size.y));
			Gizmos.color = prevColor;
		}

		private void InitializeTileProxyObjects(TileLayer layer, RectInt visibleRect)
		{
			if (IsCurrentCameraValid() == false)
				return;

			var proxies = m_TileProxyPool.GameObjects;
			var tiles = layer.TileContainer.GetTilesInRect(visibleRect);
			if (proxies.Count < tiles.Keys.Count)
				throw new Exception($"more Tiles ({tiles.Keys.Count}) than TileProxy ({proxies.Count}) instances!");

			var i = 0;
			foreach (var coord in tiles.Keys)
			{
				var tile = tiles[coord];
				var proxy = proxies[i].GetComponent<TileProxy>();
				proxy.Layer = m_World.ActiveLayer;
				proxy.gameObject.SetActive(true);
				proxy.SetCoordAndTile(coord, tile);
				i++;
			}

			m_PrevVisibleRect = m_GizmosPrevVisibleRect = visibleRect;
			m_PrevDrawDistance = m_DrawDistance;
		}

		private void UpdateTileProxyObjects(TileLayer layer)
		{
			if (m_VisibleRect.Equals(m_PrevVisibleRect) || IsCurrentCameraValid() == false)
				return;

			// a few of them need updates
			// => compare prev and current visible rect
			// tiles in prev but not in current => reusable
			// tiles in current but not prev => must be updated

			var unionRect = m_VisibleRect.Union(m_PrevVisibleRect);
			var intersects = m_VisibleRect.Intersects(m_PrevVisibleRect, out var intersection);
			var proxies = m_TileProxyPool.GameObjects;

			// find tiles that are no longer visible
			m_GizmosReusableProxies = new List<TileProxy>();
			for (var j = 0; j < proxies.Count; j++)
			{
				var proxy = proxies[j].GetComponent<TileProxy>();

				var coord = proxy.Coord;
				var coord2d = new Vector2Int(coord.x, coord.z);
				if (m_VisibleRect.Contains(coord2d) == false || proxy.gameObject.activeSelf == false)
				{
					proxy.gameObject.SetActive(false);
					m_GizmosReusableProxies.Add(proxy);
				}
			}

			m_GizmosVisibleTiles = layer.TileContainer.GetTilesInRect(m_VisibleRect);
			var i = 0;
			foreach (var coord in m_GizmosVisibleTiles.Keys)
			{
				var coord2d = new Vector2Int(coord.x, coord.z);
				if (intersection.Contains(coord2d))
					continue;

				var tile = m_GizmosVisibleTiles[coord];
				if (i < m_GizmosReusableProxies.Count)
				{
					//Debug.Log($"  update tile {tile.TileSetIndex} at {coord}, re-use proxy index {i}");
					var proxy = m_GizmosReusableProxies[i];
					proxy.Layer = m_World.ActiveLayer;
					proxy.gameObject.SetActive(true);
					proxy.SetCoordAndTile(coord, tile);
				}

				i++;
			}

			if (i > m_GizmosReusableProxies.Count)
			{
				Debug.LogWarning($"tile proxies count mismatch - needed: {i}, freed: {m_GizmosReusableProxies.Count}," +
				                 $" vis: {m_VisibleRect}, union: {unionRect}, isect: {intersection} ({intersects})");
			}

			m_GizmosPrevVisibleRect = m_PrevVisibleRect;
			m_PrevVisibleRect = m_VisibleRect;
		}
	}
}