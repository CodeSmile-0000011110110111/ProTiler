// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Model;
using CodeSmile.ProTiler.Serialization;
using CodeSmile.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Serialization.Binary;
using UnityEngine;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using LocalCoord = Unity.Mathematics.int3;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization
{
	public class LinearDataMapChunkSerializationTests
	{
		private const Byte TestAdapterVersion = 7;
		private static readonly List<IBinaryAdapter> LinearDataMapChunkAdapter = new()
		{
			new LinearDataMapChunkBinaryAdapter<SerializationTestData>(
				TestAdapterVersion, SerializationTestData.DataVersion, Allocator.Domain),
		};

		[TestCase(1, 1, 1)] [TestCase(2, 2, 2)] [TestCase(4, 2, 7)]
		public void ChunkWithData_WhenSerialized_HasExpectedLength(Int32 x, Int32 y, Int32 z)
		{
			var chunkSize = new ChunkSize(x, y, z);
			using (var chunk = DataCreation.CreateChunkFilledWithData(chunkSize))
			{
				var bytes = Serialize.ToBinary(chunk, LinearDataMapChunkAdapter);
				Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");

				unsafe
				{
					var versionLength = sizeof(Byte);
					var dataVersionLength = sizeof(Byte);
					var chunkSizeLength = sizeof(ChunkSize);
					var listLength = sizeof(Int32);
					var chunkGridLength = chunkSize.x * chunkSize.y * chunkSize.z;
					var dataLength = sizeof(ChunkSize) + sizeof(UInt16);
					var expectedLength = versionLength + dataVersionLength + chunkSizeLength + listLength +
					                     chunkGridLength * dataLength;
					Assert.That(bytes.Length, Is.EqualTo(expectedLength));
				}
			}
		}

		[TestCase(1, 1, 1)] [TestCase(2, 2, 2)] [TestCase(3, 4, 5)] [TestCase(4, 2, 7)]
		public void ChunkWithData_WhenSerialized_CanDeserialize(Int32 x, Int32 y, Int32 z)
		{
			var chunkSize = new ChunkSize(x, y, z);
			using (var chunk = DataCreation.CreateChunkFilledWithData(chunkSize))
			{
				var bytes = Serialize.ToBinary(chunk, LinearDataMapChunkAdapter);
				Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
				var deserializedChunk = Serialize.FromBinary<LinearDataMapChunk<SerializationTestData>>(
					bytes, LinearDataMapChunkAdapter);

				Assert.That(deserializedChunk.Size, Is.EqualTo(chunk.Size));
				Assert.That(deserializedChunk.Data.Length, Is.EqualTo(chunk.Data.Length));
				using (var deserializedData = deserializedChunk.GetWritableData())
				{
					var chunkData = chunk.GetWritableData();
					for (var i = 0; i < deserializedData.Length; i++)
						Assert.That(deserializedData[i], Is.EqualTo(chunkData[i]));
				}
			}
		}

		[TestCase(1)] [TestCase(-1)]
		public void Deserialize_WithUnsupportedVersion_ThrowsSerializationVersionException(Int32 versionOffset)
		{
			var chunkSize = new ChunkSize(2, 1, 2);
			using (var chunk = DataCreation.CreateChunkFilledWithData(chunkSize))
			{
				var bytes = Serialize.ToBinary(chunk, LinearDataMapChunkAdapter);
				bytes[0] += (Byte)versionOffset; // bump version
				Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");

				Assert.Throws<SerializationVersionException>(() =>
				{
					Serialize.FromBinary<LinearDataMapChunk<SerializationTestData>>(bytes, LinearDataMapChunkAdapter);
				});
			}
		}

		[Test] public void Deserialize_WhenLoadingPreviousVersion_DataCanBeDeserialized()
		{
			var data0 = new DataVersionOld
			{
				RemainsUnchanged0 = 0xff,
				WillChangeTypeInVersion1 = 8,
				RemainsUnchanged1 = 0xff,
				WillBeRemovedInVersion1 = 9,
				RemainsUnchanged2 = 0xff,
			};

			using (var chunk = new LinearDataMapChunk<DataVersionOld>(new ChunkSize(1, 1, 1)))
			{
				chunk.SetData(LocalCoord.zero, data0);

				var adapterVersion0 = new List<IBinaryAdapter>
				{
					new LinearDataMapChunkBinaryAdapter<DataVersionOld>(TestAdapterVersion, 0, Allocator.Domain),
				};
				var bytes = Serialize.ToBinary(chunk, adapterVersion0);
				Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");

				var adapterVersion1 = new List<IBinaryAdapter>
				{
					new LinearDataMapChunkBinaryAdapter<DataVersionCurrent>(TestAdapterVersion, 1, Allocator.Domain),
				};
				using (var deserializedChunk =
				       Serialize.FromBinary<LinearDataMapChunk<DataVersionCurrent>>(bytes, adapterVersion1))
				{
					var data1 = deserializedChunk.GetWritableData()[0];

					Assert.That(data1.WillChangeTypeInVersion1, Is.EqualTo((Int64)data0.WillChangeTypeInVersion1));
					Assert.That(data1.NewFieldWithNonDefaultValue, Is.EqualTo(DataVersionCurrent.NewFieldInitialValue));
					Assert.That(data1.RemainsUnchanged0, Is.EqualTo(data0.RemainsUnchanged0));
					Assert.That(data1.RemainsUnchanged1, Is.EqualTo(data0.RemainsUnchanged1));
					Assert.That(data1.RemainsUnchanged2, Is.EqualTo(data0.RemainsUnchanged2));

					// see if we can serialize v1 correctly
					var bytes2 = Serialize.ToBinary(deserializedChunk, adapterVersion1);
					Debug.Log($"{bytes2.Length} Bytes: {bytes2.AsString()}");
					using (var deserializedChunkAgain =
					       Serialize.FromBinary<LinearDataMapChunk<DataVersionCurrent>>(bytes, adapterVersion1))
						Assert.That(deserializedChunkAgain.GetWritableData()[0], Is.EqualTo(data1));
				}
			}
		}
	}
}
