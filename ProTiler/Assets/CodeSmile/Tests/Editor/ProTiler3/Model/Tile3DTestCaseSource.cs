// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Model;
using System;
using GridCoord = Unity.Mathematics.int3;

namespace CodeSmile.Tests.Editor.ProTiler3.Model
{
	public class Tile3DTestCaseSource
	{
		internal const Tile3DFlags AllDirectionsMask = Tile3DFlags.DirectionNorth | Tile3DFlags.DirectionWest |
		                                               Tile3DFlags.DirectionSouth | Tile3DFlags.DirectionEast;

		public static readonly Object[] ValidIndexes =
		{
			new Object[] { 0 },
			new Object[] { 1 },
			new Object[] { UInt16.MaxValue },
		};

		public static readonly Object[] EmptyIndexes =
		{
			new Object[] { UInt16.MinValue },
			new Object[] { 0 },
		};
		public static readonly Object[] NonEmptyIndexes =
		{
			new Object[] { 1 },
			new Object[] { UInt16.MaxValue },
		};

		public static readonly Object[] ValidIndexesWithFlags =
		{
			new Object[] { 0, Tile3DFlags.None },
			new Object[] { 1, AllDirectionsMask },
			new Object[] { 2, Tile3DFlags.FlipBoth },
			new Object[] { UInt16.MinValue, Tile3DFlags.FlipBoth | AllDirectionsMask },
			new Object[] { UInt16.MaxValue, Tile3DFlags.FlipBoth | AllDirectionsMask },
		};

		public static readonly Object[] DirectionFlags =
		{
			new Object[] { Tile3DFlags.DirectionNorth },
			new Object[] { Tile3DFlags.DirectionEast },
			new Object[] { Tile3DFlags.DirectionSouth },
			new Object[] { Tile3DFlags.DirectionWest },
			new Object[] { AllDirectionsMask },
		};

		public static readonly Object[] NonEqualTilePairs =
		{
			new Object[]
			{
				new Tile3D(0, Tile3DFlags.DirectionSouth),
				new Tile3D(1, Tile3DFlags.DirectionSouth),
			},
			new Object[]
			{
				new Tile3D(13, Tile3DFlags.DirectionSouth),
				new Tile3D(17, Tile3DFlags.DirectionSouth),
			},
			new Object[]
			{
				new Tile3D(1, Tile3DFlags.DirectionWest),
				new Tile3D(1, Tile3DFlags.DirectionSouth),
			},
			new Object[]
			{
				new Tile3D(0, Tile3DFlags.FlipHorizontal),
				new Tile3D(0, Tile3DFlags.FlipHorizontal | Tile3DFlags.FlipVertical),
			},
		};

		public static readonly Object[] EqualTilePairs =
		{
			new Object[]
			{
				new Tile3D(),
				new Tile3D(),
			},
			new Object[]
			{
				new Tile3D(7, Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal),
				new Tile3D(7, Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal),
			},
			new Object[]
			{
				new Tile3D(0, Tile3DFlags.None),
				new Tile3D(0, Tile3DFlags.None),
			},
		};

		public static readonly Object[] NonEqualTileCoordPairs =
		{
			new Object[]
			{
				new Tile3DCoord(new GridCoord(1, 2, 3), new Tile3D()),
				new Tile3DCoord(new GridCoord(3, 2, 1), new Tile3D()),
			},
			new Object[]
			{
				new Tile3DCoord(new GridCoord(1, 2, 3), new Tile3D(1)),
				new Tile3DCoord(new GridCoord(1, 2, 3), new Tile3D(2)),
			},
		};

		public static readonly Object[] EqualTileCoordPairs =
		{
			new Object[]
			{
				new Tile3DCoord(),
				new Tile3DCoord(),
			},
			new Object[]
			{
				new Tile3DCoord(new GridCoord(1, 1, 1), new Tile3D()),
				new Tile3DCoord(new GridCoord(1, 1, 1), new Tile3D()),
			},
			new Object[]
			{
				new Tile3DCoord(new GridCoord(1, 2, 3), new Tile3D(4)),
				new Tile3DCoord(new GridCoord(1, 2, 3), new Tile3D(4)),
			},
		};
	}
}
