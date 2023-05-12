// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEngine;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Data
{
	[Serializable]
	public class Tilemap3DChunkCollection : Dictionary<long, Tilemap3DChunk> {}

	[Serializable]
	public class Tilemap3D
	{
		/// <summary>
		///     This is a required technical limitation to have at least 2x2 tiles per chunk.
		///     The hashes of chunks with size of less than 2x2 would not be unique.
		/// </summary>
		internal static readonly ChunkSize MinChunkSize = new(2, 2);

		private Tilemap3DChunkCollection m_Chunks;
		private ChunkSize m_ChunkSize;
		internal ChunkSize ChunkSize => m_ChunkSize;

		public int ChunkCount => m_Chunks.Count;

		public static ChunkSize ClampChunkSize(ChunkSize chunkSize)
		{
			chunkSize.x = Mathf.Max(MinChunkSize.x, chunkSize.x);
			chunkSize.y = Mathf.Max(MinChunkSize.y, chunkSize.y);
			return chunkSize;
		}

		public Tilemap3D()
			: this(MinChunkSize) {}

		public Tilemap3D(ChunkSize chunkSize) => InitChunks(ClampChunkSize(chunkSize));

		public void InitChunks(ChunkSize chunkSize)
		{
			m_ChunkSize = chunkSize;
			m_Chunks = new Tilemap3DChunkCollection();
		}
	}
}
