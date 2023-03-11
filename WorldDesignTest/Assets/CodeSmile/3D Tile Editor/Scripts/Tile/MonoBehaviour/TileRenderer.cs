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
		public class TileRenderer : MonoBehaviour
		{
			private const int MinDrawDistance = 10;
			private const int MaxDrawDistance = 100;

			[Range(MinDrawDistance, MaxDrawDistance)] [SerializeField] private int m_DrawDistance = MinDrawDistance;

			[NonSerialized] private readonly Dictionary<GridCoord, GameObject> m_ActiveObjects = new();
			[NonSerialized] private TileWorld m_World;
			[NonSerialized] private Transform m_Cursor;
			[NonSerialized] private Transform m_TilesParent;

			private int m_PrevDrawDistance;
			private GridCoord m_CursorRenderCoord;
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
				Repaint();
				RegisterTileWorldEvents();
			}

			private void OnDisable() => UnregisterTileWorldEvents();

			private void OnRenderObject()
			{
				//Debug.Log(Time.frameCount + System.Reflection.MethodBase.GetCurrentMethod().Name);

				var camera = Camera.current;
				if (camera == null)
					return;

				var layer = m_World.ActiveLayer;
				var camRect = GetCameraRect(camera);

				DestroyTilesOutside(camRect);
				InstantiateTiles(layer, camRect);
				UpdateCursorTile(layer);
			}

			private void OnValidate()
			{
				ClampDrawDistance();
				UpdateTileProxyObjects();
			}

			private void UpdateTileProxyObjects()
			{
				if (m_DrawDistance != m_PrevDrawDistance)
				{
					m_PrevDrawDistance = m_DrawDistance;
					// TODO: update go count
				}
			}

			private void ClampDrawDistance() => m_DrawDistance = math.clamp(m_DrawDistance, MinDrawDistance, MaxDrawDistance);

			public void Repaint()
			{
				m_TilesParent.DestroyAllChildren();
				OnRenderObject();
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

			private RectInt GetCameraRect(Camera camera)
			{
				var camForward = camera.transform.forward;
				var rayOrigin = camera.transform.position + camForward * (m_DrawDistance * 10f);
				var camRay = new Ray(rayOrigin, Vector3.down);
				camRay.IntersectsPlane(out float3 point);

				//var camPos = (float3)camera.transform.position;
				var camPos = point;
				var camCoord = m_World.ActiveLayer.Grid.ToGridCoord(camPos);
				var coord1 = new GridCoord(camCoord.x - m_DrawDistance, 0, camCoord.z - m_DrawDistance);
				var coord2 = new GridCoord(camCoord.x + m_DrawDistance, 0, camCoord.z + m_DrawDistance);
				return TileGrid.MakeRect(coord1, coord2);
			}

			private void SetOrReplaceTiles(GridRect rect)
			{
				var camera = Camera.current;
				if (camera == null)
					return;

				var camRect = GetCameraRect(camera);
				rect.ClampToBounds(camRect);

				DestroyTilesInside(rect);
				InstantiateTiles(m_World.ActiveLayer, rect);
			}

			private void SetTileFlags(GridCoord coord, TileFlags flags)
			{
				if (TryGetGameObjectAtCoord(coord, out var go))
					ApplyTileFlags(go, flags);
			}

			private void OnClearActiveLayer() => DestroyAllTiles();

			private bool IsGameObjectAtCoord(GridCoord coord) => m_ActiveObjects.ContainsKey(coord);
			private bool TryGetGameObjectAtCoord(GridCoord coord, out GameObject go) => m_ActiveObjects.TryGetValue(coord, out go);

			private void InstantiateTiles(TileLayer layer, GridRect rect)
			{
				if (rect.width <= 0 || rect.height <= 0)
					return;

				var tileset = layer.TileSet;
				var tileCount = layer.TileContainer.GetTilesInRect(rect, out var coords, out var tiles);
				for (var i = 0; i < tileCount; i++)
				{
					var coord = coords[i];
					if (IsGameObjectAtCoord(coord) == false)
					{
						var tile = tiles[i];
						var prefab = tileset.GetPrefab(tile.TileSetIndex);
						if (prefab != null)
						{
							var worldPos = layer.GetTileWorldPosition(coord);
							var go = InstantiateTileObject(prefab, worldPos, m_TilesParent, tile.Flags);
							m_ActiveObjects.Add(coord, go);
						}
					}
				}

				//Selection.SetActiveObjectWithContext(null, null);
			}

			private GameObject InstantiateTileObject(GameObject prefab, Vector3 position, Transform parent, TileFlags flags)
			{
				var go = Instantiate(prefab, position, quaternion.identity, parent);
				ApplyTileFlags(go, flags);
				go.hideFlags = HideFlags.HideAndDontSave;
				return go;
			}

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

			private void SetCursorPosition(TileLayer layer, GridCoord cursorCoord)
			{
				m_CursorRenderCoord = cursorCoord;
				m_Cursor.position = layer.Grid.ToWorldPosition(m_CursorRenderCoord) + layer.TileSet.GetTileOffset();
			}

			private Transform CreateChildObject(string name, GameObject prefab = null)
			{
				var child = transform.Find(name);
				if (child == null)
				{
					if (prefab == null)
					{
						child = new GameObject(name).transform;
						child.parent = transform;
					}
					else
					{
						child = Instantiate(prefab, transform).transform;
						child.name = name;
					}

					child.gameObject.hideFlags = HideFlags.HideAndDontSave;
				}
				return child;
			}

			private void ApplyTileFlags(GameObject go, TileFlags flags)
			{
				var t = go.transform;
				t.localScale = ScaleFromTileFlags(flags, t.localScale);
				t.rotation = RotationFromTileFlags(flags);
			}

			private Vector3 ScaleFromTileFlags(TileFlags flags, Vector3 scale)
			{
				if (flags.HasFlag(TileFlags.FlipHorizontal))
					scale.x *= -1f;
				if (flags.HasFlag(TileFlags.FlipVertical))
					scale.z *= -1f;

				return scale;
			}

			private Quaternion RotationFromTileFlags(TileFlags flags)
			{
				if (flags.HasFlag(TileFlags.DirectionEast))
					return Quaternion.Euler(0f, 90f, 0f);
				if (flags.HasFlag(TileFlags.DirectionSouth))
					return Quaternion.Euler(0f, 180f, 0f);
				if (flags.HasFlag(TileFlags.DirectionWest))
					return Quaternion.Euler(0f, 270f, 0f);

				return Quaternion.identity;
			}

			private void DestroyTilesOutside(GridRect rect)
			{
				var coordsToRemove = new List<GridCoord>();
				foreach (var coord in m_ActiveObjects.Keys)
				{
					if (rect.IsInside(coord) == false)
						coordsToRemove.Add(coord);
				}

				foreach (var coord in coordsToRemove)
				{
					var go = m_ActiveObjects[coord];
					if (go.IsMissing() == false)
					{
						//Debug.Log($"Remove {go.name}");
						//go.transform.localScale = new Vector3(.2f, .2f, .2f);
						go.DestroyInAnyMode();
					}

					m_ActiveObjects.Remove(coord);
				}
			}

			private void DestroyTilesInside(GridRect rect)
			{
				var coordsToRemove = new List<GridCoord>();
				foreach (var coord in m_ActiveObjects.Keys)
				{
					if (rect.IsInside(coord))
						coordsToRemove.Add(coord);
				}

				foreach (var coord in coordsToRemove)
				{
					var go = m_ActiveObjects[coord];
					if (go.IsMissing() == false)
						go.DestroyInAnyMode();

					m_ActiveObjects.Remove(coord);
				}
			}

			private void DestroyAllTiles()
			{
				Debug.Log("DestroyAllTiles ...");
				foreach (var coord in m_ActiveObjects.Keys)
				{
					var go = m_ActiveObjects[coord];
					if (go.IsMissing() == false)
						go.DestroyInAnyMode();
				}

				m_ActiveObjects.Clear();
				m_TilesParent.DestroyAllChildren();
			}
		}
	}
}