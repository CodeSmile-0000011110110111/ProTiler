// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.v4.GridMap;
using System;
using Unity.Collections;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Model
{
	public class SparseDataMap<TData> : DataMapBase where TData : unmanaged
	{
		private NativeParallelHashMap<Int64, SparseDataMapChunk<TData>> m_Chunks;
		public NativeParallelHashMap<Int64, SparseDataMapChunk<TData>> Chunks => m_Chunks;

		public SparseDataMap() {}

		public SparseDataMap(IDataMapStream stream)
			: base(stream) {}

		public Boolean TryGetChunk(Int64 key, out SparseDataMapChunk<TData> chunk) => throw
			// try get from HashMap first
			//if (base.TryGetChunk(key, out chunk)) return true;
			// try get chunk from stream
			// may decide to dispose least recently used chunks
			new NotImplementedException();
	}
}
