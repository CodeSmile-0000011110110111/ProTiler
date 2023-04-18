// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using System;

namespace CodeSmile.ProTiler
{
	/// <summary>
	///     Represents the saved data in a tilemap.
	///     Note: a TileIndex of 0 indicates an "empty" tile.
	/// </summary>
	[Serializable]
	public struct Tile3DData : IEquatable<Tile3DData>
	{
		public int TileIndex;
		public Tile3DFlags Flags;

		/// <summary>
		/// Checks if the tile is "empty". A TileIndex of 0 indicates an "empty" tile.
		/// </summary>
		public bool IsEmpty => TileIndex == 0;
		public bool IsValid => TileIndex >= 0;

		/// <summary>
		/// Returns direction flags from Flags. Returns DirectionNorth even if flags is 0 to ensure a default rotation.
		/// </summary>
		public Tile3DFlags Direction
		{
			get
			{
				var dir = Flags & Tile3DFlags.AllDirections;
				return dir != Tile3DFlags.None ? dir : Tile3DFlags.DirectionNorth;
			}
		}

		public static Tile3DData New(int tileIndex = 0, Tile3DFlags flags = Tile3DFlags.DirectionNorth) =>
			new() { TileIndex = tileIndex, Flags = flags };

		public override string ToString() => $"Tile({TileIndex}: {Flags})";

		public bool Equals(Tile3DData other) => TileIndex == other.TileIndex && Flags == other.Flags;

		public override bool Equals(object obj) => obj is Tile3DData other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(TileIndex, (int)Flags);

		public static bool operator ==(Tile3DData left, Tile3DData right) => left.Equals(right);
		public static bool operator !=(Tile3DData left, Tile3DData right) => !left.Equals(right);
	}
}
