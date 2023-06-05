// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Runtime.Grid;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.Editor.ProTiler3.Grid
{
	public class Grid3DCursorTests
	{
		[Test] public void TestEquality()
		{
			var cursor1 = new Grid3DCursor(new Ray(Vector3.one, Vector3.down), Vector3.one);
			var cursor2 = new Grid3DCursor(new Ray(Vector3.one, Vector3.down), Vector3.one);

			Assert.That(cursor1.Equals(cursor2));
			Assert.That(cursor2.Equals(cursor1));
			Assert.That(cursor1.Equals((object)cursor2));
			Assert.That(cursor2.Equals((object)cursor1));
			Assert.That(cursor1 == cursor2);
			Assert.That(cursor1.GetHashCode() == cursor2.GetHashCode());
		}

		[Test] public void TestInequality()
		{
			var cursor1 = new Grid3DCursor(new Ray(Vector3.zero, Vector3.down), Vector3.one);
			var cursor2 = new Grid3DCursor(new Ray(Vector3.one, Vector3.down), Vector3.one);

			Assert.That(cursor1.Equals(cursor2) == false);
			Assert.That(cursor2.Equals(cursor1) == false);
			Assert.That(cursor1.Equals((object)cursor2) == false);
			Assert.That(cursor2.Equals((object)cursor1) == false);
			Assert.That(cursor1 != cursor2);
			Assert.That(cursor1.GetHashCode() != cursor2.GetHashCode());
		}
	}
}
