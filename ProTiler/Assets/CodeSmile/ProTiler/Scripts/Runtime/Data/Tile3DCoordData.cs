// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	/// <summary>
	/// TileData for a specific coordinate.
	/// </summary>
	[Serializable]
	public struct Tile3DCoordData
	{
		public Vector3Int Coord;
		public Tile3DData TileData;

		public static Tile3DCoordData New(Vector3Int coord, Tile3DData tileData) => new() { Coord = coord, TileData = tileData };
	}
}
