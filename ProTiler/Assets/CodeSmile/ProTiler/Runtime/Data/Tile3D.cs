// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     Represents the saved data in a tilemap.
	///     Note: a TileIndex of 0 indicates an "empty" tile.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Tile3D : IEquatable<Tile3D>
	{
		/// <summary>
		///     Tile index used to get a tile's assed from a tile set.
		/// </summary>
		public short Index;
		/// <summary>
		///     Flags encode rotation and mirroring/flipping of a tile.
		/// </summary>
		public Tile3DFlags Flags;

		/// <summary>
		///     Checks if the tile is "empty". A TileIndex == 0 indicates an "empty" tile.
		/// </summary>
		public bool IsEmpty => Index <= 0;
		/// <summary>
		///     Checks if the tile index is valid. A TileIndex >= 0 is considered 'valid' as it can be used for indexing.
		/// </summary>
		public bool IsValid => Index >= 0;

		/// <summary>
		///     Returns direction flags from Flags. Returns DirectionNorth even if flags is 0 to ensure a default rotation.
		/// </summary>
		public Tile3DFlags Direction => GetDirectionOrDefault();

		public static bool operator ==(Tile3D left, Tile3D right) => left.Equals(right);
		public static bool operator !=(Tile3D left, Tile3D right) => !left.Equals(right);

		/// <summary>
		///     Instantiates a new tile.
		/// </summary>
		/// <param name="tileIndex"></param>
		/// <param name="flags"></param>
		public Tile3D(short tileIndex, Tile3DFlags flags = Tile3DFlags.DirectionNorth)
		{
			Index = tileIndex;
			Flags = flags;
		}

		public bool Equals(Tile3D other) => Index == other.Index && Flags == other.Flags;
		public override bool Equals(object obj) => obj is Tile3D other && Equals(other);
		public override int GetHashCode() => HashCode.Combine(Index, (int)Flags);

		[ExcludeFromCodeCoverage] public override string ToString() =>
			$"{nameof(Tile3D)}(Index: {Index}, Flags: {Flags})";

		private Tile3DFlags GetDirectionOrDefault()
		{
			var dir = Flags & Tile3DFlags.AllDirections;
			return dir != Tile3DFlags.None ? dir : Tile3DFlags.DirectionNorth;
		}
	}
}
