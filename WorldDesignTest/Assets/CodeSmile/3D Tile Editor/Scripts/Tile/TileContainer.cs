// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using GridCoord = UnityEngine.Vector3Int;

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

		public void SetTileAt(GridCoord coord, Tile tile)
		{
			if (tile != null)
			{
				if (m_Tiles.ContainsKey(coord))
					m_Tiles[coord] = tile;
				else
					m_Tiles.Add(coord, tile);
			}
		}

		public void ClearTileAt(GridCoord coord)
		{
			if (m_Tiles.ContainsKey(coord))
				m_Tiles.Remove(coord);
		}

		public void Clear()
		{
			m_Tiles.Clear();
		}
	}
}