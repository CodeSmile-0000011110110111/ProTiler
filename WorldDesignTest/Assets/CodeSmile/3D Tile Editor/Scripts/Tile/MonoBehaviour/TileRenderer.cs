// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
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
			[NonSerialized] private Transform m_Cursor;
			[NonSerialized] private Transform m_TilesParent;
			[NonSerialized] private Transform m_PoolParent;

			private GameObjectPool m_TileProxyPool;
			private GridCoord m_CursorRenderCoord;
			private GridRect m_PrevVisibleRect;
			private int m_PrevDrawDistance;
			private int m_SelectedTileIndex;

			private void Init()
			{
				if (m_World == null)
					m_World = GetComponent<TileWorld>();

				m_Cursor = CreateChildObject("Cursor");
				m_TilesParent = CreateChildObject("Tiles");
			}

			private void OnEnable()
			{
				Init();
				InitTileProxyPool();
				Repaint();
				RegisterTileWorldEvents();
			}

			private void OnDisable()
			{
				UnregisterTileWorldEvents();
				DisposeTileProxyPool();
			}

			private void OnRenderObject() => DestroyAndInstantiateTiles();

			private void OnValidate()
			{
				ClampDrawDistance();
				UpdateTileProxyPoolCount();
			}

			private void InitTileProxyPool()
			{
				var prefab = new GameObject("TileProxy");
				try
				{
					var tileProxy = prefab.AddComponent<TileProxy>();
					tileProxy.Layer = m_World.ActiveLayer;
					tileProxy.Coord = new GridCoord(int.MinValue, 0, int.MinValue);

					var poolSize = m_DrawDistance * m_DrawDistance;
					m_PoolParent = CreateChildObject("TileProxy Pool");
					m_TileProxyPool = new GameObjectPool(prefab, m_PoolParent, poolSize);
				}
				finally
				{
					prefab.DestroyInAnyMode();
				}
			}

			private void DisposeTileProxyPool()
			{
				m_TileProxyPool.Dispose();
				m_TileProxyPool = null;
			}

			private void DestroyAndInstantiateTiles()
			{
				if (IsCameraValid(out var camera) == false)
					return;

				var layer = m_World.ActiveLayer;
				var camRect = GetCameraRect(camera);

				DestroyTilesOutside(camRect);
				InstantiateTiles(layer, camRect);

				UpdateTileProxyObjects(layer, camRect);
				UpdateCursorTile(layer);
			}

			private void SetOrReplaceTiles(GridRect rect)
			{
				if (IsCameraValid(out var camera) == false)
					return;

				var camRect = GetCameraRect(camera);
				rect.ClampToBounds(camRect);

				DestroyTilesInside(rect);
				InstantiateTiles(m_World.ActiveLayer, rect);
			}

			private void UpdateTileProxyPoolCount()
			{
				if (m_DrawDistance != m_PrevDrawDistance)
				{
					m_PrevDrawDistance = m_DrawDistance;
					var poolSize = m_DrawDistance * m_DrawDistance;
					StartCoroutine(new WaitForFramesElapsed(1, () => { m_TileProxyPool.UpdatePoolSize(poolSize); }));
				}
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

			private void OnClearActiveLayer() => DestroyAllTiles();

			private void UpdateCursorTile(TileLayer layer)
			{
				var cursorCoord = layer.CursorCoord;

				var index = layer.SelectedTileSetIndex;
				if (m_SelectedTileIndex != index)
				{
					m_SelectedTileIndex = index;
					Debug.Log($"selected tile index: {m_SelectedTileIndex}");

					m_Cursor.gameObject.DestroyInAnyMode();

					var prefab = layer.TileSet.GetPrefab(index);
					m_Cursor = CreateChildObject("Cursor", prefab);
					SetCursorPosition(layer, cursorCoord);
				}

				if (m_CursorRenderCoord.Equals(cursorCoord) == false)
				{
					SetCursorPosition(layer, cursorCoord);
					//Debug.Log($"cursor pos changed: {m_CursorRenderCoord}");
				}
			}

			private void UpdateTileProxyObjects(TileLayer layer, GridRect visibleRect)
			{
				if (visibleRect.Equals(m_PrevVisibleRect))
					return;

				// a few of them need updates
				// => compare prev and current visible rect
				// tiles in prev but not in current => reusable
				// tiles in current but not prev => must be updated

				var tileset = layer.TileSet;

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
					if (visibleRect.Contains(coord2d) == false)
						reusableProxies.Add(proxy);
				}
				
				if (reusableProxies.Count > 0)
					Debug.Log($"re-use: {reusableProxies.Count}, visible: {visibleRect}, prev: {m_PrevVisibleRect}, union: {unionRect}, intersect: {intersection} ({intersects})");

				layer.TileContainer.GetTilesInRect(visibleRect, out var tiles);
				var i = 0;
				foreach (var coord in tiles.Keys)
				{
					var coord2d = new Vector2Int(coord.x, coord.z);
					if (intersection.Contains(coord2d))
						continue;

					var tile = tiles[coord];
					Debug.Log($"  update tile {tile.TileSetIndex} at {coord}, re-use proxy index {i}");
					var proxy = reusableProxies[i++];
					proxy.Coord = coord;
					proxy.Tile = tile;
				}

				m_PrevVisibleRect = visibleRect;
			}
		}
	}
}