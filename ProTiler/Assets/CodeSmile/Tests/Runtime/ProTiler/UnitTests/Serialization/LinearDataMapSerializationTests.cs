// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Model;
using CodeSmile.ProTiler.Serialization;
using CodeSmile.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Serialization.Binary;
using UnityEngine;
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
	public class LinearDataMapSerializationTests
	{
		[Test]
		public void LinearDataMap_WhenEmptyAndSerialized_CanDeserialize()
		{
			var chunkSize = new ChunkSize(3, 4, 7);
			var map = new LinearDataMap<LinearTestData>(chunkSize);

			var adapterVersion = (Byte)2;
			var dataVersion = (Byte)5;
			var adapters = new List<IBinaryAdapter>
			{
				new LinearDataMapBinaryAdapter<LinearTestData>(adapterVersion, dataVersion),
				//new NativeParallelHashMapBinaryAdapter<ChunkKey, LinearDataMapChunk<TData>>()
			};
			var bytes = Serialize.ToBinary(map, adapters);
			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");

			var deserializedMap = Serialize.FromBinary<LinearDataMap<LinearTestData>>(bytes, adapters);

			Assert.That(deserializedMap.ChunkSize, Is.EqualTo(map.ChunkSize));
			Assert.That(deserializedMap.Chunks.IsCreated);
			Assert.That(deserializedMap.Chunks.Count(), Is.EqualTo(map.Chunks.Count()));
		}

		public struct LinearTestData
		{
			public Byte TestValue;
		}
	}
}
