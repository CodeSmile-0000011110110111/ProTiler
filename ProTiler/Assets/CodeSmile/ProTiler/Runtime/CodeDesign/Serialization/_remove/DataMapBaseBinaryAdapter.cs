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
	// TODO: remove this, it will not be needed
	/*public sealed class DataMapBaseBinaryAdapter<TDataMap> : VersionedBinaryAdapterBase, IBinaryAdapter<TDataMap>
		where TDataMap : DataMapBase, new()
	{
		public DataMapBaseBinaryAdapter(Byte adapterVersion)
			: base(adapterVersion) {}

		public unsafe void Serialize(in BinarySerializationContext<TDataMap> context, TDataMap map)
		{
			var writer = context.Writer;
			WriteAdapterVersion(writer);
			writer->Add(map.ChunkSize);
			map.Serialize(context.Writer);
		}

		public unsafe TDataMap Deserialize(in BinaryDeserializationContext<TDataMap> context)
		{
			var reader = context.Reader;
			var serializedVersion = ReadAdapterVersion(reader);
			if (serializedVersion <= AdapterVersion)
			{
				var chunkSize = reader->ReadNext<ChunkSize>();
				var map = new TDataMap();
				map.ChunkSize = chunkSize;
				map.Deserialize(reader, serializedVersion, AdapterVersion);
				return map;
			}

			throw new SerializationVersionException(GetVersionExceptionMessage(serializedVersion));
		}
	}*/
}
