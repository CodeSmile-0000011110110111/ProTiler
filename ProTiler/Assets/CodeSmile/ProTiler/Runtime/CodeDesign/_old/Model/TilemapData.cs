// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections;
using Unity.Properties;
using ChunkCoord = Unity.Mathematics.int2;

namespace CodeSmile.ProTiler.Runtime.CodeDesign._old.Model
{
	public abstract class TilemapDataBase : ITilemapData
	{
		// coord to chunk key
		// hashmap of modified (unsaved) chunks
		// possibly: hashmap of loaded chunks together with access timestamp

		// create instance of undo/redo system (editor and runtime edit-mode)
	}

	public class TilemapDataLinear<TData> : TilemapDataBase, ITilemapDataLinear
		where TData : unmanaged
	{
		// this ref comes from outside, new here is just for testing
		private readonly ITilemapChunkDataSerialization<ChunkDataLinear<TData>, TData> m_Stream;

		[CreateProperty] public NativeParallelHashMap<Int64, ChunkDataLinear<TData>> linearChunks;

		public TilemapDataLinear(ITilemapChunkDataSerialization<ChunkDataLinear<TData>, TData> stream) =>
			m_Stream = stream;

		public void SerializeChunk(ChunkCoord chunkCoord)
		{
			var chunk = new ChunkDataLinear<TData>();
			m_Stream?.SerializeChunk(0, chunk);
		}

		public void DeserializeChunk(ChunkCoord chunkCoord)
		{
			if (m_Stream != null)
				m_Stream.DeserializeChunk<ChunkDataLinear<TData>>(0);
		}

		public void UnloadChunk(Int64 key)
		{
			// simply delete that chunk from hashmap
		}
	}

	// TODO:
	// isn't TilemapDataSparse the same as linear? do i still need both due to not being able to use interfaces? (i think so)

	public class TilemapDataSparse<TData> : TilemapDataBase, ITilemapDataSparse where TData : unmanaged
	{
		[CreateProperty] public NativeParallelHashMap<Int64, ChunkDataSparse<TData>> sparseChunks;
		// public override void SerializeChunk(Int64 key, T chunk) => throw new NotImplementedException();
		// public override void DeserializeChunk(Int64 key, out Object chunk) => throw new NotImplementedException();
	}
}
