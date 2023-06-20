// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using TileIndex = System.UInt16;

namespace CodeSmile.ProTiler3.Model
{
	/// <summary>
	///     Represents the saved data in a tilemap.
	///     Note: a TileIndex of 0 indicates an "empty" tile.
	/// </summary>
	[FullCovered]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Tile3D : IEquatable<Tile3D>
	{
		private const Tile3DFlags AllDirectionsMask = Tile3DFlags.DirectionNorth | Tile3DFlags.DirectionWest |
		                                              Tile3DFlags.DirectionSouth | Tile3DFlags.DirectionEast;

		/// <summary>
		///     Tile index used to get a tile's assed from a tile set.
		/// </summary>
		public TileIndex Index;
		/// <summary>
		///     Flags encode rotation and mirroring/flipping of a tile.
		/// </summary>
		public Tile3DFlags Flags;

		/// <summary>
		///     Checks if the tile is "empty". A TileIndex == 0 indicates an "empty" tile.
		/// </summary>
		public Boolean IsEmpty => Index == 0;

		/// <summary>
		///     Returns direction flags from Flags. Returns DirectionNorth even if flags is 0 to ensure a default rotation.
		/// </summary>
		public Tile3DFlags Direction => GetDirectionOrDefault();

		public static Boolean operator ==(Tile3D left, Tile3D right) => left.Equals(right);
		public static Boolean operator !=(Tile3D left, Tile3D right) => !left.Equals(right);

		public Tile3D(Int32 tileIndex, Tile3DFlags flags = Tile3DFlags.DirectionNorth)
			: this((TileIndex)tileIndex, flags)
		{
#if DEBUG
			if (tileIndex < 0)
				throw new ArgumentException($"tileIndex must be positive! Is: {tileIndex}");
			if (tileIndex > UInt16.MaxValue)
				throw new ArgumentException($"tileIndex out of Int16 bounds! Is: {tileIndex}");
#endif
		}

		/// <summary>
		///     Instantiates a new tile.
		/// </summary>
		/// <param name="tileIndex"></param>
		/// <param name="flags"></param>
		public Tile3D(TileIndex tileIndex, Tile3DFlags flags = Tile3DFlags.DirectionNorth)
		{
			Index = tileIndex;
			Flags = flags;
		}

		public Boolean Equals(Tile3D other) => Index == other.Index && Flags == other.Flags;
		public override Boolean Equals(Object obj) => obj is Tile3D other && Equals(other);
		public override Int32 GetHashCode() => HashCode.Combine(Index, Flags);

		[ExcludeFromCodeCoverage] public override String ToString() =>
			$"{nameof(Tile3D)}(Index: {Index}, Flags: {Flags})";

		private Tile3DFlags GetDirectionOrDefault()
		{
			var dir = Flags & AllDirectionsMask;
			return dir != Tile3DFlags.None ? dir : Tile3DFlags.DirectionNorth;
		}
	}
}
