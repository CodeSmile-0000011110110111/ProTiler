// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.Model;
using CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization;
using NUnit.Framework;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkKey = System.Int64;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using LocalCoord = Unity.Mathematics.int3;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Editor.ProTiler.UnitTests.Model
{
	public class LinearDataMapTests
	{
		[Test] public void Ctor_HashMap_IsCreated()
		{
			using (var map = new LinearDataMap<SerializationTestData>())
				Assert.That(map.Chunks.IsCreated);
		}

		[Test] public void TryGetChunk_WhenKeyDoesNotExist_ReturnsFalse()
		{
			using (var map = new LinearDataMap<SerializationTestData>())
				Assert.False(map.TryGetChunk(ChunkKey.MaxValue, out var chunk));
		}

		[Test] public void TryGetChunk_WhenCoordDoesNotExist_ReturnsFalse()
		{
			using (var map = new LinearDataMap<SerializationTestData>())
				Assert.False(map.TryGetChunk(new WorldCoord(0, 0, 0), out var chunk));
		}

		[Test] public void AddChunk_WhenAddedToCoord_GetSameChunkFromCoord()
		{
			using (var map = new LinearDataMap<SerializationTestData>())
			{
				using (var chunk = new LinearDataMapChunk<SerializationTestData>())
				{
					var coord = new WorldCoord(-7, 4, 13);

					map.AddChunk(coord, chunk);
					var gotChunk = map.TryGetChunk(coord, out var outChunk);

					Assert.That(gotChunk);
					Assert.That(outChunk, Is.EqualTo(chunk));
				}
			}
		}
	}
}
