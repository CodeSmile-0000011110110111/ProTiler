// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Grid3DUtilityTests
	{
		[Test]
		public void ToIndex2D()
		{
			Assert.AreEqual(0, Grid3DUtility.ToIndex2D(0, 0, 0));
			Assert.AreEqual(0, Grid3DUtility.ToIndex2D(0, 0, 10));
			Assert.AreEqual(7, Grid3DUtility.ToIndex2D(1, 2, 3));
			Assert.AreEqual(35, Grid3DUtility.ToIndex2D(5, 10, 3));
			Assert.AreEqual(53, Grid3DUtility.ToIndex2D(3, 5, 10));

			Assert.AreEqual(-1, Grid3DUtility.ToIndex2D(-1, 0, 10));
			Assert.AreEqual(-10, Grid3DUtility.ToIndex2D(0, -1, 10));
			Assert.AreEqual(-11, Grid3DUtility.ToIndex2D(-1, -1, 10));

			Assert.AreEqual(0, Grid3DUtility.ToIndex2D(Vector3Int.zero, 0));
			Assert.AreEqual(35, Grid3DUtility.ToIndex2D(new Vector3Int(5, 0, 10), 3));
			Assert.AreEqual(53, Grid3DUtility.ToIndex2D(new Vector3Int(3, 0, 5), 10));
		}

		[Test]
		public void ToCoord()
		{
			var width = 7;
			Assert.AreEqual(Vector3Int.zero, Grid3DUtility.ToCoord(0, width));
			Assert.AreEqual(new Vector3Int(0,3,0), Grid3DUtility.ToCoord(0, width, 3));
			Assert.AreEqual(new Vector3Int(0,5,7), Grid3DUtility.ToCoord(49, width, 5));
			Assert.AreEqual(new Vector3Int(5,9,8), Grid3DUtility.ToCoord(61, width, 9));
		}
	}
}
