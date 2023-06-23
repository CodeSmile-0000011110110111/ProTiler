// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Model;
using CodeSmile.Serialization;
using System;
using System.Collections.Generic;
using Unity.Serialization.Binary;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using LocalCoord = Unity.Mathematics.int3;
using LocalPos = Unity.Mathematics.float3;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.ProTiler.Serialization
{
	public sealed class GridMapBinaryAdapter<TGridMap> : VersionedBinaryAdapterBase, IBinaryAdapter<TGridMap>
		where TGridMap : GridMapBase, new()
	{
		public GridMapBinaryAdapter(Byte version)
			: base(version) {}

		public unsafe void Serialize(in BinarySerializationContext<TGridMap> context, TGridMap gridMap)
		{
			var writer = context.Writer;
			WriteAdapterVersion(writer);

			writer->Add(gridMap.ChunkSize);
			context.SerializeValue(gridMap.GetLinearMaps());
			context.SerializeValue(gridMap.GetSparseMaps());
			// SerializeMaps(context, gridMap.LinearMaps);
			// SerializeMaps(context, gridMap.SparseMaps);

			// intended mainly for subclasses
			gridMap.Serialize(context);
		}

		private unsafe void SerializeMaps(BinarySerializationContext<TGridMap> context, IReadOnlyList<DataMapBase> maps)
		{
			var itemCount = maps.Count;
			context.Writer->Add(itemCount);
			for (var i = 0; i < itemCount; i++)
				context.SerializeValue(maps[i]);
		}

		public unsafe TGridMap Deserialize(in BinaryDeserializationContext<TGridMap> context)
		{
			var reader = context.Reader;
			ReadAdapterVersion(reader);

			var gridMap = new TGridMap();
			gridMap.ChunkSize = reader->ReadNext<ChunkSize>();
			gridMap.SetLinearMaps(context.DeserializeValue<List<DataMapBase>>());
			gridMap.SetSparseMaps(context.DeserializeValue<List<DataMapBase>>());

			// gridMap.LinearMaps = DeserializeMaps(context);

			return gridMap.Deserialize(context, AdapterVersion) as TGridMap;
		}

		private unsafe IReadOnlyList<DataMapBase> DeserializeMaps(BinaryDeserializationContext<TGridMap> context)
		{
			var list = new List<DataMapBase>();

			var itemCount = context.Reader->ReadNext<Int32>();
			for (var i = 0; i < itemCount; i++)
				list.Add(context.DeserializeValue<DataMapBase>());

			return list.AsReadOnly();
		}
	}
}
