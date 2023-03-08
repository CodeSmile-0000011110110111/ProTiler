// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

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
	public class TileWorldObjectRenderer : MonoBehaviour
	{
		[SerializeField] private Vector2Int m_DrawDistance = new(10, 10);

		private readonly Dictionary<GridCoord, GameObject> m_ActiveObjects = new();
		private TileWorld m_World;

		private void Start()
		{
			m_World = GetComponent<TileWorld>();
			m_World.ActiveLayer.OnClearLayer += OnClearActiveLayer;
			transform.DestroyAllChildren();
		}

		private void OnRenderObject()
		{
			var camera = Camera.current;
			if (camera == null)
				return;

			var layer = m_World.ActiveLayer;
			var grid = layer.Grid;
			var camPos = (float3)camera.transform.position;
			var camCoord = grid.ToGridCoord(camPos);
			var coord1 = new GridCoord(camCoord.x - m_DrawDistance.x, 0, camCoord.z - m_DrawDistance.y);
			var coord2 = new GridCoord(camCoord.x + m_DrawDistance.x, 0, camCoord.z + m_DrawDistance.y);
			var camRect = GridUtil.MakeRect(coord1, coord2);

			DestroyTilesOutOfRange(camRect);
			InstantiateTilesInRange(layer, camRect);
		}

		private void OnValidate() => m_DrawDistance.Clamp(new Vector2Int(1, 1), new Vector2Int(500, 500));

		private void OnClearActiveLayer() => DestroyAllTiles();

		private void InstantiateTilesInRange(TileLayer layer, RectInt camRect)
		{
			var tileset = layer.TileSet;
			var tilesInRect = layer.TileContainer.GetTilesInRect(camRect, out var tileCoords);
			for (var i = 0; i < tilesInRect.Count; i++)
			{
				var coord = tileCoords[i];
				if (m_ActiveObjects.ContainsKey(coord) == false)
				{
					var tile = tilesInRect[i];
					var prefab = tileset.GetPrefab(tile.TileSetIndex);
					var go = Instantiate(prefab, layer.GetTileWorldPosition(coord), quaternion.identity, transform);
					go.hideFlags = HideFlags.HideAndDontSave;
					m_ActiveObjects.Add(coord, go);
					//go.name = $"({coord}) {go.name}";
					//Debug.Log($"Add {go.name}");
				}
			}
		}

		private void DestroyTilesOutOfRange(RectInt camRect)
		{
			var coordsToRemove = new List<GridCoord>();
			foreach (var coord in m_ActiveObjects.Keys)
			{
				if (coord.x < camRect.x || coord.x >= camRect.x + camRect.width ||
				    coord.z < camRect.y || coord.z >= camRect.y + camRect.height)
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

		private void DestroyAllTiles()
		{
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