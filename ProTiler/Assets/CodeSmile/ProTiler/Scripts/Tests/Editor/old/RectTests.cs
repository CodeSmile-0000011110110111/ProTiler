// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using NUnit.Framework;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.ProTiler.Tests.Editor
{
	public class RectTests
	{
		[Test]
		public void IntersectsTest()
		{
			{
				var rect1 = new GridRect(0, 0, 1, 1);
				var rect2 = new GridRect(1, 1, 1, 1);
				var intersects = rect1.Intersects(rect2, out var intersection);
				Assert.IsFalse(intersects);
				Assert.AreEqual(new GridRect(), intersection);
			}
			{
				var rect1 = new GridRect(0, 0, 10, 10);
				var rect2 = new GridRect(1, 1, 1, 1);
				var intersects = rect1.Intersects(rect2, out var intersection);
				Assert.IsTrue(intersects);
				Assert.AreEqual(rect2, intersection);
			}
			{
				var rect1 = new GridRect(1, 1, 1, 1);
				var rect2 = new GridRect(0, 0, 10, 10);
				var intersects = rect1.Intersects(rect2, out var intersection);
				Assert.IsTrue(intersects);
				Assert.AreEqual(rect1, intersection);
			}
			{
				var rect1 = new GridRect(0, 0, 10, 10);
				var rect2 = new GridRect(5, 5, 10, 10);
				var intersects = rect1.Intersects(rect2, out var intersection);
				Assert.IsTrue(intersects);
				Assert.AreEqual(new GridRect(5, 5, 5, 5), intersection);
			}

			{
				var rect1 = new GridRect(-5, -5, 10, 10);
				var rect2 = new GridRect(-1, -1, 2, 2);
				var intersects = rect1.Intersects(rect2, out var intersection);
				Assert.IsTrue(intersects);
				Assert.AreEqual(rect2, intersection);
			}
		}
	}
}