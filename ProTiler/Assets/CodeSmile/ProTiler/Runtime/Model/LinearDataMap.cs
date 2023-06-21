﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Serialization;
using CodeSmile.Serialization;
using CodeSmile.Serialization.BinaryAdapters;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Serialization.Binary;
using ChunkKey = System.Int64;
using ChunkSize = Unity.Mathematics.int3;
using WorldCoord = Unity.Mathematics.int3;

namespace CodeSmile.ProTiler.Model
{
	public class LinearDataMap<TData> : DataMapBase, IDisposable
		where TData : unmanaged, IBinarySerializable
	{
		private const Int32 MapAdapterVersion = 0;

		// TODO: hashmap of modified (unsaved) chunks?
		// possibly: hashmap of chunk access timestamps

		private NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>> m_Chunks;

		public NativeParallelHashMap<Int64, LinearDataMapChunk<TData>>.ReadOnly Chunks => m_Chunks.AsReadOnly();
		public NativeParallelHashMap<Int64, LinearDataMapChunk<TData>> GetWritableChunks() => m_Chunks;

		public LinearDataMap()
			: this(s_MinimumChunkSize) {}

		public LinearDataMap(ChunkSize chunkSize /*, IDataMapStream stream = null*/)
			: base(chunkSize /*, stream*/) => m_Chunks = new NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>>
			(0, Allocator.Domain);

		internal LinearDataMap(ChunkSize chunkSize, NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>> chunks)
			: base(chunkSize) => m_Chunks = chunks;

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
			// if (m_Stream != null) {}

			return false;
		}

		public static List<IBinaryAdapter> GetBinaryAdapters(Byte dataAdapterVersion)
		{
			var adapters = LinearDataMapChunk<TData>.GetBinaryAdapters(dataAdapterVersion);
			adapters.Add(new NativeParallelHashMapBinaryAdapter<ChunkKey, LinearDataMapChunk<TData>>(Allocator.Domain));
			adapters.Add(new LinearDataMapBinaryAdapter<TData>(MapAdapterVersion));
			return adapters;
		}
	}
}
