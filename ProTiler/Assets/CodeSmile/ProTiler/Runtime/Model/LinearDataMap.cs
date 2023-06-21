// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using ChunkKey = System.Int64;
using ChunkSize = Unity.Mathematics.int3;
using WorldCoord = Unity.Mathematics.int3;

namespace CodeSmile.ProTiler.Model
{
	public class LinearDataMap<TData> : DataMapBase, IDisposable where TData : unmanaged
	{
		// TODO: hashmap of modified (unsaved) chunks?
		// possibly: hashmap of chunk access timestamps

		private NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>> m_Chunks;

		public NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>>.ReadOnly Chunks => m_Chunks.AsReadOnly();

		public LinearDataMap()
			: this(s_MinimumChunkSize) {}

		public LinearDataMap(ChunkSize chunkSize /*, IDataMapStream stream = null*/)
			: base(chunkSize /*, stream*/) => m_Chunks = new NativeParallelHashMap<ChunkKey, LinearDataMapChunk<TData>>
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
			// if (m_Stream != null) {}

			return false;
		}

		public override unsafe void Serialize(UnsafeAppendBuffer* writer)
		{

		}

		public override unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte serializedDataVersion,
			Byte currentDataVersion)
		{
			throw new NotImplementedException();
		}
	}
}
