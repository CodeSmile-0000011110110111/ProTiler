// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Properties;

namespace CodeSmile.ProTiler.Data
{
	[Serializable]
	internal class Tilemap3DChunks
	{
		[CreateProperty] private Dictionary<long, Tilemap3DChunk> m_Chunks = new();
		public Tilemap3DChunk this[long chunkKey] { get => m_Chunks[chunkKey]; set => m_Chunks[chunkKey] = value; }

		internal int TileCount
		{
			get
			{
				var tileCount = 0;
				foreach (var chunk in m_Chunks.Values)
					tileCount += chunk.TileCount;

				return tileCount;
			}
		}
		public int Count => m_Chunks.Count;

		public bool TryGetValue(long key, out Tilemap3DChunk chunk) => m_Chunks.TryGetValue(key, out chunk);
	}
}