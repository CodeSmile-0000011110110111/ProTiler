// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Runtime.Rendering;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEditor;
using ChunkCoord = Unity.Mathematics.int2;
using CellSize = Unity.Mathematics.float3;
using ChunkSize = Unity.Mathematics.int2;

namespace CodeSmile.Tests.Editor.ProTiler3.Rendering
{
	public class Tilemap3DFrustumCullingTests
	{
		public static readonly Object[] CameraPositions =
		{
			new Object[] { new CellSize(0f, 20f, 0f) },
			new Object[] { new CellSize(33f, 44f, 55f) },
			new Object[] { new CellSize(-33f, 66f, -55f) },
		};

		[Test] public void CreateFrustumCullingAsBaseIsNotNull()
		{
			var culling = new TestTilemap3DTopDownCulling() as Tilemap3DCullingBase;

			Assert.NotNull(culling);
		}

		[Test] public void GetCameraReturnsSceneViewCamera()
		{
			var culling = new Tilemap3DTopDownCulling();

			var camera = culling.GetMainOrSceneViewCamera();

			Assert.That(camera, Is.EqualTo(SceneView.lastActiveSceneView.camera));
		}

		[Test] [CreateDefaultScene]
		public void GetVisibleChunks()
		{
			var culling = new Tilemap3DTopDownCulling();
			var camera = culling.GetMainOrSceneViewCamera();
			camera.transform.position = new CellSize(10f, 30f, 10f);

			var chunkSize = new ChunkSize(3, 7);
			var cellSize = new CellSize(1, 1, 1);

			var visibleChunks = culling.GetVisibleChunks(chunkSize, cellSize);

			Assert.NotNull(visibleChunks);
			Assert.That(visibleChunks.Count(), Is.EqualTo(9));
			Assert.That(visibleChunks.First(), Is.EqualTo(new ChunkCoord(3, 1)));
		}

		private class TestTilemap3DTopDownCulling : Tilemap3DTopDownCulling {}
	}
}
