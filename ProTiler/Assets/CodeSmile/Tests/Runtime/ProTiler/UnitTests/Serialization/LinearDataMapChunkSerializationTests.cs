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
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Binary;
using UnityEngine;
using ChunkCoord = Unity.Mathematics.int3;
using ChunkSize = Unity.Mathematics.int3;
using Object = System.Object;

namespace CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization
{
	public class LinearDataMapChunkSerializationTests
	{
		private const Byte SerializationTestVersion = 7;
		private static readonly List<IBinaryAdapter> LinearDataMapChunkAdapter = new()
		{
			new LinearDataMapChunkBinaryAdapter<SerializationTestData>(SerializationTestVersion, Allocator.Domain),
		};

		private static IReadOnlyList<ChunkCoord> CreateChunkCoords(ChunkSize chunkSize)
		{
			var coords = new List<ChunkCoord>();

			for (var y = 0; y < chunkSize.y; y++)
			{
				for (var z = 0; z < chunkSize.z; z++)
				{
					for (var x = 0; x < chunkSize.x; x++)
						coords.Add(new ChunkCoord(x, y, z));
				}
			}

			return coords;
		}

		private static LinearDataMapChunk<SerializationTestData> CreateChunkFilledWithData(ChunkCoord chunkSize)
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

		[TestCase(1, 1, 1)] [TestCase(2, 2, 2)] [TestCase(4, 2, 7)]
		public void ChunkWithData_WhenSerialized_HasExpectedLength(Int32 x, Int32 y, Int32 z)
		{
			var chunkSize = new ChunkSize(x, y, z);
			using (var chunk = CreateChunkFilledWithData(chunkSize))
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
			using (var chunk = CreateChunkFilledWithData(chunkSize))
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
			using (var chunk = CreateChunkFilledWithData(chunkSize))
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
			using (var chunk = CreateChunkFilledWithData(chunkSize))
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

		public class SerializationTestDataBinaryAdapter : VersionedBinaryAdapterBase,
			IBinaryAdapter<SerializationTestData>
		{
			public SerializationTestDataBinaryAdapter(Byte adapterVersion)
				: base(adapterVersion) {}

			public unsafe void Serialize(in BinarySerializationContext<SerializationTestData> context,
				SerializationTestData data) => data.Serialize(context.Writer);

			public unsafe SerializationTestData Deserialize(
				in BinaryDeserializationContext<SerializationTestData> context)
			{
				var data = new SerializationTestData();
				data.Deserialize(context.Reader, AdapterVersion);
				return data;
			}
		}

		public struct SerializationTestData : IEquatable<SerializationTestData>, IBinarySerializable
		{
			public ChunkCoord Coord;
			public UInt16 Index;

			public unsafe void Serialize(UnsafeAppendBuffer* writer)
			{
				writer->Add(Coord);
				writer->Add(Index);
			}

			public unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte dataVersion)
			{
				switch (dataVersion)
				{
					case 0:
						Coord = reader->ReadNext<ChunkCoord>();
						Index = reader->ReadNext<UInt16>();
						break;
					default:
						throw new SerializationVersionException($"unhandled version {dataVersion}");
				}
			}

			public static Boolean operator ==(SerializationTestData left, SerializationTestData right) =>
				left.Equals(right);

			public static Boolean operator !=(SerializationTestData left, SerializationTestData right) =>
				!left.Equals(right);

			public Boolean Equals(SerializationTestData other) => Coord.Equals(other.Coord) && Index == other.Index;
			public override String ToString() => $"{nameof(SerializationTestData)}({Index}, {Coord})";
			public override Boolean Equals(Object obj) => obj is SerializationTestData other && Equals(other);
			public override Int32 GetHashCode() => HashCode.Combine(Coord, Index);
		}
	}
}
