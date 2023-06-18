// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Core.Serialization;
using CodeSmile.Extensions;
using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using CodeSmile.ProTiler.Runtime.CodeDesign.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Serialization.Binary;
using UnityEngine;
using ChunkCoord = Unity.Mathematics.int3;
using ChunkSize = Unity.Mathematics.int3;

namespace CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization
{
	public class LinearDataMapChunkSerializationTests
	{
		private const Byte SerializationTestVersion = 7;
		private static readonly List<IBinaryAdapter> LinearDataMapChunkAdapter = new()
		{
			new LinearDataMapChunkBinaryAdapter<SerializationTestData>(SerializationTestVersion, Allocator.Domain),
		};

		[TestCase(1, 1, 1)] [TestCase(2, 2, 2)] [TestCase(4, 2, 7)]
		public void ChunkWithData_WhenSerialized_HasExpectedLength(Int32 x, Int32 y, Int32 z)
		{
			var chunkSize = new ChunkSize(x, y, z);
			using (var chunk = ChunkDataHelper.CreateChunkFilledWithData(chunkSize))
			{
				var bytes = Serialize.ToBinary(chunk, LinearDataMapChunkAdapter);
				Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");

				unsafe
				{
					var versionLength = sizeof(Byte);
					var chunkSizeLength = sizeof(ChunkSize);
					var listLength = sizeof(Int32);
					var chunkGridLength = chunkSize.x * chunkSize.y * chunkSize.z;
					var dataLength = sizeof(ChunkSize) + sizeof(UInt16);
					var expectedLength = versionLength + chunkSizeLength + listLength + chunkGridLength * dataLength;
					Assert.That(bytes.Length, Is.EqualTo(expectedLength));
				}
			}
		}

		[TestCase(1, 1, 1)] [TestCase(2, 2, 2)] [TestCase(3, 4, 5)] [TestCase(4, 2, 7)]
		public void ChunkWithData_WhenSerialized_CanDeserialize(Int32 x, Int32 y, Int32 z)
		{
			var chunkSize = new ChunkSize(x, y, z);
			using (var chunk = ChunkDataHelper.CreateChunkFilledWithData(chunkSize))
			{
				var bytes = Serialize.ToBinary(chunk, LinearDataMapChunkAdapter);
				Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
				var deserializedChunk = Serialize.FromBinary<LinearDataMapChunk<SerializationTestData>>(
					bytes, LinearDataMapChunkAdapter);

				Assert.That(deserializedChunk, Is.EqualTo(chunk));
				var deserializedData = deserializedChunk.GetWritableData();
				var chunkData = chunk.GetWritableData();
				for (var i = 0; i < deserializedData.Length; i++)
					Assert.That(deserializedData[i], Is.EqualTo(chunkData[i]));
			}
		}

		[TestCase(1, 1, 1)] [TestCase(2, 2, 2)] [TestCase(3, 4, 5)] [TestCase(4, 2, 7)]
		public void ChunkWithData_WhenSerializedWithDataAdapter_CanDeserialize(Int32 x, Int32 y, Int32 z)
		{
			var chunkSize = new ChunkSize(x, y, z);
			using (var chunk = ChunkDataHelper.CreateChunkFilledWithData(chunkSize))
			{
				var adapters = new List<IBinaryAdapter>(LinearDataMapChunkAdapter);
				adapters.Add(new SerializationTestDataBinaryAdapter(0));

				var bytes = Serialize.ToBinary(chunk, adapters);
				Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
				var deserializedChunk = Serialize.FromBinary<LinearDataMapChunk<SerializationTestData>>(
					bytes, LinearDataMapChunkAdapter);

				Assert.That(deserializedChunk, Is.EqualTo(chunk));
				var deserializedData = deserializedChunk.GetWritableData();
				var chunkData = chunk.GetWritableData();
				for (var i = 0; i < deserializedData.Length; i++)
					Assert.That(deserializedData[i], Is.EqualTo(chunkData[i]));
			}
		}

		[TestCase(1, 1, 1)] [TestCase(2, 2, 2)] [TestCase(3, 4, 5)] [TestCase(4, 2, 7)]
		public void ChunkWithData_WhenSerializedWithInterfaceDataAdapter_CanDeserialize(Int32 x, Int32 y, Int32 z)
		{
			var chunkSize = new ChunkSize(x, y, z);
			using (var chunk = ChunkDataHelper.CreateChunkFilledWithData(chunkSize))
			{
				var adapters = new List<IBinaryAdapter>(LinearDataMapChunkAdapter);
				adapters.Add(new BinarySerializableInterfaceAdapter<SerializationTestData>(0));

				var bytes = Serialize.ToBinary(chunk, adapters);
				Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
				var deserializedChunk = Serialize.FromBinary<LinearDataMapChunk<SerializationTestData>>(
					bytes, LinearDataMapChunkAdapter);

				Assert.That(deserializedChunk, Is.EqualTo(chunk));
				var deserializedData = deserializedChunk.GetWritableData();
				var chunkData = chunk.GetWritableData();
				for (var i = 0; i < deserializedData.Length; i++)
					Assert.That(deserializedData[i], Is.EqualTo(chunkData[i]));
			}
		}
	}
}
