// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed class TileGrid
	{
		private static GridSize MinGridSize = new(1, 1, 1);

		[SerializeField] private GridSize m_GridSize;
		//public Vector3Int Gap { get => m_Gap; set => m_Gap = value; }
		//[SerializeField] private Vector3Int m_Gap;

		public TileGrid(GridSize gridSize)
		{
			m_GridSize = gridSize;
			ClampGridSize();
		}

		public GridSize Size { get => m_GridSize; set => m_GridSize = value; }

		public void ClampGridSize() => m_GridSize = math.max(m_GridSize, MinGridSize);

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
	}
}