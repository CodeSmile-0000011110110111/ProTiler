// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Serialization;
using CodeSmile.Serialization;
using CodeSmile.Serialization.BinaryAdapters;
using System;
using System.Collections.Generic;
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

namespace CodeSmile.ProTiler.Model
{
	public class LinearDataMap<TData> : DataMapBase
		where TData : unmanaged, IBinarySerializable
	{
		private const Int32 MapAdapterVersion = 0;

		private NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>> m_Chunks;
		private List<IBinaryAdapter> m_BinaryAdapters;

		public NativeParallelHashMap<Int64, LinearDataMapChunk<TData>>.ReadOnly Chunks => m_Chunks.AsReadOnly();
		public NativeParallelHashMap<Int64, LinearDataMapChunk<TData>> GetWritableChunks() => m_Chunks;

		public LinearDataMap()
			: this(s_MinimumChunkSize) {}

		public LinearDataMap(ChunkSize chunkSize /*, IDataMapStream stream = null*/)
			: base(chunkSize /*, stream*/)
		{
			m_Chunks = new NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>>(0, Allocator.Domain);
		}

		internal LinearDataMap(ChunkSize chunkSize, NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>> chunks)
			: base(chunkSize) => m_Chunks = chunks;

		public override void Dispose()
		{
			foreach (var pair in m_Chunks)
				pair.Value.Dispose();
			m_Chunks.Dispose();
		}

		internal void AddChunk(ChunkCoord chunkCoord, LinearDataMapChunk<TData> chunk) =>
			AddChunk(ToChunkKey(chunkCoord), chunk);

		internal void AddChunk(ChunkKey key, LinearDataMapChunk<TData> chunk) => m_Chunks.Add(key, chunk);

		internal LinearDataMapChunk<TData> GetChunk(ChunkCoord chunkCoord) => GetChunk(ToChunkKey(chunkCoord));

		internal LinearDataMapChunk<TData> GetChunk(ChunkKey key) => m_Chunks[key];

		internal Boolean TryGetChunk(ChunkCoord chunkCoord, out LinearDataMapChunk<TData> chunk) =>
			TryGetChunk(ToChunkKey(chunkCoord), out chunk);

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
