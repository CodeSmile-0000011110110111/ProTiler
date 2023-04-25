// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.Editor
{
	public class RectIntExtTests
	{
		public static object[] DoesIntersectSource =
		{
			new[] { new RectInt(0, 0, 2, 2), new RectInt(1, 1, 1, 1) },
			new[] { new RectInt(0, 0, 2, 2), new RectInt(1, 1, 1, 1) },
			new[] { new RectInt(-1, -1, 2, 2), new RectInt(0, 0, 1, 1) },
			new[] { new RectInt(0, 0, 10, 10), new RectInt(1, 1, 1, 1) },
			new[] { new RectInt(-5, -5, 10, 10), new RectInt(-1, -1, 2, 2) },
		};

		public static object[] DoesNotIntersectSource =
		{
			new[] { new RectInt(0, 0, 1, 2), new RectInt(1, 1, 1, 1) },
			new[] { new RectInt(0, 0, 2, 1), new RectInt(1, 1, 1, 1) },
			new[] { new RectInt(0, 0, 1, 1), new RectInt(1, 1, 1, 1) },
			new[] { new RectInt(1, 1, 1, 1), new RectInt(0, 0, 1, 1) },
		};

		[TestCaseSource(nameof(DoesIntersectSource))]
		public void DoesIntersect(RectInt rect1, RectInt rect2)
		{
			Assert.That(rect1.Intersects(rect2, out var intersection1), Is.True);
			Assert.That(rect2.Intersects(rect1, out var intersection2), Is.True);
		}

		[TestCaseSource(nameof(DoesNotIntersectSource))]
		public void DoesNotIntersect(RectInt rect1, RectInt rect2)
		{
			Assert.That(rect1.Intersects(rect2, out var intersection1), Is.False);
			Assert.That(rect2.Intersects(rect1, out var intersection2), Is.False);
		}
	}
}
