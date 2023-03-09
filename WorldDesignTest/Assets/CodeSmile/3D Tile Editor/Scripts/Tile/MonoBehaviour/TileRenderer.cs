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
			private static readonly Vector2Int MinDrawDistance = new(1, 1);
			private static readonly Vector2Int MaxDrawDistance = new(byte.MaxValue + 1, byte.MaxValue + 1);

			[SerializeField] private Vector2Int m_DrawDistance = new(20, 20);

			[NonSerialized] private readonly Dictionary<GridCoord, GameObject> m_ActiveObjects = new();
			[NonSerialized] private TileWorld m_World;

			private void OnEnable()
			{
				SetWorld();
				transform.DestroyAllChildren();
				RegisterTileWorldEvents();
			}

			private void OnDisable() => UnregisterTileWorldEvents();

			private void OnRenderObject()
			{
				var camera = Camera.current;
				if (camera == null)
					return;

				var layer = m_World.ActiveLayer;
				var camRect = GetCameraRect(camera);

				DestroyTilesOutside(camRect);
				InstantiateTiles(layer, camRect);
			}

			private void OnValidate() => m_DrawDistance.Clamp(MinDrawDistance, MaxDrawDistance);

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
				var camPos = (float3)camera.transform.position;
				var camCoord = m_World.ActiveLayer.Grid.ToGridCoord(camPos);
				var coord1 = new GridCoord(camCoord.x - m_DrawDistance.x, 0, camCoord.z - m_DrawDistance.y);
				var coord2 = new GridCoord(camCoord.x + m_DrawDistance.x, 0, camCoord.z + m_DrawDistance.y);
				return GridUtil.MakeRect(coord1, coord2);
			}

			private void SetWorld()
			{
				if (m_World == null)
					m_World = GetComponent<TileWorld>();
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
							var go = Instantiate(prefab, worldPos, quaternion.identity, transform);
							ApplyTileFlags(go, tile.Flags);
							go.hideFlags = HideFlags.DontSaveInEditor |
							               HideFlags.DontSaveInBuild |
							               HideFlags.HideInInspector |
							               HideFlags.HideInHierarchy |
							               HideFlags.NotEditable |
							               HideFlags.DontUnloadUnusedAsset;
							m_ActiveObjects.Add(coord, go);
						}
					}
				}
			}

			private void ApplyTileFlags(GameObject go, TileFlags flags)
			{
				var t = go.transform;
				t.localScale = ScaleFromTileFlags(flags);
				t.rotation = RotationFromTileFlags(flags);
			}

			private Vector3 ScaleFromTileFlags(TileFlags flags)
			{
				var scale = Vector3.one;
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
				transform.DestroyAllChildren();
			}
		}
	}
}