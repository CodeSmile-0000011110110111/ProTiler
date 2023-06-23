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
using Object = System.Object;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization
{
	public class GridMapSerializationTests
	{
		[Test] public void GridMap_WhenEmptyAndSerialized_CanDeserialize()
		{
			var chunkSize = new ChunkSize(3, 4, 7);
			var adapters = new List<IBinaryAdapter>
			{
				new GridMapBinaryAdapter<TestGridMap>(0),
			};

			using (var testmap = new TestGridMap(chunkSize, 0))
			{
				var bytes = Serialize.ToBinary(testmap, adapters);
				Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");

				using (var deserializedMap = Serialize.FromBinary<TestGridMap>(bytes, adapters))
					Assert.That(deserializedMap.ChunkSize, Is.EqualTo(testmap.ChunkSize));
			}
		}

		[Test] public void GridMap_WithMapsAndSerialized_CanDeserialize()
		{
			var chunkSize = new ChunkSize(2, 1, 3);
			var adapters = new List<IBinaryAdapter>
			{
				new GridMapBinaryAdapter<TestGridMap>(0),
			};

			using (var testmap = new TestGridMap(chunkSize, 0))
			{
				testmap.AddLinearDataMap<SerializationTestData>(0);
				testmap.AddSparseDataMap<SerializationTestData>(0);

				var bytes = Serialize.ToBinary(testmap, adapters);
				Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");

				using (var deserializedMap = Serialize.FromBinary<TestGridMap>(bytes, adapters))
				{
					Assert.That(deserializedMap.LinearMaps.Count, Is.EqualTo(testmap.LinearMaps.Count));
					Assert.That(deserializedMap.SparseMaps.Count, Is.EqualTo(testmap.SparseMaps.Count));
				}
			}
		}

		public sealed class TestGridMap : GridMapBase
		{
			public TestGridMap() : this(ChunkSize.zero, 0) {}

			public TestGridMap(ChunkSize chunkSize, Byte gridVersion) : base(chunkSize, gridVersion) {}

			public TestGridMap(ChunkSize chunkSize, GridMapBinaryAdapter<TestGridMap> binaryAdapter) :
				base(chunkSize, 0) {}

			public List<IBinaryAdapter> GetBinaryAdapters(Object dataVersion) => throw new NotImplementedException();
		}
	}
}
