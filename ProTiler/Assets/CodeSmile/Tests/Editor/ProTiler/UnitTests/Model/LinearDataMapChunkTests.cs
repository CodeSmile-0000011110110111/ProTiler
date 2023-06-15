// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using NUnit.Framework;
using System;
using ChunkCoord = Unity.Mathematics.int3;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Editor.ProTiler.UnitTests.Model
{
	public class LinearDataMapChunkTests
	{
		private static readonly ChunkSize s_TestChunkSize = new(4, 2, 4);

		[Test] public void DefaultCtor_WhenDataAccessed_IsNotCreated()
		{
			using (var chunk = new LinearDataMapChunk<TestData>())
				Assert.That(chunk.Data.IsCreated == false);
		}

		[Test] public void ChunkSizeCtor_WhenDataAccessed_IsCreated()
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
				Assert.That(chunk.Data.IsCreated);
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)]
		public void Indexer_WhenDataAdded_CollectionExpands(Int32 height)
		{
			var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize);
			var data = new TestData { value = 1 };
			var localCoord = new ChunkCoord(0, height, 0);

			chunk[localCoord] = data;

			var chunkDataLength = chunk.Data.Length;
			chunk.Dispose();
			Assert.That(chunkDataLength, Is.EqualTo(s_TestChunkSize.x * (height + 1) * s_TestChunkSize.z));
		}

		[TestCase(0)] [TestCase(1)] [TestCase(2)]
		public void Indexer_WhenDataRead_IsSameAsAddedData(Int32 height)
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
			{
				var data = new TestData { value = (Byte)(height + 1) };
				var localCoord = new ChunkCoord(0, height, 0);

				chunk.SetData(localCoord, data);
				var returnedData = chunk[localCoord];

				Assert.That(returnedData, Is.EqualTo(data));
			}
		}

		[TestCase(-1, -1, -1)] [TestCase(0, 0, 0)]
		public void Indexer_WhenCoordOutOfBounds_ThrowsIndexOutOfRangeException(Int32 x, Int32 y, Int32 z)
		{
			using (var chunk = new LinearDataMapChunk<TestData>(s_TestChunkSize))
			{
				var localCoord = new ChunkCoord(x, y, z);
				Assert.Throws<IndexOutOfRangeException>(() =>
				{
					var data = chunk[localCoord];
				});
			}
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
