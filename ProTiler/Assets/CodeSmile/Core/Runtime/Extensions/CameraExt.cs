// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile.Extensions
{
	public static class CameraExt
	{
		public static Boolean IsPositionInViewport(this Camera camera, Vector3 position) =>
			camera.IsScreenPointInView(camera.WorldToScreenPoint(position));

		public static Boolean IsScreenPointInView(this Camera camera, Vector3 screenPoint) => screenPoint.z >= 0f &&
			screenPoint.y >= 0f && screenPoint.x >= 0f &&
			screenPoint.x < camera.pixelWidth && screenPoint.y < camera.pixelHeight;
	}
}
