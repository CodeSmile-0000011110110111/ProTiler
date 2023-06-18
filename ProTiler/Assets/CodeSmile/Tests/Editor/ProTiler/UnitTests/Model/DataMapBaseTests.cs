// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using NUnit.Framework;
using System;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using LocalCoord = Unity.Mathematics.int3;
using LocalPos = Unity.Mathematics.float3;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Editor.ProTiler.UnitTests.Model
{
	public class DataMapBaseTests
	{
		private static readonly Object[] GridToChunkCoordParams =
		{
			new Object[] { new WorldCoord(0, 0, 0), new ChunkSize(2, 0, 2), new ChunkCoord(0, 0) },
			new Object[] { new WorldCoord(1, 1, 1), new ChunkSize(2, 0, 2), new ChunkCoord(0, 0) },
			new Object[] { new WorldCoord(2, 2, 2), new ChunkSize(2, -1, 2), new ChunkCoord(1, 1) },
			new Object[] { new WorldCoord(4, 5, 7), new ChunkSize(3, 2, 3), new ChunkCoord(1, 2) },
			new Object[] { new WorldCoord(9, -1, 11), new ChunkSize(3, -5, 3), new ChunkCoord(3, 3) },
			new Object[] { new WorldCoord(-1, 0, -1), new ChunkSize(2, 5, 2), new ChunkCoord(-1, -1) },
			new Object[] { new WorldCoord(-2, 0, -2), new ChunkSize(2, 1, 2), new ChunkCoord(-1, -1) },
			new Object[] { new WorldCoord(-3, 0, -3), new ChunkSize(2, 0, 2), new ChunkCoord(-2, -2) },
			new Object[] { new WorldCoord(-4, 0, -4), new ChunkSize(2, 2, 2), new ChunkCoord(-2, -2) },
			new Object[] { new WorldCoord(-16, 0, -32), new ChunkSize(4, 6, 8), new ChunkCoord(-4, -4) },
		};

		[Test] public void Ctor_InvalidChunkSize_SizeIsMinimum2x2()
		{
			Assert.Fail();
		}

		[TestCaseSource(nameof(GridToChunkCoordParams))]
		public void ToChunkCoord_WithWorldCoord_CalculatesCorrectChunkCoord(
			WorldCoord gridCoord, ChunkSize chunkSize, ChunkCoord expected)
		{
			var map = new TestDataMap(chunkSize);
			Assert.That(map.ToChunkCoord(gridCoord), Is.EqualTo(expected));
		}

		public class TestDataMap : DataMapBase
		{
			public TestDataMap(ChunkSize chunkSize)
				: base(chunkSize) {}
		}
	}
}
