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
	public sealed class SerializedCoordAndTile : Dictionary<GridCoord, TileData>, ISerializationCallbackReceiver
	{
		[SerializeField] private List<GridCoord> m_Keys = new();
		[SerializeField] private List<TileData> m_Values = new();

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
	public sealed class TileDataContainer
	{
		[SerializeField] private SerializedCoordAndTile m_Tiles = new();

		public int Count { get => m_Tiles.Count; }

		public bool Contains(GridCoord coord) => m_Tiles.ContainsKey(coord);

		public void ClearTiles() => m_Tiles.Clear();

		public void ClearTile(GridCoord coord)
		{
			if (m_Tiles.ContainsKey(coord))
				m_Tiles.Remove(coord);
		}

		//public Tile this[GridCoord coord] => GetTile(coord);
		public TileData GetTile(GridCoord coord) => m_Tiles.TryGetValue(coord, out var tile) ? tile : Global.InvalidTileData;

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
				SetTile(coord, new TileData(tileSetIndex));
		}

		public void SetTile(GridCoord coord, TileData tileData)
		{
			if (tileData.TileSetIndex < 0)
				ClearTile(coord);
			else
			{
#if TRYADDTILES
				if (m_Tiles.TryAdd(coord, tileData) == false)
					m_Tiles[coord] = tileData;
#else
				if (m_Tiles.ContainsKey(coord))
					m_Tiles[coord] = tile;
				else
					m_Tiles.Add(coord, tile);
#endif
			}
		}

		public IDictionary<GridCoord, TileData> GetTilesInRect(GridRect rect)
		{
			var dict = new Dictionary<GridCoord, TileData>();
			foreach (var coord in rect.GetTileCoords())
			{
				var tile = GetTile(coord);
				if (tile.TileSetIndex < 0)
					continue;

				dict.Add(coord, tile);
			}
			return dict;
		}

		public void GetTilesInUnionRect(GridRect rect1, GridRect rect2, out IDictionary<GridCoord, TileData> coordsAndTiles)
		{
			coordsAndTiles = new Dictionary<GridCoord, TileData>();
			var unionRect = rect1.Union(rect2);

			foreach (var coord in unionRect.GetTileCoords())
			{
				var tile = GetTile(coord);
				if (tile.TileSetIndex < 0)
					continue;

				coordsAndTiles.Add(coord, tile);
			}
		}

		public TileFlags SetTileFlags(GridCoord coord, TileFlags flags)
		{
			var tile = GetTile(coord);
			if (tile.TileSetIndex < 0)
				return TileFlags.None;

			tile.Flags |= flags;
			return tile.Flags;
		}

		public TileFlags ClearTileFlags(GridCoord coord, TileFlags flags)
		{
			var tile = GetTile(coord);
			if (tile.TileSetIndex < 0)
				return TileFlags.None;

			tile.Flags &= ~flags;
			return tile.Flags;
		}
	}
}