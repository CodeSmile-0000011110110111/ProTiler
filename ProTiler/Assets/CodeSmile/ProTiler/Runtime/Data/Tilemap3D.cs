// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Properties;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     Container for a 3D tilemap.
	/// </summary>
	[Serializable]
	public class Tilemap3D
	{
		[CreateProperty] private ChunkSize m_ChunkSize;
		[CreateProperty] private Tilemap3DChunks m_Chunks;

		internal ChunkSize ChunkSize => m_ChunkSize;
		internal int ChunkCount => m_Chunks.Count;
		internal int TileCount => m_Chunks.TileCount;

		public Tilemap3D()
			: this(Tilemap3DUtility.MinChunkSize) {}

		public Tilemap3D(ChunkSize chunkSize) => InitChunks(Tilemap3DUtility.ClampChunkSize(chunkSize));

		public int GetLayerCount(ChunkCoord chunkCoord) =>
			TryGetChunk(Tilemap3DUtility.GetChunkKey(chunkCoord), out var chunk) ? chunk.LayerCount : 0;

		/// <summary>
		///     Sets tiles on affected chunks.
		/// </summary>
		/// <param name="gridCoordTiles"></param>
		public void SetTiles(IEnumerable<Tile3DCoord> gridCoordTiles)
		{
			var layerCoordTiles = new ChunkTileCoords(gridCoordTiles, m_ChunkSize);
			foreach (var chunkKey in layerCoordTiles.Keys)
			{
				GetOrCreateChunk(chunkKey, out var chunk);
				chunk.SetLayerTiles(layerCoordTiles[chunkKey]);
			}
		}

		/// <summary>
		///     Returns tiles from tilemap chunks.
		/// </summary>
		/// <param name="gridCoords"></param>
		/// <returns></returns>
		public IEnumerable<Tile3DCoord> GetTiles(IEnumerable<GridCoord> gridCoords)
		{
			var tileCoords = new List<Tile3DCoord>();

			var chunkLayerCoords = new ChunkCoords(gridCoords, m_ChunkSize);
			foreach (var chunkKey in chunkLayerCoords.Keys)
			{
				if (TryGetChunk(chunkKey, out var chunk) == false)
					continue;

				var chunkCoord = chunkLayerCoords.GetChunkCoord(chunkKey);
				var layerCoords = chunkLayerCoords[chunkKey];
				var chunkTileCoords = chunk.GetLayerTiles(chunkCoord, layerCoords);

				tileCoords.AddRange(chunkTileCoords);
			}

			return tileCoords;
		}

		private void GetOrCreateChunk(long chunkKey, out Tilemap3DChunk chunk)
		{
			if (TryGetChunk(chunkKey, out chunk) == false)
				chunk = CreateChunk(chunkKey);
		}

		private Tilemap3DChunk CreateChunk(long chunkKey) => m_Chunks[chunkKey] = new Tilemap3DChunk(m_ChunkSize);

		private bool TryGetChunk(long chunkKey, out Tilemap3DChunk chunk) => m_Chunks.TryGetValue(chunkKey, out chunk);

		private void InitChunks(ChunkSize chunkSize)
		{
			m_ChunkSize = chunkSize;
			m_Chunks = new Tilemap3DChunks();
		}

		[ExcludeFromCodeCoverage]
		public override string ToString() => $"{nameof(Tilemap3D)}(ChunkCount: {ChunkCount}, ChunkSize: {ChunkSize})";
	}
}
