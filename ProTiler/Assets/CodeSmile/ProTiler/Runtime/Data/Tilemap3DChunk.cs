// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Properties;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     A collection of Tile3DLayer instances.
	/// </summary>
	[Serializable]
	internal class Tile3DLayerCollection : List<Tile3DLayer> {}

	/// <summary>
	///     A chunk is one part of a larger tilemap at a given position offset.
	///     It contains one or more height layers of the same size.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class Tilemap3DChunk
	{
		[CreateProperty] private ChunkSize m_Size;
		[CreateProperty] private Tile3DLayerCollection m_Layers;

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
				foreach (var layer in m_Layers)
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
				foreach (var layer in m_Layers)
					tileCount += layer.TileCount;
				return tileCount;
			}
		}

		/// <summary>
		///     Creates a new chunk instance with the given size (width, length).
		/// </summary>
		/// <param name="size"></param>
		public Tilemap3DChunk(ChunkSize size)
		{
			m_Size = size;
			m_Layers = new Tile3DLayerCollection();
			m_Layers.Capacity = 1;
		}

		/// <summary>
		///     Set tiles in the chunk's layers at the given coordinates.
		///     Will create additional height layers as needed based on the Y coordinate.
		/// </summary>
		/// <param name="layerCoordTiles"></param>
		public void SetLayerTiles(IEnumerable<Tile3DCoord> layerCoordTiles)
		{
			foreach (var layerCoordTile in layerCoordTiles)
			{
				var coord = layerCoordTile.Coord;
				var layer = GetOrCreateHeightLayer(coord.y);
				var tileIndex = ToTileIndex(coord);
				layer[tileIndex] = layerCoordTile.Tile;
			}
		}

		public IEnumerable<Tile3DCoord> GetLayerTiles(IEnumerable<GridCoord> layerCoords)
		{
			var layerTileCoords = new Tile3DCoord[layerCoords.Count()];
			var tileCoordIndex = 0;
			foreach (var layerCoord in layerCoords)
			{
				var layer = GetHeightLayerOrDefault(layerCoord.y);
				if (layer.IsInitialized)
				{
					var tileIndex = ToTileIndex(layerCoord);
					layerTileCoords[tileCoordIndex].Coord = layerCoord;
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
		/// List<> will increment capacity by doubling it, thus leaving significant unused additional capacity
		/// while we'd rather conserve memory usage.
		/// </summary>
		private void DiscardExcessCapacityFromLayersCollection() => m_Layers.Capacity = m_Layers.Count;

		private int ToTileIndex(GridCoord coord) => Grid3DUtility.ToIndex2D(ToLayerCoord(coord), m_Size.x);
		private GridCoord ToLayerCoord(GridCoord coord) => coord - ToChunkCoord(coord);
		private GridCoord ToChunkCoord(GridCoord coord) => new(coord.x / m_Size.x, coord.y, coord.z / m_Size.y);
	}
}
