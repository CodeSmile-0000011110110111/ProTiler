// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

// TRYADDTILES: SetTile will call TryAdd, otherwise assign, rather than ContainsKey, otherwise add.

#define TRYADDTILES

using System;
using System.Collections.Generic;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed class SerializedCoordAndTile : Dictionary<GridCoord, Tile>, ISerializationCallbackReceiver
	{
		[SerializeField] private List<GridCoord> m_Keys = new();
		[SerializeField] private List<Tile> m_Values = new();

		public void OnBeforeSerialize()
		{
			m_Keys.Clear();
			m_Values.Clear();

			foreach (var kvp in this)
			{
				m_Keys.Add(kvp.Key);
				m_Values.Add(kvp.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			Clear();
			for (var i = 0; i < m_Keys.Count; i++)
				Add(m_Keys[i], m_Values[i]);

			m_Keys.Clear();
			m_Values.Clear();
		}
	}

	[Serializable]
	public sealed class TileContainer
	{
		[SerializeField] private SerializedCoordAndTile m_Tiles = new();

		public bool Contains(GridCoord coord) => m_Tiles.ContainsKey(coord);

		public void ClearTiles() => m_Tiles.Clear();

		public void ClearTile(GridCoord coord)
		{
			if (m_Tiles.ContainsKey(coord))
				m_Tiles.Remove(coord);
		}

		public int Count => m_Tiles.Count;

		//public Tile this[GridCoord coord] => GetTile(coord);
		public Tile GetTile(GridCoord coord) => m_Tiles.TryGetValue(coord, out var tile) ? tile : default;

		public IReadOnlyList<GridCoord> SetTiles(GridRect rect, int tileSetIndex)
		{
			var coords = rect.GetTileCoords();
			for (var i = 0; i < coords.Count; i++)
				SetTile(coords[i], tileSetIndex);
			return coords;
		}

		public void SetTile(GridCoord coord, int tileSetIndex)
		{
			if (tileSetIndex < 0)
				ClearTile(coord);
			else
			{
				var newTile = new Tile(tileSetIndex);
				SetTile(coord, newTile);
			}
		}

		public void SetTile(GridCoord coord, Tile tile)
		{
			if (tile == null)
				ClearTile(coord);
			else
			{
#if TRYADDTILES
				if (m_Tiles.TryAdd(coord, tile) == false)
					m_Tiles[coord] = tile;
#else
				if (m_Tiles.ContainsKey(coord))
					m_Tiles[coord] = tile;
				else
					m_Tiles.Add(coord, tile);
#endif
			}
		}

		public int GetTilesInRect(GridRect rect, out IList<GridCoord> coords, out IList<Tile> tiles)
		{
			coords = new List<GridCoord>();
			tiles = new List<Tile>();

			foreach (var coord in rect.GetTileCoords())
			{
				var tile = GetTile(coord);
				if (tile != null)
				{
					coords.Add(coord);
					tiles.Add(tile);
				}
			}

			return coords.Count;
		}

		public TileFlags SetTileFlags(GridCoord coord, TileFlags flags)
		{
			var tileFlags = TileFlags.None;
			var tile = GetTile(coord);
			if (tile != null)
			{
				tile.Flags |= flags;
				tileFlags = flags;
			}
			return tileFlags;
		}

		public TileFlags ClearTileFlags(GridCoord coord, TileFlags flags)
		{
			var tileFlags = TileFlags.None;
			var tile = GetTile(coord);
			if (tile != null)
			{
				tile.Flags &= ~flags;
				tileFlags = flags;
			}
			return tileFlags;
		}
	}
}