// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Data
{
	[Serializable]
	public class Tilemap3DChunkCollection : Dictionary<long, Tilemap3DChunk> {}

	[Serializable]
	public class Tile3DCoordChunkCollection : Dictionary<long, IEnumerable<Tile3DCoord>>
	{
		public Tile3DCoordChunkCollection(IEnumerable<Tile3DCoord> tileCoords)
		{
			ToChunkedTileCoords(tileCoords);
		}

		private void ToChunkedTileCoords(IEnumerable<Tile3DCoord> tileCoords)
		{
			foreach (var tileCoord in tileCoords)
			{
				var coord = tileCoord.Coord;
				var chunkCoord = ToChunkCoord(coord);
				var chunk = GetOrCreateChunk(chunkCoord);
				chunk.SetTiles(tileCoords);
			}
		}
	}

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

		private static ChunkSize ClampChunkSize(ChunkSize chunkSize)
		{
			chunkSize.x = Mathf.Max(MinChunkSize.x, chunkSize.x);
			chunkSize.y = Mathf.Max(MinChunkSize.y, chunkSize.y);
			return chunkSize;
		}

		public Tilemap3D()
			: this(MinChunkSize) {}

		public Tilemap3D(ChunkSize chunkSize) => InitChunks(ClampChunkSize(chunkSize));

		private void InitChunks(ChunkSize chunkSize)
		{
			m_ChunkSize = chunkSize;
			m_Chunks = new Tilemap3DChunkCollection();
		}

		[ExcludeFromCodeCoverage]
		public override string ToString() => $"{nameof(Tilemap3D)}(ChunkCount: {ChunkCount}, ChunkSize: {ChunkSize})";

		public void SetTiles(IEnumerable<Tile3DCoord> tileCoords)
		{
			var chunkedTileCoords = ToChunkedTileCoords(tileCoords);
			foreach (var tileCoord in tileCoords)
			{
				var coord = tileCoord.Coord;
				var chunkCoord = Grid3DUtility.ToChunkCoord(coord, m_ChunkSize);
				var chunk = GetOrCreateChunk(chunkCoord);
				chunk.SetTiles(tileCoords);
			}
		}

		private Tile3DCoordChunkCollection ToChunkedTileCoords(IEnumerable<Tile3DCoord> tileCoords)
		{
			var chunkedTileCoords = new Tile3DCoordChunkCollection();
			foreach (var tileCoord in tileCoords)
			{
				var coord = tileCoord.Coord;
				var chunkCoord = Grid3DUtility.ToChunkCoord(coord, m_ChunkSize);
				var chunk = GetOrCreateChunk(chunkCoord);
				chunk.SetTiles(tileCoords);
			}
		}

		private Tilemap3DChunk GetOrCreateChunk(GridCoord chunkCoord)
		{
			var chunkKey = GetChunkKey(chunkCoord);
			if (TryGetChunk(chunkKey, out var chunk))
				return chunk;

			return CreateChunk(chunkKey);
		}

		private Tilemap3DChunk CreateChunk(long chunkKey) => m_Chunks[chunkKey] = new Tilemap3DChunk(m_ChunkSize);

		private bool TryGetChunk(long key, out Tilemap3DChunk chunk) => m_Chunks.TryGetValue(key, out chunk);

		private long GetChunkKey(GridCoord chunkCoord) => HashUtility.GetHash(chunkCoord.x, chunkCoord.z);
	}
}
