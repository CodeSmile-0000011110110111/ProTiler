// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Grid;
using NUnit.Framework;
using WorldPos = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Editor.ProTiler.Grid
{
	public class Grid3DUtilityTests
	{
		[Test] public void CoordToIndex2D_Zero()
		{
			Assert.That(Grid3DUtility.ToIndex2D(0, 0, 0) == 0);
			Assert.That(Grid3DUtility.ToIndex2D(0, 0, 10) == 0);
		}

		[Test] public void CoordToIndex2D()
		{
			Assert.That(Grid3DUtility.ToIndex2D(1, 2, 3) == 7);
			Assert.That(Grid3DUtility.ToIndex2D(5, 10, 3) == 35);
			Assert.That(Grid3DUtility.ToIndex2D(3, 5, 10) == 53);
		}

		[Test] public void CoordToIndex2D_Negative()
		{
			Assert.That(Grid3DUtility.ToIndex2D(-1, 0, 10) == -1);
			Assert.That(Grid3DUtility.ToIndex2D(0, -1, 10) == -10);
			Assert.That(Grid3DUtility.ToIndex2D(-1, -1, 10) == -11);
		}

		[Test] public void CoordToIndex2D_Vector3()
		{
			Assert.That(Grid3DUtility.ToIndex2D(GridCoord.zero, 0) == 0);
			Assert.That(Grid3DUtility.ToIndex2D(new GridCoord(5, 0, 10), 3) == 35);
			Assert.That(Grid3DUtility.ToIndex2D(new GridCoord(3, 0, 5), 10) == 53);
		}

		[Test] public void Index2DToCoord_Zero() =>
			Assert.That(Grid3DUtility.ToGridCoord(0, 7), Is.EqualTo(GridCoord.zero));

		[Test] public void Index2DToCoord()
		{
			Assert.That(Grid3DUtility.ToGridCoord(0, 7, 3), Is.EqualTo(new GridCoord(0, 3, 0)));
			Assert.That(Grid3DUtility.ToGridCoord(49, 7, 5), Is.EqualTo(new GridCoord(0, 5, 7)));
			Assert.That(Grid3DUtility.ToGridCoord(61, 7, 9), Is.EqualTo(new GridCoord(5, 9, 8)));
		}

		[Test] public void Index2DToCoord_Negative()
		{
			Assert.That(Grid3DUtility.ToGridCoord(-1, 7), Is.EqualTo(new GridCoord(-1, 0, 0)));
			Assert.That(Grid3DUtility.ToGridCoord(-15, 7), Is.EqualTo(new GridCoord(-1, 0, -2)));
		}

		[Test] public void WorldPositionToCoord_Zero() =>
			Assert.That(Grid3DUtility.ToGridCoord(WorldPos.zero, new CellSize(3, 4, 7)), Is.EqualTo(GridCoord.zero));

		[Test] public void WorldPositionToCoord()
		{
			var cellSize = new CellSize(3, 4, 7);
			Assert.That(Grid3DUtility.ToGridCoord(new WorldPos(2.99999f, 3.99999f, 6.99999f), cellSize),
				Is.EqualTo(new GridCoord(0, 0, 0)));
			Assert.That(Grid3DUtility.ToGridCoord(new WorldPos(3f, 4f, 7f), cellSize),
				Is.EqualTo(new GridCoord(1, 1, 1)));
		}

		[Test] public void WorldPositionToCoord_Negative()
		{
			var cellSize = new CellSize(3, 4, 7);
			Assert.That(Grid3DUtility.ToGridCoord(new WorldPos(-.1f, -4.1f, -20.99f), cellSize),
				Is.EqualTo(new GridCoord(-1, -2, -3)));
		}

		[Test] public void CoordToWorldPosition()
		{
			var cellSize = new CellSize(1, 1, 1);
			Assert.That(Grid3DUtility.ToWorldPos(GridCoord.zero, cellSize), Is.EqualTo(new WorldPos(.5f, .5f, .5f)));
		}
	}
}
