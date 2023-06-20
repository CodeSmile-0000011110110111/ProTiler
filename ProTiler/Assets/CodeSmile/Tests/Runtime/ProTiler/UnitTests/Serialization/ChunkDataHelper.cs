using CodeSmile.ProTiler.CodeDesign.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization
{
	public static class ChunkDataHelper
	{
		public static IReadOnlyList<int3> CreateChunkCoords(int3 chunkSize)
		{
			var coords = new List<int3>();

			for (var y = 0; y < chunkSize.y; y++)
			{
				for (var z = 0; z < chunkSize.z; z++)
				{
					for (var x = 0; x < chunkSize.x; x++)
						coords.Add(new int3(x, y, z));
				}
			}

			return coords;
		}

		public static LinearDataMapChunk<SerializationTestData> CreateChunkFilledWithData(int3 chunkSize)
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
	}
}
