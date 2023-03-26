﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

// TRYADDTILES: SetTile will call TryAdd, otherwise assign, rather than ContainsKey, otherwise add.

#define TRYADDTILES

using CodeSmile.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;

namespace CodeSmile.ProTiler
{
	[Serializable]
	public sealed class TileDataContainer
	{
		[SerializeField] private TileDataDictionary m_TilesDataDictionary = new();

		public int Count => m_TilesDataDictionary.Count;

		public bool Contains(GridCoord coord) => m_TilesDataDictionary.ContainsKey(coord);

		public void ClearAllTiles() => m_TilesDataDictionary.Clear();

		//public TileData this[GridCoord coord] => GetTile(coord);
		public TileData GetTile(GridCoord coord) => m_TilesDataDictionary.TryGetValue(coord, out var tile) ? tile : Global.InvalidTileData;

		public (IReadOnlyList<GridCoord>, IReadOnlyList<TileData>) SetTileIndexes(GridRect rect, int tileSetIndex) =>
			SetTileIndexes(rect.GetTileCoords(), tileSetIndex);

		public (IReadOnlyList<GridCoord>, IReadOnlyList<TileData>) SetTileIndexes(IReadOnlyList<GridCoord> coords, int tileSetIndex)
		{
			var tile = new TileData(tileSetIndex);
			var tiles = new List<TileData>();
			if (tile.IsInvalid)
			{
				for (var i = 0; i < coords.Count; i++)
				{
					TryRemoveTile(coords[i]);
					tiles.Add(tile);
				}
			}
			else
			{
				for (var i = 0; i < coords.Count; i++)
				{
					AddOrUpdateTile(coords[i], tile);
					tiles.Add(tile);
				}
			}

			return (coords, tiles);
		}

		public void ClearTile(GridCoord coord) => TryRemoveTile(coord);

		public void SetTile(GridCoord coord, TileData tileData) => AddOrUpdateTile(coord, tileData);

		private void AddOrUpdateTile(GridCoord coord, in TileData tileData)
		{
#if TRYADDTILES
			if (m_TilesDataDictionary.TryAdd(coord, tileData) == false)
				UpdateTile(coord, tileData);
#else
				if (m_Tiles.ContainsKey(coord))
					m_Tiles[coord] = tile;
				else
					m_Tiles.Add(coord, tile);
#endif
		}

		private void UpdateTile(GridCoord coord, TileData tileData) => m_TilesDataDictionary[coord] = tileData;

		private void TryRemoveTile(GridCoord coord)
		{
			if (m_TilesDataDictionary.ContainsKey(coord))
				m_TilesDataDictionary.Remove(coord);
		}

		public IDictionary<GridCoord, TileData> GetTilesInRect(GridRect rect)
		{
			var dict = new Dictionary<GridCoord, TileData>();
			foreach (var coord in rect.GetTileCoords())
			{
				var tile = GetTile(coord);
				if (tile.IsInvalid)
					continue;

				dict.Add(coord, tile);
			}
			return dict;
		}

		public TileFlags SetTileFlags(GridCoord coord, TileFlags flags)
		{
			var tile = GetTile(coord);
			if (tile.IsInvalid)
				return TileFlags.None;

			var newFlags = tile.SetFlags(flags);
			UpdateTile(coord, tile);
			return newFlags;
		}

		public TileFlags ClearTileFlags(GridCoord coord, TileFlags flags)
		{
			var tile = GetTile(coord);
			if (tile.IsInvalid)
				return TileFlags.None;

			var newFlags = tile.ClearFlags(flags);
			UpdateTile(coord, tile);
			return newFlags;
		}

		public TileFlags RotateTile(GridCoord coord, int delta)
		{
			var tile = GetTile(coord);
			if (tile.IsInvalid)
				return TileFlags.None;

			var newFlags = tile.Rotate(delta);
			UpdateTile(coord, tile);
			return newFlags;
		}

		public TileFlags FlipTile(GridCoord coord, int delta)
		{
			var tile = GetTile(coord);
			if (tile.IsInvalid)
				return TileFlags.None;

			var newFlags = tile.Flip(delta);
			UpdateTile(coord, tile);
			return newFlags;
		}
	}
}