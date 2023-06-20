﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using GridCoord = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using ChunkSize = Unity.Mathematics.int2;

namespace CodeSmile.ProTiler3.Rendering
{
	public abstract class Tilemap3DCullingBase : ITilemap3DFrustumCulling
	{
		public abstract IEnumerable<GridCoord> GetVisibleCoords(ChunkSize chunkSize, CellSize cellSize);
	}
}
