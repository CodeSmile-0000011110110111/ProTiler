// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions.NativeCollections;
using CodeSmile.ProTiler.Model;
using CodeSmile.Serialization;
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Binary;
using ChunkSize = Unity.Mathematics.int3;

namespace CodeSmile.ProTiler.Serialization
{
	public class LinearDataMapChunkBinaryAdapter<TData> : VersionedBinaryAdapterBase,
		IBinaryAdapter<LinearDataMapChunk<TData>>
		where TData : unmanaged, IBinarySerializable
	{
		private readonly Byte m_DataVersion;
		private readonly Allocator m_Allocator;

		private static unsafe void WriteChunkData(
			in BinarySerializationContext<LinearDataMapChunk<TData>> context, in UnsafeList<TData>.ReadOnly dataList)
		{
			var writer = context.Writer;
			var dataLength = dataList.Length;
			writer->Add(dataLength);

			foreach (var data in dataList)
				data.Serialize(writer);
		}

		private static unsafe UnsafeList<TData> ReadChunkData(
			in BinaryDeserializationContext<LinearDataMapChunk<TData>> context, Byte serializedDataVersion,
			Allocator allocator)
		{
			var reader = context.Reader;
			var dataLength = reader->ReadNext<Int32>();

			var list = UnsafeListExt.NewWithLength<TData>(dataLength, allocator);
			for (var i = 0; i < dataLength; i++)
			{
				var data = new TData();
				// TODO: speed up method call, avoid interface boxing
				data.Deserialize(reader, serializedDataVersion);
				list[i] = data;
			}

			return list;
		}

		public LinearDataMapChunkBinaryAdapter(Byte adapterVersion, Byte dataVersion, Allocator allocator)
			: base(adapterVersion)
		{
			m_DataVersion = dataVersion;
			m_Allocator = allocator;
		}

		public unsafe void Serialize(in BinarySerializationContext<LinearDataMapChunk<TData>> context,
			LinearDataMapChunk<TData> chunk)
		{
			var writer = context.Writer;

			WriteAdapterVersion(writer);
			writer->Add(m_DataVersion);
			writer->Add(chunk.Size);
			WriteChunkData(context, chunk.Data);
		}

		public unsafe LinearDataMapChunk<TData> Deserialize(
			in BinaryDeserializationContext<LinearDataMapChunk<TData>> context)
		{
			var reader = context.Reader;

			var serializedAdapterVersion = ReadAdapterVersion(reader);
			if (serializedAdapterVersion == AdapterVersion)
			{
				var serializedDataVersion = reader->ReadNext<Byte>();
				var chunkSize = reader->ReadNext<ChunkSize>();
				var data = ReadChunkData(context, serializedDataVersion, m_Allocator);

				return new LinearDataMapChunk<TData>(chunkSize, data);
			}

			throw new SerializationVersionException(GetVersionExceptionMessage(serializedAdapterVersion));
		}
	}
}
