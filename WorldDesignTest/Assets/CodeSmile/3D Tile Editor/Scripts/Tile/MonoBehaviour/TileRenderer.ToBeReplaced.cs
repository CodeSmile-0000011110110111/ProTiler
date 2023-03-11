// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.Tile
{
	public sealed partial class TileWorld
	{
		public partial class TileRenderer
		{
			private void InstantiateTiles(TileLayer layer, RectInt rect)
			{
				if (rect.width <= 0 || rect.height <= 0)
					return;

				var tileset = layer.TileSet;
				var tileCount = layer.TileContainer.GetTilesInRect_old(rect, out var coords, out var tiles, out var dict);
				for (var i = 0; i < tileCount; i++)
				{
					var coord = coords[i];
					if (IsGameObjectAtCoord(coord) == false)
					{
						var tile = tiles[i];
						var prefab = tileset.GetPrefab(tile.TileSetIndex);
						if (prefab != null)
						{
							var worldPos = layer.GetTilePosition(coord);
							var go = InstantiateTileObject(prefab, worldPos, m_TilesParent, tile.Flags);
							m_ActiveObjects.Add(coord, go);
						}
					}
				}
			}

			private void DestroyTilesOutside(RectInt rect)
			{
				var coordsToRemove = new List<int3>();
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

			private void DestroyTilesInside(RectInt rect)
			{
				var coordsToRemove = new List<int3>();
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