// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using NUnit.Framework;
using UnityEngine;
using Object = System.Object;

namespace CodeSmile.Tests.Editor.Extensions
{
	public class RayExtTests
	{
		private static readonly Object[] RaysFacingStraightDown =
		{
			new Object[] { new Vector3(0f, .00001f, 0f), Vector3.down },
			new Object[] { new Vector3(0f, 100000f, 0f), Vector3.down },
			new Object[] { new Vector3(0f, -100000f, 0f), Vector3.down },
		};

		private static readonly Object[] RaysFacingDownAtAngle =
		{
			new Object[] { new Vector3(10f, .00001f, 12.34f), new Vector3(0.0001f, -0.0001f, 0.0001f) },
			// flat angle, intersects very far away (denominator close to min value)
			new Object[] { new Vector3(-10f, 10000f, -456.789f), new Vector3(-1f, -0.015f, -1f) },
		};

		private static readonly Object[] RaysNotIntersecting =
		{
			new Object[] { Vector3.zero, Vector3.zero },
			new Object[] { new Vector3(0f, -100000f, 0f), Vector3.up }, // wrong direction
			new Object[] { new Vector3(0f, 100000f, 0f), Vector3.up }, // wrong direction
			new Object[] { new Vector3(0f, 1f, 0f), Vector3.right }, // parallel
			new Object[] { new Vector3(0f, 1f, 0f), Vector3.back }, // parallel
			new Object[] { new Vector3(0f, 1f, 0f), Vector3.left }, // parallel
			new Object[] { new Vector3(0f, 1f, 0f), Vector3.forward }, // parallel
			new Object[] { new Vector3(0f, 1f, 0f), new Vector3(0.001f, 0.000001f, 0.001f) }, // almost parallel
			new Object[] { new Vector3(-10f, 100000f, -456.789f), new Vector3(-0.99f, 0.0001f, -0.99f) },
		};

		[TestCaseSource(nameof(RaysFacingStraightDown))]
		public void RayIntersectsGroundPlaneAtOrigin(Vector3 origin, Vector3 direction)
		{
			var ray = new Ray(origin, direction);

			var didIntersect = ray.IntersectsPlane(out var intersectPoint);

			Assert.True(didIntersect);
			Assert.True(intersectPoint == default);
		}

		[TestCaseSource(nameof(RaysFacingStraightDown))]
		public void RayIntersectsGroundPlaneAtHeight(Vector3 origin, Vector3 direction)
		{
			var height = 123f;
			origin.y += height; // don't fail just because we moved the ground plane up
			var ray = new Ray(origin, direction);

			var didIntersect = ray.IntersectsPlane(out var intersectPoint, 123f);

			Assert.True(didIntersect);
			Assert.True(intersectPoint == new Vector3(0f, height, 0f));
		}

		[TestCaseSource(nameof(RaysFacingDownAtAngle))]
		public void AngledRaysIntersectGroundPlane(Vector3 origin, Vector3 direction)
		{
			var ray = new Ray(origin, direction);

			var didIntersect = ray.IntersectsPlane(out var intersectPoint);

			Assert.True(didIntersect);
			Assert.False(intersectPoint == default);
		}

		[TestCaseSource(nameof(RaysNotIntersecting))]
		public void RaysDoNotIntersectGroundPlane(Vector3 origin, Vector3 direction)
		{
			var ray = new Ray(origin, direction);

			var didIntersect = ray.IntersectsPlane(out var intersectPoint);

			Assert.False(didIntersect);
			Assert.True(intersectPoint == default);
		}
	}
}
