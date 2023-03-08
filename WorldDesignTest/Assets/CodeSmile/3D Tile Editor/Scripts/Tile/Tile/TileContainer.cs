// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using GridCoord = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed class CoordAndTile : SerializedDictionary<GridCoord, Tile> {}

	[Serializable]
	public sealed class TileContainer : IEnumerable<KeyValuePair<GridCoord, Tile>>
	{
		[SerializeField] private CoordAndTile m_Tiles = new();

		public IEnumerator<KeyValuePair<GridCoord, Tile>> GetEnumerator() => m_Tiles.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int Count => m_Tiles.Count;
		
		public void SetTiles(GridRect rect, Tile tile)
		{
			var coords = rect.GetTileCoords();
			for (var i = 0; i < coords.Count(); i++)
				SetTile(coords[i], tile);
		}

		public void SetTile(GridCoord coord, Tile tile)
		{
			if (m_Tiles.ContainsKey(coord))
				m_Tiles[coord] = tile;
			else
				m_Tiles.Add(coord, tile);
		}

		public Tile GetTile(GridCoord coord)
		{
			m_Tiles.TryGetValue(coord, out var tile);
			return tile;
		}

		public IReadOnlyList<Tile> GetTilesInRect(GridRect rect, out IList<GridCoord> tileCoords)
		{
			var tiles = new List<Tile>();
			
			var allCoordsInRect = rect.GetTileCoords();
			tileCoords = new List<GridCoord>();
			foreach (var coord in allCoordsInRect)
			{
				var tile = GetTile(coord);
				if (tile != null)
				{
					tiles.Add(tile);
					tileCoords.Add(coord);
				}
			}
			
			return tiles;
		}

		public void ClearTileAt(GridCoord coord)
		{
			if (m_Tiles.ContainsKey(coord))
				m_Tiles.Remove(coord);
		}

		public void Clear() => m_Tiles.Clear();
	}
}