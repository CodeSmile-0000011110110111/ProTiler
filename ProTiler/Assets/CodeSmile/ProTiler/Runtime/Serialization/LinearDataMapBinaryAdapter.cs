// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.Model;
using CodeSmile.Serialization;
using System;
using Unity.Serialization.Binary;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using LocalCoord = Unity.Mathematics.int3;
using LocalPos = Unity.Mathematics.float3;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.ProTiler.CodeDesign.Serialization
{
	public class LinearDataMapBinaryAdapter<TData> : VersionedBinaryAdapterBase, IBinaryAdapter<LinearDataMap<TData>>
	where TData : unmanaged
	{
		public Byte DataVersion { get; }

		public LinearDataMapBinaryAdapter(Byte adapterVersion, Byte dataVersion)
			: base(adapterVersion)
		{
			DataVersion = dataVersion;
		}

		public unsafe void Serialize(in BinarySerializationContext<LinearDataMap<TData>> context, LinearDataMap<TData> map)
		{
			var writer = context.Writer;
			WriteAdapterVersion(writer);
			writer->Add(map.ChunkSize);

			writer->Add(DataVersion);
			map.Serialize(context.Writer);
		}

		public unsafe LinearDataMap<TData> Deserialize(in BinaryDeserializationContext<LinearDataMap<TData>> context)
		{
			var reader = context.Reader;
			var serializedAdapterVersion = ReadAdapterVersion(reader);
			if (serializedAdapterVersion <= AdapterVersion)
			{
				var chunkSize = reader->ReadNext<ChunkSize>();
				var map = new LinearDataMap<TData>(chunkSize);

				var serializedDataVersion = reader->ReadNext<Byte>();
				map.Deserialize(reader, serializedDataVersion, DataVersion);
				return map;
			}

			throw new SerializationVersionException(GetVersionExceptionMessage(serializedAdapterVersion));
		}
	}
}
