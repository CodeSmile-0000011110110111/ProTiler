﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.Model._remove;
using CodeSmile.ProTiler.CodeDesign.v4.GridMap;
using System;
using Unity.Collections;
using ChunkKey = System.Int64;
using ChunkSize = Unity.Mathematics.int3;
using WorldCoord = Unity.Mathematics.int3;

namespace CodeSmile.ProTiler.CodeDesign.Model
{
	public class LinearDataMap<TData> : DataMapBase, IDisposable where TData : unmanaged
	{
		// TODO: hashmap of modified (unsaved) chunks?
		// possibly: hashmap of chunk access timestamps

		private NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>> m_Chunks;

		internal NativeParallelHashMap<Int64, LinearDataMapChunk<TData>>.ReadOnly Chunks => m_Chunks.AsReadOnly();

		public LinearDataMap()
			: this(s_MinChunkSize) {}

		public LinearDataMap(ChunkSize chunkSize, IDataMapStream stream = null)
			: base(chunkSize, stream) => m_Chunks = new NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>>
			(0, Allocator.Domain);

		public void Dispose() => m_Chunks.Dispose();

		internal void AddChunk(WorldCoord worldCoord, LinearDataMapChunk<TData> chunk) =>
			AddChunk(ToChunkKey(worldCoord), chunk);

		//public void AddChunk(ChunkCoord chunkCoord, LinearDataMapChunk<TData> chunk) => AddChunk(ToChunkKey(chunkCoord), chunk);
		internal void AddChunk(ChunkKey key, LinearDataMapChunk<TData> chunk) => m_Chunks.Add(key, chunk);

		internal Boolean TryGetChunk(WorldCoord worldCoord, out LinearDataMapChunk<TData> chunk) =>
			TryGetChunk(ToChunkKey(worldCoord), out chunk);

		internal Boolean TryGetChunk(ChunkKey key, out LinearDataMapChunk<TData> chunk)
		{
			// try get from HashMap first
			if (m_Chunks.TryGetValue(key, out chunk))
				return true;

			// try get chunk from stream here ...
			if (m_Stream != null) {}

			return false;
		}

		public override void Serialize(IBinaryWriter writer)
		{
			//writer.Add(..);
		}

		public override DataMapBase Deserialize(IBinaryReader reader, Byte userDataVersion) =>
			// deserialize base class fields first
			//baseField = reader.ReadNext<Byte>();
			this;
	}
}
