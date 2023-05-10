// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.ProTiler.Editor.Data
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
			Assert.That(Grid3DUtility.ToIndex2D(Vector3Int.zero, 0) == 0);
			Assert.That(Grid3DUtility.ToIndex2D(new Vector3Int(5, 0, 10), 3) == 35);
			Assert.That(Grid3DUtility.ToIndex2D(new Vector3Int(3, 0, 5), 10) == 53);
		}

		[Test] public void Index2DToCoord_Zero() => Assert.That(Grid3DUtility.ToCoord(0, 7), Is.EqualTo(Vector3Int.zero));

		[Test] public void Index2DToCoord()
		{
			Assert.That(Grid3DUtility.ToCoord(0, 7, 3), Is.EqualTo(new Vector3Int(0, 3, 0)));
			Assert.That(Grid3DUtility.ToCoord(49, 7, 5), Is.EqualTo(new Vector3Int(0, 5, 7)));
			Assert.That(Grid3DUtility.ToCoord(61, 7, 9), Is.EqualTo(new Vector3Int(5, 9, 8)));
		}

		[Test] public void Index2DToCoord_Negative()
		{
			Assert.That(Grid3DUtility.ToCoord(-1, 7), Is.EqualTo(new Vector3Int(-1, 0, 0)));
			Assert.That(Grid3DUtility.ToCoord(-15, 7), Is.EqualTo(new Vector3Int(-1, 0, -2)));
		}

		[Test] public void WorldPositionToCoord_Zero() =>
			Assert.That(Grid3DUtility.ToCoord(Vector3.zero, new Vector3Int(3, 4, 7)), Is.EqualTo(Vector3Int.zero));

		[Test] public void WorldPositionToCoord()
		{
			var cellSize = new Vector3Int(3, 4, 7);
			Assert.That(Grid3DUtility.ToCoord(new Vector3(2.99999f, 3.99999f, 6.99999f), cellSize), Is.EqualTo(new Vector3Int(0, 0, 0)));
			Assert.That(Grid3DUtility.ToCoord(new Vector3(3f, 4f, 7f), cellSize), Is.EqualTo(new Vector3Int(1, 1, 1)));
		}

		[Test] public void WorldPositionToCoord_Negative()
		{
			var cellSize = new Vector3Int(3, 4, 7);
			Assert.That(Grid3DUtility.ToCoord(new Vector3(-.1f, -4.1f, -20.99f), cellSize), Is.EqualTo(new Vector3Int(-1, -2, -3)));
		}
	}
}
