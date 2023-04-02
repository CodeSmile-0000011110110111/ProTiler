// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	[Serializable]
	public struct Tile3DData
	{
		public int PrefabSetIndex;
		public Tile3DFlags Flags;
	}

	[Serializable]
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
