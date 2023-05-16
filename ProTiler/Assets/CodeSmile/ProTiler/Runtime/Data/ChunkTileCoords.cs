// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     Container for Tile3DCoord collections that are automatically divided into chunks.
	/// </summary>
	internal class ChunkTileCoords : Dictionary<long, IList<Tile3DCoord>>
	{
		public ChunkTileCoords(IEnumerable<Tile3DCoord> tileCoords, Vector2Int chunkSize) =>
			SplitTileCoordsIntoChunks(tileCoords, chunkSize);

		private void SplitTileCoordsIntoChunks(IEnumerable<Tile3DCoord> tileCoords, Vector2Int chunkSize)
		{
			foreach (var tileCoord in tileCoords)
			{
				var chunkCoord = tileCoord.GetChunkCoord(chunkSize);
				var chunkKey = Tilemap3DUtility.GetChunkKey(chunkCoord);
				AddTileCoordToChunk(chunkKey, tileCoord);
			}
		}

		private void AddTileCoordToChunk(long chunkKey, in Tile3DCoord tileCoord)
		{
			if (TryGetValue(chunkKey, out var coords))
				coords.Add(tileCoord);
			else
				Add(chunkKey, new List<Tile3DCoord> { tileCoord });
		}
	}
}