// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	/// <summary>
	///     Represents the saved data in a tilemap.
	///     Note: a TileIndex of 0 indicates an "empty" tile.
	/// </summary>
	[Serializable]
	public struct Tile3DData
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
	}

	[Serializable]
	public struct Tile3DCoordData
	{
		public Vector3Int Coord;
		public Tile3DData TileData;

		public static Tile3DCoordData New(Vector3Int coord, Tile3DData tileData) => new() { Coord = coord, TileData = tileData };
	}
}
