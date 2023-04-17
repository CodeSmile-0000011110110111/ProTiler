// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.ProTiler.Tests.Editor.old
{
	public class RectIntExtTests
	{
		[Test]
		public void Intersects()
		{
			{
				var rect1 = new RectInt(0, 0, 1, 1);
				var rect2 = new RectInt(1, 1, 1, 1);
				var intersects = rect1.Intersects(rect2, out var intersection);
				Assert.IsFalse(intersects);
				Assert.AreEqual(new RectInt(), intersection);
			}
			{
				var rect1 = new RectInt(0, 0, 10, 10);
				var rect2 = new RectInt(1, 1, 1, 1);
				var intersects = rect1.Intersects(rect2, out var intersection);
				Assert.IsTrue(intersects);
				Assert.AreEqual(rect2, intersection);
			}
			{
				var rect1 = new RectInt(1, 1, 1, 1);
				var rect2 = new RectInt(0, 0, 10, 10);
				var intersects = rect1.Intersects(rect2, out var intersection);
				Assert.IsTrue(intersects);
				Assert.AreEqual(rect1, intersection);
			}
			{
				var rect1 = new RectInt(0, 0, 10, 10);
				var rect2 = new RectInt(5, 5, 10, 10);
				var intersects = rect1.Intersects(rect2, out var intersection);
				Assert.IsTrue(intersects);
				Assert.AreEqual(new RectInt(5, 5, 5, 5), intersection);
			}

			{
				var rect1 = new RectInt(-5, -5, 10, 10);
				var rect2 = new RectInt(-1, -1, 2, 2);
				var intersects = rect1.Intersects(rect2, out var intersection);
				Assert.IsTrue(intersects);
				Assert.AreEqual(rect2, intersection);
			}
		}
	}
}
