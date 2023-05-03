// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     Represents the saved data in a tilemap.
	///     Note: a TileIndex of 0 indicates an "empty" tile.
	/// </summary>
	[Serializable]
	public struct Tile3D : IEquatable<Tile3D>
	{
		public int Index;
		public Tile3DFlags Flags;

		/// <summary>
		///     Checks if the tile is "empty". A TileIndex of 0 indicates an "empty" tile.
		/// </summary>
		public bool IsEmpty => Index <= 0;
		public bool IsValid => Index >= 0;

		/// <summary>
		///     Returns direction flags from Flags. Returns DirectionNorth even if flags is 0 to ensure a default rotation.
		/// </summary>
		public Tile3DFlags Direction
		{
			get
			{
				var dir = Flags & Tile3DFlags.AllDirections;
				return dir != Tile3DFlags.None ? dir : Tile3DFlags.DirectionNorth;
			}
		}

		public static Tile3D New(int tileIndex = 0, Tile3DFlags flags = Tile3DFlags.DirectionNorth) =>
			new() { Index = tileIndex, Flags = flags };

		public static bool operator ==(Tile3D left, Tile3D right) => left.Equals(right);
		public static bool operator !=(Tile3D left, Tile3D right) => !left.Equals(right);

		public bool Equals(Tile3D other) => Index == other.Index && Flags == other.Flags;

		[ExcludeFromCodeCoverage] public override string ToString() => $"{nameof(Tile3D)}(Index: {Index}, Flags: {Flags})";

		public override bool Equals(object obj) => obj is Tile3D other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(Index, (int)Flags);
	}
}
