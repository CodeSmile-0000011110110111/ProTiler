﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.v4.GridMap;
using System;
using Unity.Collections;
using Unity.Properties;

namespace CodeSmile.ProTiler.CodeDesign.v4.DataMaps
{
	public class LinearDataMap<TData> : DataMapBase where TData : unmanaged
	{
		private NativeParallelHashMap<Int64, LinearDataMapChunk<TData>> m_Chunks;
		public NativeParallelHashMap<Int64, LinearDataMapChunk<TData>> Chunks => m_Chunks;

		public LinearDataMap() {}

		public LinearDataMap(IDataMapStream stream)
			: base(stream) {}

		public Boolean TryGetChunk(Int64 key, out LinearDataMapChunk<TData> chunk) => throw
			// try get from HashMap first
			//if (base.TryGetChunk(key, out chunk)) return true;
			// try get chunk from stream
			// may decide to dispose least recently used chunks
			new NotImplementedException();

		public override void Serialize(IBinaryWriter writer)
		{
			//writer.Add(..);
		}

		public override DataMapBase Deserialize(IBinaryReader reader, byte userDataVersion) =>
			// deserialize base class fields first
			//baseField = reader.ReadNext<Byte>();
			this;
	}
}