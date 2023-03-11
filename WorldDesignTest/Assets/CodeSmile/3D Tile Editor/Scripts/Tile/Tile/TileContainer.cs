// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

// TRYADDTILES: SetTile will call TryAdd, otherwise assign, rather than ContainsKey, otherwise add.

#define TRYADDTILES

using System;
using System.Collections.Generic;
using Unity.Mathematics;
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

		public int GetTilesInRect_old(GridRect rect, out IList<GridCoord> coords, out IList<Tile> tiles, out IDictionary<GridCoord, Tile> dict)
		{
			coords = new List<GridCoord>();
			tiles = new List<Tile>();
			dict = new Dictionary<GridCoord, Tile>();

			foreach (var coord in rect.GetTileCoords())
			{
				var tile = GetTile(coord);
				if (tile != null)
				{
					dict.Add(coord, tile);
					coords.Add(coord);
					tiles.Add(tile);
				}
			}

			return coords.Count;
		}
		public void GetTilesInRect(GridRect rect, out IDictionary<GridCoord, Tile> dict)
		{
			dict = new Dictionary<GridCoord, Tile>();

			foreach (var coord in rect.GetTileCoords())
			{
				var tile = GetTile(coord);
				if (tile != null)
				{
					dict.Add(coord, tile);
				}
			}
		}
		public void GetTilesInUnionRect(GridRect rect1, GridRect rect2, out IDictionary<GridCoord, Tile> coordsAndTiles)
		{
			coordsAndTiles = new Dictionary<GridCoord, Tile>();
			var unionRect = rect1.Union(rect2);

			foreach (var coord in unionRect.GetTileCoords())
			{
				var tile = GetTile(coord);
				if (tile != null)
				{
					coordsAndTiles.Add(coord, tile);
				}
			}
		}
		/*
public void GetTilesInRect2(GridRect rect1, GridRect rect2, out IDictionary<GridCoord, Tile> onlyInRect1, out IDictionary<GridCoord, Tile> onlyInRect2)
{
	onlyInRect1 = onlyInRect2 = null;
	if (rect1.Equals(rect2))
		return;

	onlyInRect1 = new Dictionary<GridCoord, Tile>();
	onlyInRect2 = new Dictionary<GridCoord, Tile>();

	var unionRect = rect1.Union(rect2);
	foreach (var coord in unionRect.GetTileCoords())
	{
		var tile = GetTile(coord);
		if (tile != null)
		{
			dict.Add(coord, tile);
			coords.Add(coord);
			tiles.Add(tile);
		}
	}
}*/

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