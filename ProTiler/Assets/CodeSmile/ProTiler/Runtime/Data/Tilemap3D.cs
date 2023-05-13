// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Data
{
	[Serializable]
	internal class Tilemap3DChunks : Dictionary<long, Tilemap3DChunk> {}

	/// <summary>
	///     Container for Tile3DCoord collections that are automatically divided into chunks.
	/// </summary>
	internal class LayerCoordTiles : Dictionary<long, IList<Tile3DCoord>>
	{
		public LayerCoordTiles(IEnumerable<Tile3DCoord> tileCoords, ChunkSize chunkSize) =>
			ToLayerTileCoords(tileCoords, chunkSize);

		private void ToLayerTileCoords(IEnumerable<Tile3DCoord> tileCoords, ChunkSize chunkSize)
		{
			foreach (var tileCoord in tileCoords)
			{
				var chunkCoord = Tilemap3DUtility.GridToChunkCoord(tileCoord.Coord, chunkSize);
				var layerCoord = Tilemap3DUtility.GridToLayerCoord(tileCoord.Coord, chunkSize);
				var layerTileCoord = new Tile3DCoord(layerCoord, tileCoord.Tile);

				var chunkKey = Tilemap3DUtility.GetChunkKey(chunkCoord);
				if (TryGetValue(chunkKey, out var coords))
					coords.Add(layerTileCoord);
				else
					Add(chunkKey, new List<Tile3DCoord> { layerTileCoord });
			}
		}
	}

	internal struct GridChunkLayerCoord
	{
		public GridCoord GridCoord;
		public ChunkCoord ChunkCoord;
		public GridCoord LayerCoord;

		public GridChunkLayerCoord(GridCoord gridCoord, ChunkSize chunkSize)
		{
			GridCoord = gridCoord;
			ChunkCoord = Tilemap3DUtility.GridToChunkCoord(GridCoord, chunkSize);
			LayerCoord = Tilemap3DUtility.GridToLayerCoord(GridCoord, chunkSize);
		}

		public override string ToString() => $"(Chunk:{ChunkCoord}, Layer:{LayerCoord}, Grid:{GridCoord})";
	}

	internal static class TileLayerCoordsExt
	{
		public static IEnumerable<Tile3DCoord> ToGridCoords(this IEnumerable<Tile3DCoord> layerCoordTiles,
			ChunkCoord chunkCoord, ChunkSize chunkSize)
		{
			var count = layerCoordTiles.Count();
			var gridCoords = new Tile3DCoord[count];
			var i = 0;
			foreach (var layerCoordTile in layerCoordTiles)
			{
				gridCoords[i] = new Tile3DCoord(layerCoordTile);
				gridCoords[i].Coord = Tilemap3DUtility.LayerToGridCoord(layerCoordTile.Coord, chunkCoord, chunkSize);
				i++;
			}

			return gridCoords;
		}
	}

	internal static class GridChunkLayerCoordsExt
	{
		public static IEnumerable<GridCoord> ToLayerCoords(this IList<GridChunkLayerCoord> chunkLayerCoords)
		{
			var count = chunkLayerCoords.Count;
			var layerCoords = new GridCoord[count];
			for (var i = 0; i < count; i++)
				layerCoords[i] = chunkLayerCoords[i].LayerCoord;

			return layerCoords;
		}
	}

	internal class GridChunkLayerCoords : Dictionary<long, IList<GridChunkLayerCoord>>
	{
		public GridChunkLayerCoords(IEnumerable<GridCoord> gridCoords, ChunkSize chunkSize) =>
			ToGridChunkLayerCoords(gridCoords, chunkSize);

		private void ToGridChunkLayerCoords(IEnumerable<GridCoord> gridCoords, ChunkSize chunkSize)
		{
			foreach (var gridCoord in gridCoords)
			{
				var gridChunkLayerCoord = new GridChunkLayerCoord(gridCoord, chunkSize);

				var chunkKey = Tilemap3DUtility.GetChunkKey(gridChunkLayerCoord.ChunkCoord);
				if (TryGetValue(chunkKey, out var coords))
					coords.Add(gridChunkLayerCoord);
				else
					Add(chunkKey, new List<GridChunkLayerCoord> { gridChunkLayerCoord });
			}
		}
	}

	/// <summary>
	///     Container for a 3D tilemap.
	/// </summary>
	[Serializable]
	public class Tilemap3D
	{
		private Tilemap3DChunks m_Chunks;
		private ChunkSize m_ChunkSize;

		internal ChunkSize ChunkSize => m_ChunkSize;
		public int ChunkCount => m_Chunks.Count;

		public Tilemap3D()
			: this(Tilemap3DUtility.MinChunkSize) {}

		public Tilemap3D(ChunkSize chunkSize) => InitChunks(Tilemap3DUtility.ClampChunkSize(chunkSize));

		private void InitChunks(ChunkSize chunkSize)
		{
			m_ChunkSize = chunkSize;
			m_Chunks = new Tilemap3DChunks();
		}

		/// <summary>
		///     Sets tiles on affected chunks.
		/// </summary>
		/// <param name="gridCoordTiles"></param>
		public void SetTiles(IEnumerable<Tile3DCoord> gridCoordTiles)
		{
			var layerCoordTiles = new LayerCoordTiles(gridCoordTiles, m_ChunkSize);
			foreach (var chunkKey in layerCoordTiles.Keys)
			{
				var chunk = GetOrCreateChunk(chunkKey);
				var layerTiles = layerCoordTiles[chunkKey];
				chunk.SetLayerTiles(layerTiles);
			}
		}

		/// <summary>
		///     Gets tiles from affected chunks.
		/// </summary>
		/// <param name="gridCoords"></param>
		/// <returns></returns>
		public IEnumerable<Tile3DCoord> GetTiles(IEnumerable<GridCoord> gridCoords)
		{
			var tileCoords = new List<Tile3DCoord>();
			var chunkLayerCoords = new GridChunkLayerCoords(gridCoords, m_ChunkSize);
			foreach (var chunkKey in chunkLayerCoords.Keys)
			{
				var chunk = m_Chunks[chunkKey];
				var layerCoords = chunkLayerCoords[chunkKey].ToLayerCoords();
				var layerTileCoords = chunk.GetLayerTiles(layerCoords);
				var gridTileCoords =
					layerTileCoords.ToGridCoords(chunkLayerCoords[chunkKey][0].ChunkCoord, m_ChunkSize);
				tileCoords.AddRange(gridTileCoords);
			}

			return tileCoords;
		}

		private Tilemap3DChunk GetOrCreateChunk(long chunkKey)
		{
			if (TryGetChunk(chunkKey, out var chunk))
				return chunk;

			return CreateChunk(chunkKey);
		}

		private Tilemap3DChunk CreateChunk(long chunkKey) => m_Chunks[chunkKey] = new Tilemap3DChunk(m_ChunkSize);

		private bool TryGetChunk(long key, out Tilemap3DChunk chunk) => m_Chunks.TryGetValue(key, out chunk);

		[ExcludeFromCodeCoverage]
		public override string ToString() => $"{nameof(Tilemap3D)}(ChunkCount: {ChunkCount}, ChunkSize: {ChunkSize})";
	}
}
