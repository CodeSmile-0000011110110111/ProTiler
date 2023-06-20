﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Model;
using NUnit.Framework;
using System.Linq;
using WorldCoord = Unity.Mathematics.int3;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int2;

namespace CodeSmile.Tests.Editor.ProTiler3.Model
{
	public class Tilemap3DUtilityTests
	{
		private static readonly object[] ChunkCoords =
		{
			new object[] { new ChunkCoord(0, 0), new ChunkSize(2, 2), new WorldCoord(0, 0, 0) },
			new object[] { new ChunkCoord(2, 3), new ChunkSize(8, 8), new WorldCoord(16, 0, 24) },
		};

		private static readonly object[] LayerToGridCoordParams =
		{
			new object[] { new WorldCoord(0, 0, 0), new ChunkCoord(0, 0), new ChunkSize(2, 2), new WorldCoord(0, 0, 0) },
			new object[] { new WorldCoord(0, 0, 0), new ChunkCoord(1, 1), new ChunkSize(2, 2), new WorldCoord(2, 0, 2) },
			new object[] { new WorldCoord(1, 1, 1), new ChunkCoord(1, 1), new ChunkSize(2, 2), new WorldCoord(3, 1, 3) },
			new object[] { new WorldCoord(2, 2, 2), new ChunkCoord(0, 0), new ChunkSize(2, 2), new WorldCoord(2, 2, 2) },
			new object[] { new WorldCoord(2, 2, 2), new ChunkCoord(1, 1), new ChunkSize(2, 2), new WorldCoord(4, 2, 4) },
			new object[] { new WorldCoord(2, 3, 2), new ChunkCoord(3, 4), new ChunkSize(2, 2), new WorldCoord(8, 3, 10) },
		};

		private static readonly object[] GridToChunkCoordParams =
		{
			new object[] { new WorldCoord(0, 0, 0), new ChunkSize(2, 2), new ChunkCoord(0, 0) },
			new object[] { new WorldCoord(1, 1, 1), new ChunkSize(2, 2), new ChunkCoord(0, 0) },
			new object[] { new WorldCoord(2, 2, 2), new ChunkSize(2, 2), new ChunkCoord(1, 1) },
			new object[] { new WorldCoord(4, 5, 7), new ChunkSize(3, 3), new ChunkCoord(1, 2) },
			new object[] { new WorldCoord(9, -1, 11), new ChunkSize(3, 3), new ChunkCoord(3, 3) },
			new object[] { new WorldCoord(-1, 0, -1), new ChunkSize(2, 2), new ChunkCoord(-1, -1) },
			new object[] { new WorldCoord(-2, 0, -2), new ChunkSize(2, 2), new ChunkCoord(-1, -1) },
			new object[] { new WorldCoord(-3, 0, -3), new ChunkSize(2, 2), new ChunkCoord(-2, -2) },
			new object[] { new WorldCoord(-4, 0, -4), new ChunkSize(2, 2), new ChunkCoord(-2, -2) },
			new object[] { new WorldCoord(-16, 0, -32), new ChunkSize(4, 8), new ChunkCoord(-4, -4) },
		};

		private static readonly object[] GridToLayerCoordParams =
		{
			new object[] { new WorldCoord(0, 0, 0), new ChunkCoord(2, 2), new WorldCoord(0, 0, 0) },
			new object[] { new WorldCoord(3, 4, 5), new ChunkCoord(2, 2), new WorldCoord(1, 4, 1) },
			new object[] { new WorldCoord(8, 8, 8), new ChunkCoord(4, 3), new WorldCoord(0, 8, 2) },
			new object[] { new WorldCoord(-8, -8, -8), new ChunkCoord(4, 3), new WorldCoord(0, 0, 2) },
			new object[] { new WorldCoord(9, 9, 9), new ChunkCoord(4, 3), new WorldCoord(1, 9, 0) },
			new object[] { new WorldCoord(-9, -9, -9), new ChunkCoord(4, 3), new WorldCoord(1, 0, 0) },
		};

		private static readonly object[] ChunkToGridCoordParams =
		{
			new object[] { new ChunkCoord(0, 0), new ChunkSize(2, 2), new WorldCoord(0, 0, 0) },
			new object[] { new ChunkCoord(1, 1), new ChunkSize(2, 2), new WorldCoord(2, 0, 2) },
			new object[] { new ChunkCoord(3, 3), new ChunkSize(3, 4), new WorldCoord(9, 0, 12) },
			new object[] { new ChunkCoord(-1, 0), new ChunkSize(2, 2), new WorldCoord(-2, 0, 0) },
			new object[] { new ChunkCoord(0, -1), new ChunkSize(3, 3), new WorldCoord(0, 0, -3) },
			new object[] { new ChunkCoord(-1, -1), new ChunkSize(4, 4), new WorldCoord(-4, 0, -4) },
		};

		[TestCaseSource(nameof(ChunkCoords))]
		public void GetAllChunkLayerCoordsCorrectness(ChunkCoord chunkCoord, ChunkSize chunkSize, WorldCoord firstCoord)
		{
			var coords = Tilemap3DUtility.GetChunkGridCoords(chunkCoord, chunkSize);

			var expected = new WorldCoord(chunkCoord.x * chunkSize.x, 0, chunkCoord.y * chunkSize.y);
			Assert.That(coords.Count(), Is.EqualTo(chunkSize.x * chunkSize.y));
			Assert.That(coords.First(), Is.EqualTo(firstCoord));
		}

		[TestCaseSource(nameof(LayerToGridCoordParams))]
		public void LayerToGridCoordIsCorrect(WorldCoord layerCoord, ChunkCoord chunkCoord, ChunkSize chunkSize,
			WorldCoord expected) => Assert.That(Tilemap3DUtility.LayerToGridCoord(layerCoord, chunkCoord, chunkSize),
			Is.EqualTo(expected));

		[TestCaseSource(nameof(GridToChunkCoordParams))]
		public void GridToChunkCoordIsCorrect(WorldCoord gridCoord, ChunkSize chunkSize, ChunkCoord expected) =>
			Assert.That(Tilemap3DUtility.GridToChunkCoord(gridCoord, chunkSize), Is.EqualTo(expected));

		[TestCaseSource(nameof(GridToLayerCoordParams))]
		public void GridToLayerCoordIsCorrect(WorldCoord gridCoord, ChunkSize chunkSize, WorldCoord expected) =>
			Assert.That(Tilemap3DUtility.GridToLayerCoord(gridCoord, chunkSize), Is.EqualTo(expected));


		[TestCaseSource(nameof(ChunkToGridCoordParams))]
		public void ChunkToGridCoordIsCorrect(ChunkCoord chunkCoord, ChunkSize chunkSize, WorldCoord expected) =>
			Assert.That(Tilemap3DUtility.ChunkToGridCoord(chunkCoord, chunkSize), Is.EqualTo(expected));
	}
}
