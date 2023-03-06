// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed class CoordAndTile : SerializedDictionary<TileGridCoord, Tile> {}

	[Serializable]
	public sealed class TileContainer : IEnumerable<KeyValuePair<TileGridCoord, Tile>>
	{
		[SerializeField] private CoordAndTile m_Tiles = new();

	
		public IEnumerator<KeyValuePair<TileGridCoord, Tile>> GetEnumerator() => m_Tiles.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void SetTileAt(TileGridCoord coord, Tile tile)
		{
			if (tile != null)
			{
				if (m_Tiles.ContainsKey(coord))
					m_Tiles[coord] = tile;
				else
					m_Tiles.Add(coord, tile);
			}
		}

		public void ClearTileAt(TileGridCoord coord)
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