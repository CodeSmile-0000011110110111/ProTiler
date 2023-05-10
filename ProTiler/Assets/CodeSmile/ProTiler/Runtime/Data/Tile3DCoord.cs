// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     TileData for a specific coordinate.
	/// </summary>
	[Serializable]
	public struct Tile3DCoord : IEquatable<Tile3DCoord>
	{
		public GridCoord Coord;
		public Tile3D Tile;

		public static bool operator ==(Tile3DCoord left, Tile3DCoord right) => left.Equals(right);
		public static bool operator !=(Tile3DCoord left, Tile3DCoord right) => !left.Equals(right);

		public Tile3DCoord(GridCoord coord, Tile3D tile)
		{
			Coord = coord;
			Tile = tile;
		}

		public bool Equals(Tile3DCoord other) => Coord.Equals(other.Coord) && Tile.Equals(other.Tile);

		[ExcludeFromCodeCoverage] public override string ToString() => $"{Coord}, {Tile}";

		public override bool Equals(object obj) => obj is Tile3DCoord other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(Coord, Tile);
	}
}
