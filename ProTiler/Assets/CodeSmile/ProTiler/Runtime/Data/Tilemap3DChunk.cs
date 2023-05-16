// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Properties;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     A chunk is one part of a larger tilemap at a given position offset.
	///     It contains one or more height layers of the same size.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Tilemap3DChunk
	{
		[CreateProperty] private ChunkSize m_Size;
		[CreateProperty] private Tile3DLayers m_Layers;

		/// <summary>
		///     Gets a layer at the given index (aka "height").
		///     Caution: No bounds check is performed.
		/// </summary>
		/// <param name="layerIndex"></param>
		internal Tile3DLayer this[int layerIndex] => m_Layers[layerIndex];

		/// <summary>
		///     The size (width, length) of the chunk.
		/// </summary>
		public ChunkSize Size => m_Size;

		/// <summary>
		///     The number of non-empty height layers in this chunk.
		/// </summary>
		public int LayerCount
		{
			get
			{
				var layerCount = 0;
				foreach (Tile3DLayer layer in m_Layers)
					layerCount += layer.IsInitialized ? 1 : 0;
				return layerCount;
			}
		}

		/// <summary>
		///     The number of non-empty tiles in this chunk's height layers.
		/// </summary>
		public int TileCount
		{
			get
			{
				var tileCount = 0;
				foreach (Tile3DLayer layer in m_Layers)
					tileCount += layer.TileCount;
				return tileCount;
			}
		}

		// required by serialization
		public Tilemap3DChunk() {}

		/// <summary>
		///     Creates a new chunk instance with the given size (width, length).
		/// </summary>
		/// <param name="size"></param>
		public Tilemap3DChunk(ChunkSize size)
		{
			m_Size = size;
			m_Layers = new Tile3DLayers();
			m_Layers.Capacity = 1;
		}

		/// <summary>
		///     Set tiles in the chunk's layers at the given coordinates.
		///     Will create additional height layers as needed based on the Y coordinate.
		/// </summary>
		/// <param name="coordTiles"></param>
		public void SetLayerTiles(IEnumerable<Tile3DCoord> coordTiles)
		{
			foreach (var coordTile in coordTiles)
			{
				var layerCoord = coordTile.GetLayerCoord(m_Size);
				var layer = GetOrCreateHeightLayer(layerCoord.y);
				var tileIndex = ToTileIndex(layerCoord);
				layer[tileIndex] = coordTile.Tile;
			}
		}

		public IEnumerable<Tile3DCoord> GetLayerTiles(ChunkCoord chunkCoord, IEnumerable<GridCoord> layerCoords)
		{
			var layerTileCoords = new Tile3DCoord[layerCoords.Count()];

			var tileCoordIndex = 0;
			foreach (var layerCoord in layerCoords)
			{
				var layer = GetHeightLayerOrDefault(layerCoord.y);
				if (layer.IsInitialized)
				{
					var tileIndex = ToTileIndex(layerCoord);
					var gridCoord = Tilemap3DUtility.LayerToGridCoord(layerCoord, chunkCoord, m_Size);
					layerTileCoords[tileCoordIndex].Coord = gridCoord;
					layerTileCoords[tileCoordIndex].Tile = layer[tileIndex];
					tileCoordIndex++;
				}
			}
			return layerTileCoords;
		}

		private Tile3DLayer GetOrCreateHeightLayer(int height)
		{
			AddLayersUpToGivenHeight(height);
			return GetHeightLayer(height);
		}

		private Tile3DLayer GetHeightLayerOrDefault(int height)
		{
			if (height < m_Layers.Count)
				return GetHeightLayer(height);

			return default;
		}

		private Tile3DLayer GetHeightLayer(int height) => m_Layers[height];

		private void AddLayersUpToGivenHeight(int height)
		{
			for (var i = m_Layers.Count; i < height + 1; i++)
				m_Layers.Add(new Tile3DLayer(m_Size));

			DiscardExcessCapacityFromLayersCollection();
		}

		/// <summary>
		///     List<> will increment capacity by doubling it, thus leaving significant unused additional capacity
		///     while we'd rather conserve memory usage.
		/// </summary>
		private void DiscardExcessCapacityFromLayersCollection() => m_Layers.Capacity = m_Layers.Count;

		private int ToTileIndex(GridCoord coord) => Grid3DUtility.ToIndex2D(ToLayerCoord(coord), m_Size.x);
		private GridCoord ToLayerCoord(GridCoord coord) => coord - ToChunkCoord(coord);
		private GridCoord ToChunkCoord(GridCoord coord) => new(coord.x / m_Size.x, coord.y, coord.z / m_Size.y);
	}
}
