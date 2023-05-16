// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Tilemap.Tile;
using System.Collections.Generic;
using ChunkKey = System.Int64;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Tilemap.Chunk
{
	/// <summary>
	///     Container for Tile3DCoord collections that are automatically divided into chunks.
	/// </summary>
	internal class ChunkTileCoords : Dictionary<ChunkKey, IList<Tile3DCoord>>
	{
		public ChunkTileCoords(IEnumerable<Tile3DCoord> tileCoords, ChunkSize chunkSize) =>
			SplitTileCoordsIntoChunks(tileCoords, chunkSize);

		private void SplitTileCoordsIntoChunks(IEnumerable<Tile3DCoord> tileCoords, ChunkSize chunkSize)
		{
			foreach (var tileCoord in tileCoords)
			{
				var chunkCoord = tileCoord.GetChunkCoord(chunkSize);
				var chunkKey = Tilemap3DUtility.GetChunkKey(chunkCoord);
				AddTileCoordToChunk(chunkKey, tileCoord);
			}
		}

		private void AddTileCoordToChunk(ChunkKey chunkKey, in Tile3DCoord tileCoord)
		{
			if (TryGetValue(chunkKey, out var coords))
				coords.Add(tileCoord);
			else
				Add(chunkKey, new List<Tile3DCoord> { tileCoord });
		}
	}
}
