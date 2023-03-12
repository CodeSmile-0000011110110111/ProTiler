// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public sealed partial class TileWorld
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
		public partial class TileRenderer : MonoBehaviour
		{
			private const int MinDrawDistance = 10;
			private const int MaxDrawDistance = 100;
			public const HideFlags RenderHideFlags = HideFlags.DontSave;

			[Range(MinDrawDistance, MaxDrawDistance)] [SerializeField] private int m_DrawDistance = MinDrawDistance;

			[NonSerialized] private readonly Dictionary<GridCoord, GameObject> m_ActiveObjects = new();
			[NonSerialized] private TileWorld m_World;
			[NonSerialized] private Transform m_TilesParent;
			[NonSerialized] private Transform m_PoolParent;

			private GameObject m_TileProxyPrefab;
			private GameObjectPool m_TileProxyPool;
			private GridRect m_PrevVisibleRect;
			[NonSerialized] private int m_PrevDrawDistance;
			private int m_SelectedTileIndex;

			private void OnEnable()
			{
				OnEnableInit();
				RegisterTileWorldEvents();
				Repaint();
			}

			private void OnDisable()
			{
				UnregisterTileWorldEvents();
				OnDisableDisposeTileProxies();
			}

			private void OnRenderObject() => DestroyAndInstantiateTiles();

			private void OnValidate() => UpdateDrawDistance();

			private void OnEnableInit()
			{
				if (m_World == null)
					m_World = GetComponent<TileWorld>();

				m_Cursor = FindOrCreateChildObject("Cursor");
				m_TilesParent = FindOrCreateChildObject("Tiles");

				CreateTileProxyPool();
			}

			private void CreateTileProxyPool()
			{
				CreateTileProxyPrefab();
				m_PoolParent = FindOrCreateChildObject("TileProxies");

				var poolSize = 10;
				Debug.Log($"INIT TileProxy pool with {poolSize} instances");
				m_TileProxyPool = new GameObjectPool(m_TileProxyPrefab, m_PoolParent, poolSize);
				UpdateDrawDistance();
				UpdateTileProxyPoolCount();
				UpdateTileProxyObjects(m_World.ActiveLayer, GetCameraRect(), true);
			}

			private void CreateTileProxyPrefab()
			{
				m_TileProxyPrefab = new GameObject("TileProxy");
				m_TileProxyPrefab.hideFlags = HideFlags.HideAndDontSave;
				m_TileProxyPrefab.transform.parent = transform;

				var tileProxy = m_TileProxyPrefab.AddComponent<TileProxy>();
				tileProxy.Layer = m_World.ActiveLayer;
				Debug.Log($"Created TileProxy prefab with layer: {tileProxy.Layer}");
			}

			private void OnDisableDisposeTileProxies()
			{
				m_TileProxyPool?.Dispose();
				m_TileProxyPool = null;
				m_TileProxyPrefab.DestroyInAnyMode();
				m_TileProxyPrefab = null;
			}

			private void UpdateDrawDistance()
			{
				m_DrawDistance = math.clamp(m_DrawDistance, MinDrawDistance, MaxDrawDistance);
				if (m_DrawDistance != m_PrevDrawDistance)
				{
					m_PrevDrawDistance = m_DrawDistance;
					UpdateTileProxyPoolCount();
				}
			}

			private void UpdateTileProxyPoolCount()
			{
				if (m_TileProxyPool != null)
				{
					var poolSize = m_DrawDistance * m_DrawDistance;
					StartCoroutine(new WaitForFramesElapsed(1, () =>
					{
						Debug.Log($"UPDATE TileProxy pool with {poolSize} instances");
						m_TileProxyPool.UpdatePoolSize(poolSize);
					}));
				}
			}

			private void DestroyAndInstantiateTiles()
			{
				if (IsCurrentCameraValid() == false)
					return;

				var layer = m_World.ActiveLayer;
				var camRect = GetCameraRect();

				DestroyTilesOutside(camRect);
				InstantiateTiles(layer, camRect);

				UpdateTileProxyObjects(layer, camRect);
				UpdateCursorTile(layer);
			}

			private void UpdateTileProxyObjects(TileLayer layer, GridRect visibleRect, bool forceUpdate = false)
			{
				if (visibleRect.Equals(m_PrevVisibleRect) || IsCurrentCameraValid() == false)
					return;

				// a few of them need updates
				// => compare prev and current visible rect
				// tiles in prev but not in current => reusable
				// tiles in current but not prev => must be updated

				var unionRect = visibleRect.Union(m_PrevVisibleRect);
				var intersects = visibleRect.Intersects(m_PrevVisibleRect, out var intersection);
				var proxies = m_TileProxyPool.GameObjects;

				// find tiles that are no longer visible
				var reusableProxies = new List<TileProxy>();
				for (var j = 0; j < proxies.Count; j++)
				{
					var proxy = proxies[j].GetComponent<TileProxy>();
					var coord = proxy.Coord;
					var coord2d = new Vector2Int(coord.x, coord.z);

					// no longer visible?
					if (visibleRect.Contains(coord2d) == false || forceUpdate)
						reusableProxies.Add(proxy);
				}

				layer.TileContainer.GetTilesInRect(visibleRect, out var tiles);
				var i = 0;
				foreach (var coord in tiles.Keys)
				{
					var coord2d = new Vector2Int(coord.x, coord.z);
					if (intersection.Contains(coord2d) && forceUpdate == false)
						continue;

					var tile = tiles[coord];
					if (i < reusableProxies.Count)
					{
						//Debug.Log($"  update tile {tile.TileSetIndex} at {coord}, re-use proxy index {i}");
						var proxy = reusableProxies[i];
						proxy.Layer = m_World.ActiveLayer;
						proxy.SetCoordAndTile(coord, tile);
					}
					else
						Debug.LogWarning($"  needed {i} proxies but only {reusableProxies.Count} available, coord: {coord}");

					i++;
				}

				if (i > reusableProxies.Count)
				{
					Debug.LogWarning($"needed: {i}, freed: {reusableProxies.Count}, vis: {visibleRect}, " +
					                 $"union: {unionRect}, isect: {intersection} ({intersects})");
				}

				m_PrevVisibleRect = visibleRect;
			}

			public void Repaint()
			{
				m_TilesParent.DestroyAllChildren();
				DestroyAndInstantiateTiles();
			}

			private void RegisterTileWorldEvents()
			{
				var layer = m_World.ActiveLayer;
				layer.OnClearTiles += OnClearActiveLayer;
				layer.OnSetTiles += SetOrReplaceTiles;
				layer.OnSetTileFlags += SetTileFlags;
			}

			private void UnregisterTileWorldEvents()
			{
				var layer = m_World.ActiveLayer;
				layer.OnClearTiles -= OnClearActiveLayer;
				layer.OnSetTiles -= SetOrReplaceTiles;
				layer.OnSetTileFlags -= SetTileFlags;
			}
		}
	}
}