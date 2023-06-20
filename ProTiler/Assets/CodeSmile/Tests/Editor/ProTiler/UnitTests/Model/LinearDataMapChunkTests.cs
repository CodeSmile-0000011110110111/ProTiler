// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.Model;
using NUnit.Framework;
using System;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using LocalCoord = Unity.Mathematics.int3;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Editor.ProTiler.UnitTests.Model
{
	public class LinearDataMapChunkTests
	{
		private const Int32 s_TestChunkSizeX = 4;
		private const Int32 s_TestChunkSizeZ = 4;
		private static readonly ChunkSize s_TestChunkSize = new(s_TestChunkSizeX, 0, s_TestChunkSizeZ);

		[Test] public void DefaultCtor_WhenDataAccessed_IsNotCreated()
		{
			using (var chunk = new LinearDataMapChunk<TestData>())
				Assert.That(chunk.GetWritableData().IsCreated == false);
		}

		[Test] public void ChunkSizeCtor_WhenDataAccessed_IsCreated()
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
				Assert.That(chunk.GetWritableData().IsCreated);
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)]
		public void ChunkSizeCtor_ForChunkSizeY_PreAllocatesHeightLayers(Int32 height)
		{
			var chunkSize = new ChunkSize(4, height, 7);
			using (var chunk = new LinearDataMapChunk<TestData>(chunkSize))
				Assert.That(chunk.Data.Length, Is.EqualTo(chunkSize.x * height * chunkSize.z));
		}

		[TestCase(Int32.MinValue)] [TestCase(-1)] [TestCase(0)]
		public void ChunkSizeCtor_ForZeroOrNegativeSizeY_DoesNotPreAllocate(Int32 height)
		{
			var chunkSize = new ChunkSize(2, height, 2);
			using (var chunk = new LinearDataMapChunk<TestData>(chunkSize))
				Assert.That(chunk.Data.Length, Is.Zero);
		}

		[TestCase(Int32.MinValue)] [TestCase(-1)] [TestCase(0)]
		public void ChunkSizeCtor_ForNegativeSizeAxis_SizeAxisIsZero(Int32 axisSize)
		{
			var chunkSize = new ChunkSize(axisSize, axisSize, axisSize);
			using (var chunk = new LinearDataMapChunk<TestData>(chunkSize))
				Assert.That(chunk.Size, Is.EqualTo(ChunkSize.zero));
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)]
		public void CoordIndexer_WhenDataAdded_CollectionExpands(Int32 height)
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
			{
				var data = new TestData { value = 1 };
				var localCoord = new LocalCoord(0, height, 0);

				chunk.SetData(localCoord, data);

				var chunkDataLength = chunk.Data.Length;
				Assert.That(chunkDataLength, Is.EqualTo(chunk.Size.x * (height + 1) * chunk.Size.x));
			}
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)]
		public void IndexIndexer_WhenDataAdded_CollectionExpands(Int32 height)
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
			{
				var data = new TestData { value = 1 };
				var index = chunk.Size.x * height * chunk.Size.z;

				chunk.SetData(index, data);

				var chunkDataLength = chunk.Data.Length;
				Assert.That(chunkDataLength, Is.EqualTo(chunk.Size.x * (height + 1) * chunk.Size.x));
			}
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)]
		public void CoordIndexer_ExpandingHeightLayer_SizeYIsNewHeightLayerCount(Int32 height)
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
			{
				var data = new TestData { value = 1 };
				var localCoord = new LocalCoord(0, height, 0);

				chunk.SetData(localCoord, data);

				Assert.That(chunk.Size.y, Is.EqualTo(height + 1));
			}
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)]
		public void IndexIndexer_ExpandingHeightLayer_SizeYIsNewHeightLayerCount(Int32 height)
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
			{
				var data = new TestData { value = 1 };
				var index = chunk.Size.x * height * chunk.Size.z;

				chunk.SetData(index, data);

				Assert.That(chunk.Size.y, Is.EqualTo(height + 1));
			}
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)]
		public void CoordIndexer_AddAndReadData_IsIdenticalData(Int32 height)
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
			{
				var data = new TestData { value = (Byte)(height + 1) };
				var localCoord = new LocalCoord(0, height, 0);

				chunk.SetData(localCoord, data);
				var returnedData = chunk.GetData(localCoord);

				Assert.That(returnedData, Is.EqualTo(data));
			}
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)]
		public void IndexIndexer_AddAndReadData_IsIdenticalData(Int32 height)
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
			{
				var data = new TestData { value = (Byte)(height + 1) };
				var index = chunk.Size.x * height * chunk.Size.z;

				chunk.SetData(index, data);
				var returnedData = chunk.GetData(index);

				Assert.That(returnedData, Is.EqualTo(data));
			}
		}

		[TestCase(-1, -1, -1)] [TestCase(0, 0, 0)]
		public void CoordIndexer_GetDataWithInvalidCoords_ThrowsIndexOutOfRangeException(Int32 x, Int32 y, Int32 z)
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
				Assert.Throws<IndexOutOfRangeException>(() => { chunk.GetData(new LocalCoord(x, y, z)); });
		}

		[TestCase(-1)] [TestCase(Int32.MinValue)]
		public void IndexIndexer_GetDataWithInvalidCoords_ThrowsIndexOutOfRangeException(Int32 index)
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
				Assert.Throws<IndexOutOfRangeException>(() => { chunk.GetData(index); });
		}

		public struct TestData : IEquatable<TestData>
		{
			public Byte value;

			public Boolean Equals(TestData other) => value == other.value;
			public override Boolean Equals(Object obj) => obj is TestData other && Equals(other);
			public override Int32 GetHashCode() => value.GetHashCode();
			public static Boolean operator ==(TestData left, TestData right) => left.Equals(right);
			public static Boolean operator !=(TestData left, TestData right) => !left.Equals(right);
		}
	}
}
