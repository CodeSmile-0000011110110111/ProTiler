// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.ProTiler
{
	public struct Tile3DCoordData
	{
		public Vector3Int Coord;
		public Tile3DData TileData;

		public Tile3DCoordData(Vector3Int coord, Tile3DData tileData)
		{
			Coord = coord;
			TileData = tileData;
		}
	}
}
