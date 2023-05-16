// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile.ProTiler.Data
{
	internal class ChunkCoords : Dictionary<long, IList<Vector3Int>>
	{
		private readonly Dictionary<long, Vector2Int> m_ChunkCoords = new();

		public ChunkCoords(IEnumerable<Vector3Int> gridCoords, Vector2Int chunkSize) =>
			SplitIntoChunkLayerCoords(gridCoords, chunkSize);

		public Vector2Int GetChunkCoord(long chunkKey) => m_ChunkCoords[chunkKey];

		private void SplitIntoChunkLayerCoords(IEnumerable<Vector3Int> gridCoords, Vector2Int chunkSize)
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

		private void AddChunkCoord(long chunkKey, in Vector2Int chunkCoord)
		{
			if (m_ChunkCoords.ContainsKey(chunkKey) == false)
				m_ChunkCoords[chunkKey] = chunkCoord;
		}

		private void AddLayerCoord(long chunkKey, in Vector3Int layerCoord)
		{
			if (TryGetValue(chunkKey, out var coords))
				coords.Add(layerCoord);
			else
				Add(chunkKey, new List<Vector3Int> { layerCoord });
		}
	}
}
