// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using ChunkSize = Unity.Mathematics.int3;

namespace CodeSmile.ProTiler.CodeDesign.Model
{
	public class SparseDataMap<TData> : DataMapBase where TData : unmanaged
	{
		private NativeParallelHashMap<Int64, SparseDataMapChunk<TData>> m_Chunks;
		public NativeParallelHashMap<Int64, SparseDataMapChunk<TData>> Chunks => m_Chunks;

		public SparseDataMap() {}

		public SparseDataMap(ChunkSize chunkSize /*, IDataMapStream stream*/)
			: base(chunkSize /*, stream*/) {}

		public Boolean TryGetChunk(Int64 key, out SparseDataMapChunk<TData> chunk) => throw
			// try get from HashMap first
			//if (base.TryGetChunk(key, out chunk)) return true;
			// try get chunk from stream
			// may decide to dispose least recently used chunks
			new NotImplementedException();

		public override unsafe void Serialize(UnsafeAppendBuffer* writer) => throw new NotImplementedException();

		public override unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte serializedDataVersion,
			Byte currentDataVersion) => throw new NotImplementedException();
	}
}
