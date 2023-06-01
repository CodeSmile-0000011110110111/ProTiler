﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using CellSize = UnityEngine.Vector3;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.Tests.Runtime.ProTiler.Rendering
{
	[ExcludeFromCodeCoverage]
	public sealed class TestCulling : Tilemap3DCullingBase
	{
		private ChunkSize m_Size = new(2, 2);

		public TestCulling(ChunkSize size) => m_Size = size;
		public TestCulling(Int32 width, Int32 length) => m_Size = new ChunkSize(width, length);

		public override IEnumerable<Vector3Int> GetVisibleCoords(ChunkSize chunkSize, CellSize cellSize)
		{
			var coords = new List<Vector3Int>(m_Size.x * m_Size.y);

			for (var z = 0; z < m_Size.y; z++)
				for (var x = 0; x < m_Size.x; x++)
					coords.Add(new Vector3Int(x, 0, z));

			return coords;
		}
	}
}
