// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Model;
using CodeSmile.Serialization;
using NUnit.Framework;
using System;
using Unity.Collections.LowLevel.Unsafe;
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
	public class GridMapBaseTests
	{
		[Test] public void TestGrid_WhenInstantiated_HasExpectedChunkSize()
		{
			var chunkSize = new ChunkSize(3, 4, 5);
			var grid = new GridMapBaseTestImpl(chunkSize, 0);

			Assert.That(grid.ChunkSize, Is.EqualTo(chunkSize));
		}

		[Test] public void TestGrid_WhenInstantiated_ContainsExpectedNumberOfMaps()
		{
			var grid = new GridMapBaseTestImpl(new ChunkSize(2, 2, 2), 0);

			Assert.That(grid.LinearMaps.Count, Is.EqualTo(1));
			Assert.That(grid.SparseMaps.Count, Is.EqualTo(1));
		}

		public struct MyLinearTestData : IBinarySerializable
		{
			public Byte byteValue;

			public unsafe void Serialize(UnsafeAppendBuffer* writer) => throw new NotImplementedException();

			public unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte serializedDataVersion) =>
				throw new NotImplementedException();
		}

		public struct MySparseTestData : IBinarySerializable
		{
			public Byte byteValue;

			public unsafe void Serialize(UnsafeAppendBuffer* writer) => throw new NotImplementedException();

			public unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte serializedDataVersion) =>
				throw new NotImplementedException();
		}

		public class GridMapBaseTestImpl : GridMapBase
		{
			public GridMapBaseTestImpl(ChunkSize chunkSize, Byte gridVersion) : base(chunkSize, gridVersion)
			{
				AddLinearDataMap<MyLinearTestData>(2);
				AddSparseDataMap<MySparseTestData>(3);
			}
		}
	}
}
