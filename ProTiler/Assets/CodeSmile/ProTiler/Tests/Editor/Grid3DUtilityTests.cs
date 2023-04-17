// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using NUnit.Framework;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Grid3DUtilityTests
	{
		[Test]
		public void ToIndex()
		{
			Assert.AreEqual(0, Grid3DUtility.ToIndex(0, 0, 0));
			Assert.AreEqual(0, Grid3DUtility.ToIndex(0, 0, 10));
			Assert.AreEqual(7, Grid3DUtility.ToIndex(1, 2, 3));
			Assert.AreEqual(35, Grid3DUtility.ToIndex(5, 10, 3));
			Assert.AreEqual(53, Grid3DUtility.ToIndex(3, 5, 10));
		}
	}
}
