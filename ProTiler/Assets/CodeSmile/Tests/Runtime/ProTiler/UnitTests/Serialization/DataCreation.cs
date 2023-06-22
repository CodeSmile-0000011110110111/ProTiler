// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using LocalCoord = Unity.Mathematics.int3;
using LocalPos = Unity.Mathematics.float3;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization
{
	public static class DataCreation
	{
		public static IReadOnlyList<ChunkSize> CreateChunkCoords(ChunkSize chunkSize)
		{
			var coords = new List<ChunkSize>();

			for (var y = 0; y < chunkSize.y; y++)
			{
				for (var z = 0; z < chunkSize.z; z++)
				{
					for (var x = 0; x < chunkSize.x; x++)
						coords.Add(new ChunkSize(x, y, z));
				}
			}

			return coords;
		}

		public static LinearDataMapChunk<SerializationTestData> CreateChunkFilledWithData(ChunkSize chunkSize)
		{
			var chunk = new LinearDataMapChunk<SerializationTestData>(chunkSize);
			var coords = CreateChunkCoords(chunkSize);
			Assert.That(coords.Count, Is.GreaterThan(0));

			var i = (UInt16)0;
			foreach (var coord in coords)
			{
				var data = new SerializationTestData { Coord = coord, Index = i };
				//Debug.Log($"SetData({data})");
				chunk.SetData(coord, data);

				Assert.That(chunk[coord], Is.EqualTo(data));
				Assert.That(chunk[i], Is.EqualTo(data));
				i++;
			}

			Assert.That(chunk.Data.Length, Is.EqualTo(coords.Count));
			return chunk;
		}

		public static LinearDataMap<SerializationTestData> CreateMapWithFilledChunks(ChunkSize chunkSize,
			ChunkCoord chunkCoords)
		{
			var map = new LinearDataMap<SerializationTestData>();

			for (var y = 0; y < chunkCoords.y; y++)
			{
				for (var x = 0; x < chunkCoords.x; x++)
				{
					var chunk = CreateChunkFilledWithData(chunkSize);
					var chunkCoord = new ChunkCoord(x, y);
					map.AddChunk(chunkCoord, chunk);
					Assert.That(map.TryGetChunk(chunkCoord, out var gotChunk));
					Assert.That(gotChunk, Is.EqualTo(chunk));
				}
			}

			return map;
		}
	}
}
