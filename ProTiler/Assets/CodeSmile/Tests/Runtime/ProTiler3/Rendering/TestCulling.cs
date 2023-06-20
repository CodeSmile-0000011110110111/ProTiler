// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CellSize = Unity.Mathematics.float3;
using ChunkSize = Unity.Mathematics.int2;
using GridCoord = Unity.Mathematics.int3;

namespace CodeSmile.Tests.Runtime.ProTiler3.Rendering
{
	[ExcludeFromCodeCoverage]
	public sealed class TestCulling : Tilemap3DCullingBase
	{
		private readonly ChunkSize m_Size = new(2, 2);

		public TestCulling(ChunkSize size) => m_Size = size;
		public TestCulling(Int32 width, Int32 length) => m_Size = new ChunkSize(width, length);

		public override IEnumerable<GridCoord> GetVisibleCoords(ChunkSize chunkSize, CellSize cellSize)
		{
			var coords = new List<GridCoord>(m_Size.x * m_Size.y);

			for (var z = 0; z < m_Size.y; z++)
				for (var x = 0; x < m_Size.x; x++)
					coords.Add(new GridCoord(x, 0, z));

			return coords;
		}
	}
}
