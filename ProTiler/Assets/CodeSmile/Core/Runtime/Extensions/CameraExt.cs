// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.Extensions
{
	public static class CameraExt
	{
		public static bool IsCurrentCameraValid() => Camera.current != null;

		public static bool IsSceneViewOrGameCamera(Camera camera) =>
			camera != null && (camera.cameraType == CameraType.Game || camera.cameraType == CameraType.SceneView);
	}
}