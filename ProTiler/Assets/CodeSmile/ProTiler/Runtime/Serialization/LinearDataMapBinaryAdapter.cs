// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Model;
using CodeSmile.Serialization;
using System;
using Unity.Collections;
using Unity.Serialization.Binary;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkKey = System.Int64;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using LocalCoord = Unity.Mathematics.int3;
using LocalPos = Unity.Mathematics.float3;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.ProTiler.Serialization
{
	public class LinearDataMapBinaryAdapter<TData> : VersionedBinaryAdapterBase, IBinaryAdapter<LinearDataMap<TData>>
		where TData : unmanaged, IBinarySerializable
	{
		public LinearDataMapBinaryAdapter(Byte adapterVersion)
			: base(adapterVersion) {}

		public unsafe void Serialize(in BinarySerializationContext<LinearDataMap<TData>> context,
			LinearDataMap<TData> map)
		{
			var writer = context.Writer;
			WriteAdapterVersion(writer);
			writer->Add(map.ChunkSize);

			context.SerializeValue(map.GetWritableChunks());
		}

		public unsafe LinearDataMap<TData> Deserialize(in BinaryDeserializationContext<LinearDataMap<TData>> context)
		{
			var reader = context.Reader;
			var serializedAdapterVersion = ReadAdapterVersion(reader);
			if (serializedAdapterVersion == AdapterVersion)
			{
				var chunkSize = reader->ReadNext<ChunkSize>();
				var chunks = context.DeserializeValue<NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>>>();
				return new LinearDataMap<TData>(chunkSize, chunks);
			}

			throw new SerializationVersionException(GetVersionExceptionMessage(serializedAdapterVersion));
		}
	}
}
