// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Properties;
using ChunkKey = System.Int64;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;
using LayerCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Model
{
	/// <summary>
	///     Container for a 3D tilemap.
	/// </summary>
	[FullCovered]
	[Serializable]
	public sealed class Tilemap3D
	{
		/// <summary>
		///     This is a required technical limitation to have at least 2x2 tiles per chunk.
		///     The hashes of chunks with size of less than 2x2 would not be unique.
		/// </summary>
		internal static readonly ChunkSize MinChunkSize = new(2, 2);
		internal static readonly ChunkSize DefaultChunkSize = new(16, 16);

		[CreateProperty] private ChunkSize m_ChunkSize;
		[CreateProperty] private Tilemap3DChunks m_Chunks;

		internal ChunkSize ChunkSize { get => m_ChunkSize; set => InitChunks(value); }
		internal Int32 ChunkCount => m_Chunks.Count;
		internal Int32 TileCount => m_Chunks.TileCount;

		public Tilemap3D()
			: this(DefaultChunkSize) {}

		public Tilemap3D(ChunkSize chunkSize) => InitChunks(Tilemap3DUtility.ClampChunkSize(chunkSize));

		public Int32 GetLayerCount(ChunkCoord chunkCoord) =>
			TryGetChunk(Tilemap3DUtility.GetChunkKey(chunkCoord), out var chunk) ? chunk.LayerCount : 0;

		/// <summary>
		///     Sets tiles on affected chunks.
		/// </summary>
		/// <param name="gridCoordTiles"></param>
		public void SetTiles(IEnumerable<Tile3DCoord> gridCoordTiles)
		{
			var chunkTileCoords = new ChunkTileCoords(gridCoordTiles, m_ChunkSize);
			foreach (var chunkKey in chunkTileCoords.Keys)
			{
				GetOrCreateChunk(chunkKey, out var chunk);
				chunk.SetLayerTiles(chunkTileCoords[chunkKey]);
			}
		}

		public IDictionary<GridCoord, Tile3DCoord> GetTiles(IEnumerable<GridCoord> gridCoords)
		{
			var tileCoords = new Dictionary<GridCoord, Tile3DCoord>();

			var existingTileCoords = GetExistingTiles(gridCoords);
			foreach (var tileCoord in existingTileCoords)
				tileCoords.Add(tileCoord.Coord, tileCoord);

			// add empty tiles for any non-existing tile
			foreach (var coord in gridCoords)
			{
				if (tileCoords.ContainsKey(coord) == false)
					tileCoords.Add(coord, new Tile3DCoord(coord));
			}

			return tileCoords;
		}

		/// <summary>
		///     Returns only existing tiles from already (existing) tilemap chunks.
		///     Coordinates in chunks or layers that haven't been created yet will not return a Tile3DCoord instance.
		/// </summary>
		/// <param name="gridCoords"></param>
		/// <returns></returns>
		public IEnumerable<Tile3DCoord> GetExistingTiles(IEnumerable<GridCoord> gridCoords)
		{
			var tileCoords = new List<Tile3DCoord>();

			var chunkLayerCoords = new ChunkCoords(gridCoords, m_ChunkSize);
			foreach (var chunkKey in chunkLayerCoords.Keys)
			{
				if (TryGetChunk(chunkKey, out var chunk) == false)
					continue;

				var chunkCoord = chunkLayerCoords.GetChunkCoord(chunkKey);
				var layerCoords = chunkLayerCoords[chunkKey];
				var chunkTileCoords = chunk.GetExistingLayerTiles(chunkCoord, layerCoords);

				tileCoords.AddRange(chunkTileCoords);
			}

			return tileCoords;
		}

		private void GetOrCreateChunk(ChunkKey chunkKey, out Tilemap3DChunk chunk)
		{
			if (TryGetChunk(chunkKey, out chunk) == false)
				chunk = CreateChunk(chunkKey);
		}

		private Tilemap3DChunk CreateChunk(ChunkKey chunkKey) => m_Chunks[chunkKey] = new Tilemap3DChunk(m_ChunkSize);

		private Boolean TryGetChunk(ChunkKey chunkKey, out Tilemap3DChunk chunk) =>
			m_Chunks.TryGetValue(chunkKey, out chunk);

		private void InitChunks(ChunkSize chunkSize)
		{
			m_ChunkSize = chunkSize;
			m_Chunks = new Tilemap3DChunks();
		}

		[ExcludeFromCodeCoverage]
		public override String ToString() =>
			$"{nameof(Tilemap3D)}(Size: {ChunkSize}, Chunks: {ChunkCount}, Tiles: {TileCount})";

		/// <summary>
		///     Container for Tile3DCoord collections that are automatically divided into chunks.
		/// </summary>
		private sealed class ChunkTileCoords : Dictionary<ChunkKey, IList<Tile3DCoord>>
		{
			internal ChunkTileCoords(IEnumerable<Tile3DCoord> tileCoords, ChunkSize chunkSize) =>
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

		/// <summary>
		///     Coords divided into chunks
		/// </summary>
		private sealed class ChunkCoords : Dictionary<ChunkKey, IList<GridCoord>>
		{
			private readonly Dictionary<ChunkKey, ChunkCoord> m_ChunkCoords = new();

			internal ChunkCoords(IEnumerable<GridCoord> gridCoords, ChunkSize chunkSize) =>
				SplitIntoChunkLayerCoords(gridCoords, chunkSize);

			internal ChunkCoord GetChunkCoord(ChunkKey chunkKey) => m_ChunkCoords[chunkKey];

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
					Add(chunkKey, new List<GridCoord> { layerCoord });
			}
		}

		/// <summary>
		///     Collection of chunks
		/// </summary>
		[Serializable]
		private sealed class Tilemap3DChunks
		{
			[CreateProperty] private Dictionary<ChunkKey, Tilemap3DChunk> m_Chunks = new();
			internal Tilemap3DChunk this[ChunkKey chunkKey]
			{
				//get => m_Chunks[chunkKey];
				set => m_Chunks[chunkKey] = value;
			}

			internal Int32 TileCount
			{
				get
				{
					var tileCount = 0;
					foreach (var chunk in m_Chunks.Values)
						tileCount += chunk.TileCount;

					return tileCount;
				}
			}
			internal Int32 Count => m_Chunks.Count;

			internal Boolean TryGetValue(ChunkKey key, out Tilemap3DChunk chunk) =>
				m_Chunks.TryGetValue(key, out chunk);
		}
	}
}
