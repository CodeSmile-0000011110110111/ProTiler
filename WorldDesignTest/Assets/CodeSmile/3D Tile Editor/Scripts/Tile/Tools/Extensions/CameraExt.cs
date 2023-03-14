// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.Tile
{
	public static class CameraExt
	{
		public static bool IsCurrentCameraValid() => Camera.current != null;
	}
}