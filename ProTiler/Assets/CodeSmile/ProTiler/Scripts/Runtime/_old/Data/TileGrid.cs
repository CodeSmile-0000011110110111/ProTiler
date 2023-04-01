// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using System;
using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.ProTiler.Data
{
	[Serializable]
	public sealed class TileGrid
	{
		private static readonly GridSize s_MinGridSize = new(1, 1, 1);

		[SerializeField] private GridSize m_GridSize;

		public static WorldRect ToWorldRect(GridRect rect, GridSize gridSize) => new(
			rect.x * gridSize.x, rect.y * gridSize.z,
			rect.width * gridSize.x, rect.height * gridSize.z);

		public TileGrid() => m_GridSize = s_MinGridSize;

		public TileGrid(GridSize gridSize)
		{
			m_GridSize = gridSize;
			ClampGridSize();
		}

		public GridSize Size { get => m_GridSize; set => m_GridSize = value; }

		public void ClampGridSize() => m_GridSize = math.max(m_GridSize, s_MinGridSize);

		public GridSize ToGridCoord(float3 position)
		{
			// Note: a simple int-cast won't do because (int)-0.1f should be -1 and not 0
			// floor() rounds towards infinity vs int-cast rounds towards zero
			var x = (int)math.floor(position.x * (1f / m_GridSize.x));
			var y = (int)math.floor(position.y * (1f / m_GridSize.y));
			var z = (int)math.floor(position.z * (1f / m_GridSize.z));
			return new GridSize(x, y, z);
		}

		public float3 ToWorldPosition(GridCoord coord) => new(coord.x * m_GridSize.x, coord.y * m_GridSize.y, coord.z * m_GridSize.z);

		public RectInt GetCameraRect(Camera camera, int drawDistance, float distanceMultiplier)
		{
			if (camera == null)
				return new GridRect();
					
			var camTrans = camera.transform;
			var rayOrigin = camTrans.position + camTrans.forward * (drawDistance * distanceMultiplier);
			var camRay = new Ray(rayOrigin, Vector3.down);
			camRay.IntersectsPlane(out var camPos);

			var camCoord = ToGridCoord(camPos);
			return new GridRect(camCoord.x - drawDistance / 2, camCoord.z - drawDistance / 2, drawDistance, drawDistance);
		}
	}
}