// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile
{
	public static class Ray
	{
		const float MinDenominator = 0.00001f;
		
		public static bool IntersectsVirtualPlane(UnityEngine.Ray ray, out Vector3 intersectPoint, float planeY = 0f)
		{
			intersectPoint = Vector3.negativeInfinity;

			// try pick the virtual XZ plane (with Y = planeY)
			var planeNormal = Vector3.down;
			var planePoint = new Vector3(0f, planeY, 0f);
			var denominator = Vector3.Dot(ray.direction, planeNormal);
			var intersects = denominator >= MinDenominator;
			if (intersects)
			{
				var distanceToPlane = Vector3.Dot(planePoint - ray.origin, planeNormal) / denominator;
				intersectPoint = ray.origin + ray.direction * distanceToPlane;
			}

			return intersects;
		}
	}
}