// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public struct TileBrush : IEquatable<TileBrush>
	{
		private int m_TileSetIndex;
		private GridCoord m_Coord;

		public int TileSetIndex
		{
			get => m_TileSetIndex;
			set => m_TileSetIndex = value;
		}
		public bool IsClearing { get => m_TileSetIndex < 0; }
		public GridCoord Coord
		{
			get => m_Coord;
			set => m_Coord = value;
		}

		public TileBrush(GridCoord coord, int tileSetIndex)
		{
			m_TileSetIndex = tileSetIndex;
			m_Coord = coord;
		}

		public bool Equals(TileBrush other) => m_TileSetIndex == other.m_TileSetIndex && m_Coord.Equals(other.m_Coord);

		public override bool Equals(object obj) => obj is TileBrush other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(m_TileSetIndex, m_Coord);

		public static bool operator ==(TileBrush left, TileBrush right) => left.Equals(right);
		public static bool operator !=(TileBrush left, TileBrush right) => !left.Equals(right);
	}
}