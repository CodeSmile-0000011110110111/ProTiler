// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using System;
using System.Collections.Generic;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Serialization.Unity.Serialization
{
	public class TilemapChunkBufferedFileStream<TChunk, TData> : TilemapChunkMemoryStream<TChunk, TData>
		where TChunk : unmanaged, IChunkData<TData>
		where TData : unmanaged
	{
		private Dictionary<Int64, byte[]> m_SerializedChunks;

		// serialize to byte[]
		public void SerializeChunk<TChunk>(Int64 key, TChunk chunk) => throw new NotImplementedException();
		// deserialize from byte[]
		public TChunk DeserializeChunk<TChunk>(Int64 key) => throw new NotImplementedException();
	}

	public class TilemapChunkMemoryStream<TChunk, TData> : ITilemapChunkDataSerialization<TChunk, TData>
		where TChunk : unmanaged, IChunkData<TData>
		where TData : unmanaged
	{
		private Dictionary<Int64, byte[]> m_SerializedChunks;

		// serialize to byte[]
		public void SerializeChunk<TChunk>(Int64 key, TChunk chunk) => throw new NotImplementedException();

		// deserialize from byte[]
		public TChunk DeserializeChunk<TChunk>(Int64 key) => throw new NotImplementedException();
	}
}
