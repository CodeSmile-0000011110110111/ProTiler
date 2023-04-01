// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Assets;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	[Flags]
	public enum TileFlags
	{
		None = 0,
		DirectionNorth = 1 << 0,
		DirectionEast = 1 << 1,
		DirectionSouth = 1 << 2,
		DirectionWest = 1 << 3,
		FlipHorizontal = 1 << 4,
		FlipVertical = 1 << 5,

		AllDirections = DirectionNorth | DirectionSouth | DirectionEast | DirectionWest,
		AllFlips = FlipHorizontal | FlipVertical,
	}

	[Serializable]
	public struct Tile3DData
	{
		public int PrefabSetIndex;
		public TileFlags Flags;
		// TODO matrix

		// TODO remove tile ref
		public Tile3D Tile;
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
