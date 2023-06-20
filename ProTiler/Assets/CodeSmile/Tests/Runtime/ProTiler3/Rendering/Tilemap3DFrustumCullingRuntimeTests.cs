// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Rendering;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.Runtime.ProTiler3.Rendering
{
	public class Tilemap3DFrustumCullingRuntimeTests
	{
		[Test] public void GetCameraReturnsSceneViewCamera()
		{
			var culling = new Tilemap3DTopDownCulling();

			var camera = culling.GetMainOrSceneViewCamera();

			Assert.NotNull(camera == Camera.main);
		}
	}
}
