// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Runtime.Grid;
using CodeSmile.ProTiler3.Runtime.Model;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int2;

namespace CodeSmile.ProTiler3.Runtime.Rendering
{
	public class Tilemap3DTopDownCulling : Tilemap3DCullingBase
	{
		private GridCoord m_TestOffset;

		public override IEnumerable<GridCoord> GetVisibleCoords(ChunkSize chunkSize, CellSize cellSize)
		{
			var visibleChunks = GetVisibleChunks(chunkSize, cellSize);
			var visibleCoords = new List<GridCoord>();

			foreach (var chunkCoord in visibleChunks)
			{
				var chunkGridCoords = Tilemap3DUtility.GetChunkGridCoords(chunkCoord, chunkSize);
				visibleCoords.AddRange(chunkGridCoords);
			}

			return visibleCoords;
		}


		public IEnumerable<ChunkCoord> GetVisibleChunks(ChunkSize chunkSize, CellSize cellSize)
		{
			var visibleChunks = new List<ChunkCoord>();

			var startChunkCoord = GetCameraChunkCoord(chunkSize, cellSize);
			visibleChunks.Add(startChunkCoord);

			visibleChunks.Add(startChunkCoord + new ChunkCoord(-1, -1));
			visibleChunks.Add(startChunkCoord + new ChunkCoord(-1, 0));
			visibleChunks.Add(startChunkCoord + new ChunkCoord(-1, 1));
			visibleChunks.Add(startChunkCoord + new ChunkCoord(0, 1));
			visibleChunks.Add(startChunkCoord + new ChunkCoord(1, 1));
			visibleChunks.Add(startChunkCoord + new ChunkCoord(1, 0));
			visibleChunks.Add(startChunkCoord + new ChunkCoord(1, -1));
			visibleChunks.Add(startChunkCoord + new ChunkCoord(0, -1));

			return visibleChunks;
		}

		private ChunkCoord GetCameraChunkCoord(ChunkSize chunkSize, CellSize cellSize)
		{
			var camera = GetMainOrSceneViewCamera();
			var gridCoord = Grid3DUtility.ToGridCoord(camera.transform.position, cellSize);
			var chunkCoord = Tilemap3DUtility.GridToChunkCoord(gridCoord, chunkSize);
			return chunkCoord;
		}


		public Camera GetMainOrSceneViewCamera()
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
				return SceneView.lastActiveSceneView.camera;
#endif

			return Camera.main;
		}
	}
}
