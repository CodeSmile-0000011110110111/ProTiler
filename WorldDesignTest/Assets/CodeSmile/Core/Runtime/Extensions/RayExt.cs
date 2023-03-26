// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.Extensions
{
	public static class RayExt
	{
		private const float MinDenominator = 0.00001f;

		/// <summary>
		/// Test intersection of ray with a virtual XZ plane with given plane height (default: 0).
		/// "Virtual" because it does not rely on actual geometry in the world and doesn't use the Plane class. 
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="intersectPoint">the intersection point in world coordinates, or default if there was no intersection</param>
		/// <param name="planeY">the assumed "height" (Y) of the plane, default: 0</param>
		/// <returns>true if the ray intersected with the plane</returns>
		public static bool IntersectsPlane(this Ray ray, out float3 intersectPoint, float planeY = 0f)
		{
			// try pick the virtual XZ plane (with Y = planeY)
			var planeNormal = math.down();
			var planePoint = new float3(0f, planeY, 0f);
			var rayDirection = (float3)ray.direction;
			var denominator = math.dot(rayDirection, planeNormal);
			var intersects = denominator >= MinDenominator;
			if (intersects)
			{
				var rayOrigin = (float3)ray.origin;
				var distanceToPlane = math.dot(planePoint - rayOrigin, planeNormal) / denominator;
				intersectPoint = rayOrigin + rayDirection * distanceToPlane;
				intersectPoint.y = planeY;
			}
			else
				intersectPoint = default;

			return intersects;
		}

		/// <summary>
		/// Test intersection of ray with a virtual XZ plane with given plane height (default: 0).
		/// "Virtual" because it does not rely on actual geometry in the world and doesn't use the Plane class.
		/// This version provides a Vector3 as out parameter, internally calls the float3 version. 
		/// </summary>
		/// <param name="ray"></param>
		/// <param name="intersectPoint">the intersection point in world coordinates, or default if there was no intersection</param>
		/// <param name="planeY">the assumed "height" (Y) of the plane, default: 0</param>
		/// <returns>true if the ray intersected with the plane</returns>
		public static bool IntersectsPlane(this Ray ray, out Vector3 intersectPoint, float planeY = 0f)
		{
			intersectPoint = IntersectsPlane(ray, out float3 point, planeY) ? point : default(Vector3);
			return false;
		}
	}
}