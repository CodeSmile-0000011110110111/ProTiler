// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Grid3DUtilityTests
	{
		[Test] public void CoordToIndex2D_Zero()
		{
			Assert.AreEqual(0, Grid3DUtility.ToIndex2D(0, 0, 0));
			Assert.AreEqual(0, Grid3DUtility.ToIndex2D(0, 0, 10));
		}

		[Test] public void CoordToIndex2D()
		{
			Assert.AreEqual(7, Grid3DUtility.ToIndex2D(1, 2, 3));
			Assert.AreEqual(35, Grid3DUtility.ToIndex2D(5, 10, 3));
			Assert.AreEqual(53, Grid3DUtility.ToIndex2D(3, 5, 10));
		}

		[Test] public void CoordToIndex2D_Negative()
		{
			Assert.AreEqual(-1, Grid3DUtility.ToIndex2D(-1, 0, 10));
			Assert.AreEqual(-10, Grid3DUtility.ToIndex2D(0, -1, 10));
			Assert.AreEqual(-11, Grid3DUtility.ToIndex2D(-1, -1, 10));
		}

		[Test] public void CoordToIndex2D_Vector3()
		{
			Assert.AreEqual(0, Grid3DUtility.ToIndex2D(Vector3Int.zero, 0));
			Assert.AreEqual(35, Grid3DUtility.ToIndex2D(new Vector3Int(5, 0, 10), 3));
			Assert.AreEqual(53, Grid3DUtility.ToIndex2D(new Vector3Int(3, 0, 5), 10));
		}

		[Test] public void Index2DToCoord_Zero() => Assert.AreEqual(Vector3Int.zero, Grid3DUtility.ToCoord(0, 7));

		[Test] public void Index2DToCoord()
		{
			Assert.AreEqual(new Vector3Int(0, 3, 0), Grid3DUtility.ToCoord(0, 7, 3));
			Assert.AreEqual(new Vector3Int(0, 5, 7), Grid3DUtility.ToCoord(49, 7, 5));
			Assert.AreEqual(new Vector3Int(5, 9, 8), Grid3DUtility.ToCoord(61, 7, 9));
		}

		[Test] public void Index2DToCoord_Negative()
		{
			Assert.AreEqual(new Vector3Int(-1, 0, 0), Grid3DUtility.ToCoord(-1, 7));
			Assert.AreEqual(new Vector3Int(-1, 0, -2), Grid3DUtility.ToCoord(-15, 7));
		}

		[Test] public void WorldPositionToCoord_Zero() =>
			Assert.AreEqual(Vector3Int.zero, Grid3DUtility.ToCoord(Vector3.zero, new Vector3Int(3, 4, 7)));

		[Test] public void WorldPositionToCoord()
		{
			var cellSize = new Vector3Int(3, 4, 7);
			Assert.AreEqual(new Vector3Int(0, 0, 0), Grid3DUtility.ToCoord(new Vector3(2.99999f, 3.99999f, 6.99999f), cellSize));
			Assert.AreEqual(new Vector3Int(1, 1, 1), Grid3DUtility.ToCoord(new Vector3(3f, 4f, 7f), cellSize));
		}

		[Test] public void WorldPositionToCoord_Negative()
		{
			var cellSize = new Vector3Int(3, 4, 7);
			Assert.AreEqual(new Vector3Int(-1, -2, -3), Grid3DUtility.ToCoord(new Vector3(-.1f, -4.1f, -20.99f), cellSize));
		}
	}
}
