// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Core.Runtime.Serialization;
using CodeSmile.Core.Runtime.Serialization.BinaryAdapters;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Serialization.Binary;
using UnityEngine;
using UnityEngine.TestTools;
using ChunkCoord = Unity.Mathematics.int3;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Editor.ProTiler.UnitTests.Model
{
	public class LinearDataMapChunkSerializationTests
	{
		private static LinearDataMapChunk<SerializationTestData> CreateChunkWithTestData(ChunkCoord chunkSize)
		{
			var chunk = new LinearDataMapChunk<SerializationTestData>(chunkSize);

			var i = 0;
			for (var y = 0; y < chunkSize.y; y++)
			{
				for (var z = 0; z < chunkSize.z; z++)
				{
					for (var x = 0; x < chunkSize.x; x++)
					{
						var localCoord = new ChunkCoord(x, y, z);
						chunk.SetData(localCoord, new SerializationTestData { Coord = localCoord, Index = i++ });
					}
				}
			}

			return chunk;
		}

		[Test]
		public void ChunkWithData_WhenSerialized_CanDeserialize()
		{
			var axisSize = 4;
			var chunkSize = new ChunkSize(axisSize, axisSize, axisSize);
			using (var chunk = CreateChunkWithTestData(chunkSize))
			{
				var bytes = Serialize.ToBinary(chunk, new List<IBinaryAdapter>()
				{
					// TODO: make LinearDataMapChunkBinaryAdapter
					//new UnsafeListBinaryAdapter<T>(),
				});
				Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
			}
		}

		[Test] public void LinearDataMapChunkSerializationTestsSimplePasses()
		{
			// Use the Assert class to test conditions.
		}

		// A UnityTest behaves like a coroutine in PlayMode
		// and allows you to yield null to skip a frame in EditMode
		[UnityTest]
		public IEnumerator LinearDataMapChunkSerializationTestsWithEnumeratorPasses()
		{
			// Use the Assert class to test conditions.
			// yield to skip a frame
			yield return null;
		}

		public struct SerializationTestData
		{
			public ChunkCoord Coord;
			public Int32 Index;
		}
	}
}
