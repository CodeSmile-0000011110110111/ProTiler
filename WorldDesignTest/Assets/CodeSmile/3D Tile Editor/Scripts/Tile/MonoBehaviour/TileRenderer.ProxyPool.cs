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
		private readonly TileCursorRenderer m_TileCursorRenderer;
		private GameObject m_TileProxyPoolParent;
		private GameObject m_TileProxyPrefab;
		private ObjectPool<TileProxy> m_TileProxyPool;

		[NonSerialized] private GridRect m_GizmosPrevVisibleRect;
		[NonSerialized] private IDictionary<GridCoord, Tile> m_GizmosVisibleTiles;

		[NonSerialized] private GridRect m_VisibleRect;
		[NonSerialized] private GridRect m_PrevVisibleRect;
		[NonSerialized] private int m_PrevDrawDistance;
		public TileCursorRenderer TileCursorRenderer => m_TileCursorRenderer;

		private void RecreateTileProxyPool()
		{
			var poolSize = m_DrawDistance * m_DrawDistance;
			Debug.Log($"INIT TileProxy pool with {poolSize} instances");

			DisposeTileProxyPool();
			m_TileProxyPoolParent = FindOrCreateGameObject("TileProxy(Pool)", Global.TileRenderHideFlags);
			m_TileProxyPool = new ObjectPool<TileProxy>(m_TileProxyPrefab, m_TileProxyPoolParent.transform, poolSize);
			if (m_TileProxyPool.InactiveInstances.Count != poolSize)
				throw new Exception("pool objects should all be initially inactive");

			m_PrevVisibleRect = m_GizmosPrevVisibleRect = new GridRect();
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
				throw new Exception($"{nameof(m_TileProxyPrefab)} already initialized!");
			if (m_World == null)
				throw new ArgumentNullException("TileWorld is null");
			if (m_World.ActiveLayer == null)
				throw new ArgumentNullException("TileWorld.ActiveLayer is null");

			m_TileProxyPrefab = FindOrCreateGameObject("TileProxy(Prefab)", Global.TileRenderHideFlags);

			var tileProxy = m_TileProxyPrefab.GetOrAddComponent<TileProxy>();
			tileProxy.Layer = m_World.ActiveLayer;
		}

		private IEnumerator WaitForEndOfFrameThenRecreateTileProxyPool()
		{
			yield return null;

			RecreateTileProxyPool();
		}

		private void DrawProxyPoolGizmos()
		{
			var gridSize = m_World.ActiveLayer.Grid.Size;
			GizmosDrawRect(m_GizmosPrevVisibleRect.ToWorldRect(gridSize), Color.yellow);
			GizmosDrawRect(m_VisibleRect.ToWorldRect(gridSize), Color.green);
			//GizmosDrawInactiveTiles(gridSize.x / 2f);
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

		private void GizmosDrawInactiveTiles(float radius)
		{
			var inactiveProxies = m_TileProxyPool.InactiveInstances;
			if (inactiveProxies != null)
			{
				foreach (var proxy in inactiveProxies)
				{
					if (proxy == null)
						continue;

					var pos = proxy.transform.position;
					Gizmos.DrawSphere(pos, radius);
					Gizmos.DrawLine(pos, pos + new Vector3(0f, radius + 5f, 0f));
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

		private void UpdateTileProxiesInVisibleRect()
		{
			var layer = m_World.ActiveLayer;
			var visibleRect = GetVisibleRect(layer);
			if (visibleRect.Equals(m_PrevVisibleRect))
				return;
			
			UpdateTileProxies(layer, visibleRect);
			m_GizmosPrevVisibleRect = m_PrevVisibleRect;
			m_PrevVisibleRect = m_VisibleRect;
		}

		private void UpdateTileProxies(TileLayer layer, GridRect visibleRect)
		{
			// a few of them need updates
			// => compare prev and current visible rect
			// tiles in prev but not in current => reusable
			// tiles in current but not prev => must be updated

			GridRect intersectRect;
			if (m_PrevVisibleRect.width == 0 || m_PrevVisibleRect.height == 0)
			{
				// first time initialization, assume all tiles visible
				m_PrevVisibleRect = visibleRect;
				intersectRect = new GridRect(); // force all tiles "visible"
			}
			else
				m_VisibleRect.Intersects(m_PrevVisibleRect, out intersectRect);

			// find tiles that are no longer visible
			var proxies = m_TileProxyPool.AllInstances;
			for (var j = 0; j < proxies.Count; j++)
			{
				var proxy = proxies[j];
				if (proxy == null || proxy.gameObject.activeSelf == false)
					continue;

				if (m_VisibleRect.Contains(proxy.Coord.ToCoord2d()) == false)
					m_TileProxyPool.ReturnToPool(proxy);
			}

			m_GizmosVisibleTiles = layer.TileContainer.GetTilesInRect(visibleRect);
			foreach (var coord in m_GizmosVisibleTiles.Keys)
			{
				if (intersectRect.Contains(coord.ToCoord2d()))
					continue;

				var tile = m_GizmosVisibleTiles[coord];
				{
					//Debug.Log($"  update tile {tile.TileSetIndex} at {coord}, re-use proxy index {i}");
					var proxy = m_TileProxyPool.GetPooledObject();
					proxy.Layer = m_World.ActiveLayer;
					proxy.SetCoordAndTile(coord, tile);
				}
			}

		}

		private void SetTilesInRectAsDirty(RectInt dirtyRect)
		{
			// two options:
			// 1) move outside visible rect bounds
			// 2) SetActive(false)
			// which is faster? does it matter?

			// set tile proxies within dirty rect as inactive
			var proxies = m_TileProxyPool.AllInstances;
			for (var i = 0; i < proxies.Count; i++)
			{
				if (proxies[i].gameObject.activeSelf == false)
					continue;

				if (dirtyRect.Contains(proxies[i].Coord.ToCoord2d()))
					m_TileProxyPool.ReturnToPool(proxies[i]);
			}
		}
	}
}