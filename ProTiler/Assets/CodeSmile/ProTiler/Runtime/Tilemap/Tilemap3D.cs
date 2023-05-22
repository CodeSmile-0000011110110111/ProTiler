// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.ProTiler.Tile;
using CodeSmile.ProTiler.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Unity.Properties;
using ChunkKey = System.Int64;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;
using LayerCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Tilemap
{
	/// <summary>
	///     Container for a 3D tilemap.
	/// </summary>
	[FullCovered]
	[Serializable]
	public class Tilemap3D
	{
		[CreateProperty] private ChunkSize m_ChunkSize;
		[CreateProperty] private Tilemap3DChunks m_Chunks;

		[Pure] internal ChunkSize ChunkSize { get => m_ChunkSize; set => InitChunks(value); }
		[Pure] internal Int32 ChunkCount => m_Chunks.Count;
		[Pure] internal Int32 TileCount => m_Chunks.TileCount;

		[Pure] public Tilemap3D()
			: this(Tilemap3DUtility.MinChunkSize) {}

		[Pure] public Tilemap3D(ChunkSize chunkSize) => InitChunks(Tilemap3DUtility.ClampChunkSize(chunkSize));

		[Pure] public Int32 GetLayerCount(ChunkCoord chunkCoord) =>
			TryGetChunk(Tilemap3DUtility.GetChunkKey(chunkCoord), out var chunk) ? chunk.LayerCount : 0;

		/// <summary>
		///     Sets tiles on affected chunks.
		/// </summary>
		/// <param name="gridCoordTiles"></param>
		[Pure] public void SetTiles(IEnumerable<Tile3DCoord> gridCoordTiles)
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
		[Pure] public IEnumerable<Tile3DCoord> GetTiles(IEnumerable<GridCoord> gridCoords)
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

		[Pure] private void GetOrCreateChunk(ChunkKey chunkKey, out Tilemap3DChunk chunk)
		{
			if (TryGetChunk(chunkKey, out chunk) == false)
				chunk = CreateChunk(chunkKey);
		}

		[Pure] private Tilemap3DChunk CreateChunk(ChunkKey chunkKey) =>
			m_Chunks[chunkKey] = new Tilemap3DChunk(m_ChunkSize);

		[Pure] private Boolean TryGetChunk(ChunkKey chunkKey, out Tilemap3DChunk chunk) =>
			m_Chunks.TryGetValue(chunkKey, out chunk);

		private void InitChunks(ChunkSize chunkSize)
		{
			m_ChunkSize = chunkSize;
			m_Chunks = new Tilemap3DChunks();
		}

		[ExcludeFromCodeCoverage]
		[Pure] public override String ToString() =>
			$"{nameof(Tilemap3D)}(Size: {ChunkSize}, Chunks: {ChunkCount}, Tiles: {TileCount})";


		/// <summary>
		///     Container for Tile3DCoord collections that are automatically divided into chunks.
		/// </summary>
		private sealed class ChunkTileCoords : Dictionary<ChunkKey, IList<Tile3DCoord>>
		{
			[Pure] internal ChunkTileCoords(IEnumerable<Tile3DCoord> tileCoords, ChunkSize chunkSize) =>
				SplitTileCoordsIntoChunks(tileCoords, chunkSize);

			[Pure] private void SplitTileCoordsIntoChunks(IEnumerable<Tile3DCoord> tileCoords, ChunkSize chunkSize)
			{
				foreach (var tileCoord in tileCoords)
				{
					var chunkCoord = tileCoord.GetChunkCoord(chunkSize);
					var chunkKey = Tilemap3DUtility.GetChunkKey(chunkCoord);
					AddTileCoordToChunk(chunkKey, tileCoord);
				}
			}

			[Pure] private void AddTileCoordToChunk(ChunkKey chunkKey, in Tile3DCoord tileCoord)
			{
				if (TryGetValue(chunkKey, out var coords))
					coords.Add(tileCoord);
				else
					Add(chunkKey, new List<Tile3DCoord> { tileCoord });
			}
		}

		/// <summary>
		///     Coords divided into chunks
		/// </summary>
		private sealed class ChunkCoords : Dictionary<ChunkKey, IList<LayerCoord>>
		{
			private readonly Dictionary<ChunkKey, ChunkCoord> m_ChunkCoords = new();

			[Pure] internal ChunkCoords(IEnumerable<GridCoord> gridCoords, ChunkSize chunkSize) =>
				SplitIntoChunkLayerCoords(gridCoords, chunkSize);

			[Pure] internal ChunkCoord GetChunkCoord(ChunkKey chunkKey) => m_ChunkCoords[chunkKey];

			[Pure] private void SplitIntoChunkLayerCoords(IEnumerable<GridCoord> gridCoords, ChunkSize chunkSize)
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

			[Pure] private void AddChunkCoord(ChunkKey chunkKey, in ChunkCoord chunkCoord)
			{
				if (m_ChunkCoords.ContainsKey(chunkKey) == false)
					m_ChunkCoords[chunkKey] = chunkCoord;
			}

			[Pure] private void AddLayerCoord(ChunkKey chunkKey, in LayerCoord layerCoord)
			{
				if (TryGetValue(chunkKey, out var coords))
					coords.Add(layerCoord);
				else
					Add(chunkKey, new List<LayerCoord> { layerCoord });
			}
		}

		/// <summary>
		///     Collection of chunks
		/// </summary>
		[Serializable]
		private sealed class Tilemap3DChunks
		{
			[CreateProperty] private Dictionary<Int64, Tilemap3DChunk> m_Chunks = new();
			[Pure] internal Tilemap3DChunk this[Int64 chunkKey]
			{
				get => m_Chunks[chunkKey];
				set => m_Chunks[chunkKey] = value;
			}

			[Pure] internal Int32 TileCount
			{
				get
				{
					var tileCount = 0;
					foreach (var chunk in m_Chunks.Values)
						tileCount += chunk.TileCount;

					return tileCount;
				}
			}
			[Pure] internal Int32 Count => m_Chunks.Count;

			[Pure] internal Boolean TryGetValue(Int64 key, out Tilemap3DChunk chunk) =>
				m_Chunks.TryGetValue(key, out chunk);
		}
	}
}
