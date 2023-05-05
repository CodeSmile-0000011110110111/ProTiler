// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using UnityEngine;

namespace CodeSmile.Tests.ProTiler.Editor.Data
{
	public class Tile3DTestCaseSource
	{
		public readonly static object[] InvalidIndexes =
		{
			new object[] { int.MinValue },
			new object[] { -1 },
		};
		public readonly static object[] ValidIndexes =
		{
			new object[] { 0 },
			new object[] { 1 },
			new object[] { int.MaxValue },
		};

		public readonly static object[] EmptyIndexes =
		{
			new object[] { int.MinValue },
			new object[] { -1 },
			new object[] { 0 },
		};
		public readonly static object[] NonEmptyIndexes =
		{
			new object[] { 1 },
			new object[] { int.MaxValue },
		};

		public readonly static object[] ValidIndexesWithFlags =
		{
			new object[] { 0, Tile3DFlags.None },
			new object[] { 1, Tile3DFlags.AllDirections },
			new object[] { -1, Tile3DFlags.AllFlips },
			new object[] { int.MinValue, Tile3DFlags.AllFlips | Tile3DFlags.AllDirections },
			new object[] { int.MaxValue, Tile3DFlags.AllFlips | Tile3DFlags.AllDirections },
		};

		public readonly static object[] DirectionFlags =
		{
			new object[] { Tile3DFlags.DirectionNorth },
			new object[] { Tile3DFlags.DirectionEast },
			new object[] { Tile3DFlags.DirectionSouth },
			new object[] { Tile3DFlags.DirectionWest },
			new object[] { Tile3DFlags.AllDirections },
		};

		public readonly static object[] NonEqualTilePairs =
		{
			new object[]
			{
				new Tile3D(0, Tile3DFlags.DirectionSouth),
				new Tile3D(-1, Tile3DFlags.DirectionSouth),
			},
			new object[]
			{
				new Tile3D(13, Tile3DFlags.DirectionSouth),
				new Tile3D(17, Tile3DFlags.DirectionSouth),
			},
			new object[]
			{
				new Tile3D(1, Tile3DFlags.DirectionWest),
				new Tile3D(1, Tile3DFlags.DirectionSouth),
			},
			new object[]
			{
				new Tile3D(0, Tile3DFlags.FlipHorizontal),
				new Tile3D(0, Tile3DFlags.FlipHorizontal | Tile3DFlags.FlipVertical),
			},
		};

		public readonly static object[] EqualTilePairs =
		{
			new object[]
			{
				new Tile3D(),
				new Tile3D(),
			},
			new object[]
			{
				new Tile3D(7, Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal),
				new Tile3D(7, Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal),
			},
			new object[]
			{
				new Tile3D(0, Tile3DFlags.None),
				new Tile3D(0, Tile3DFlags.None),
			},
			new object[]
			{
				new Tile3D(-1, Tile3DFlags.None),
				new Tile3D(-1, Tile3DFlags.None),
			},
		};

		public readonly static object[] NonEqualTileCoordPairs =
		{
			new object[]
			{
				new Tile3DCoord(new Vector3Int(1, 2, 3), new Tile3D()),
				new Tile3DCoord(new Vector3Int(3, 2, 1), new Tile3D()),
			},
			new object[]
			{
				new Tile3DCoord(new Vector3Int(1, 2, 3), new Tile3D(1)),
				new Tile3DCoord(new Vector3Int(1, 2, 3), new Tile3D(2)),
			},
		};

		public readonly static object[] EqualTileCoordPairs =
		{
			new object[]
			{
				new Tile3DCoord(),
				new Tile3DCoord(),
			},
			new object[]
			{
				new Tile3DCoord(new Vector3Int(1, 1, 1), new Tile3D()),
				new Tile3DCoord(new Vector3Int(1, 1, 1), new Tile3D()),
			},
			new object[]
			{
				new Tile3DCoord(new Vector3Int(1, 2, 3), new Tile3D(-1)),
				new Tile3DCoord(new Vector3Int(1, 2, 3), new Tile3D(-1)),
			},
		};
	}
}
