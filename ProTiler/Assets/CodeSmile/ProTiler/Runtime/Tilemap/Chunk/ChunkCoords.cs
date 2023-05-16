// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using ChunkKey = System.Int64;
using ChunkSize = UnityEngine.Vector2Int;
using ChunkCoord = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;
using LayerCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Tilemap.Chunk
{
	internal class ChunkCoords : Dictionary<ChunkKey, IList<LayerCoord>>
	{
		private readonly Dictionary<ChunkKey, ChunkCoord> m_ChunkCoords = new();

		public ChunkCoords(IEnumerable<GridCoord> gridCoords, ChunkSize chunkSize) =>
			SplitIntoChunkLayerCoords(gridCoords, chunkSize);

		public ChunkCoord GetChunkCoord(ChunkKey chunkKey) => m_ChunkCoords[chunkKey];

		private void SplitIntoChunkLayerCoords(IEnumerable<GridCoord> gridCoords, ChunkSize chunkSize)
		{
			foreach (var gridCoord in gridCoords)
			{
				var chunkCoord = Tilemap3DUtility.GridToChunkCoord(gridCoord, chunkSize);
				var chunkKey = Tilemap3DUtility.GetChunkKey(chunkCoord);
				AddChunkCoord(chunkKey, chunkCoord);

				var layerCoord = Tilemap3DUtility.GridToLayerCoord(gridCoord, chunkSize);
				AddLayerCoord(chunkKey, layerCoord);
			}
		}

		private void AddChunkCoord(ChunkKey chunkKey, in ChunkCoord chunkCoord)
		{
			if (m_ChunkCoords.ContainsKey(chunkKey) == false)
				m_ChunkCoords[chunkKey] = chunkCoord;
		}

		private void AddLayerCoord(ChunkKey chunkKey, in LayerCoord layerCoord)
		{
			if (TryGetValue(chunkKey, out var coords))
				coords.Add(layerCoord);
			else
				Add(chunkKey, new List<LayerCoord> { layerCoord });
		}
	}
}
