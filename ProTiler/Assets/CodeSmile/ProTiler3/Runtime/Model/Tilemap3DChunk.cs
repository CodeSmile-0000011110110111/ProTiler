// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.ProTiler3.Runtime.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Properties;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int2;
using GridCoord = Unity.Mathematics.int3;

namespace CodeSmile.ProTiler3.Runtime.Model
{
	/// <summary>
	///     A chunk is one part of a larger tilemap at a given position offset.
	///     It contains one or more height layers of the same size.
	/// </summary>
	[FullCovered]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class Tilemap3DChunk
	{
		[CreateProperty] private ChunkSize m_Size;
		[CreateProperty] private Tile3DLayers m_Layers;

		/// <summary>
		///     Gets a layer at the given index (aka "height").
		///     Caution: No bounds check is performed.
		/// </summary>
		/// <param name="layerIndex"></param>
		internal Tile3DLayer this[Int32 layerIndex] => m_Layers[layerIndex];

		/// <summary>
		///     The size (width, length) of the chunk.
		/// </summary>
		internal ChunkSize Size => m_Size;

		/// <summary>
		///     The number of height layers in this chunk.
		/// </summary>
		internal Int32 LayerCount => m_Layers.Count;

		/// <summary>
		///     The number of non-empty tiles in this chunk's height layers.
		/// </summary>
		internal Int32 TileCount
		{
			get
			{
				var tileCount = 0;
				foreach (Tile3DLayer layer in m_Layers)
					tileCount += layer.TileCount;
				return tileCount;
			}
		}

		[SuppressMessage("NDepend", "ND1701:PotentiallyDeadMethods",
			Justification = "public parameter-less ctor required by serialization")]
		public Tilemap3DChunk() {}

		/// <summary>
		///     Creates a new chunk instance with the given size (width, length).
		/// </summary>
		/// <param name="size"></param>
		internal Tilemap3DChunk(ChunkSize size)
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
		internal void SetLayerTiles(IEnumerable<Tile3DCoord> coordTiles)
		{
			foreach (var coordTile in coordTiles)
			{
				var layerCoord = coordTile.GetLayerCoord(m_Size);
				var layer = GetOrCreateHeightLayer(layerCoord.y);
				var tileIndex = ToTileIndex(layerCoord);
				layer[tileIndex] = coordTile.Tile;
			}
		}

		internal IEnumerable<Tile3DCoord> GetExistingLayerTiles(ChunkCoord chunkCoord,
			IEnumerable<GridCoord> layerCoords)
		{
			var layerTileCoords = new List<Tile3DCoord>(layerCoords.Count());

			var tileCoordIndex = 0;
			foreach (var layerCoord in layerCoords)
			{
				var layer = GetHeightLayerOrDefault(layerCoord.y);
				if (layer.IsInitialized)
				{
					var gridCoord = Tilemap3DUtility.LayerToGridCoord(layerCoord, chunkCoord, m_Size);
					var layerTileIndex = ToTileIndex(layerCoord);
					layerTileCoords.Add(new Tile3DCoord(gridCoord, layer[layerTileIndex]));
					tileCoordIndex++;
				}
			}

			return layerTileCoords;
		}

		private Tile3DLayer GetOrCreateHeightLayer(Int32 height)
		{
			AddLayersUpToGivenHeight(height);
			return GetHeightLayer(height);
		}

		private Tile3DLayer GetHeightLayerOrDefault(Int32 height)
		{
			if (height < m_Layers.Count)
				return GetHeightLayer(height);

			return default;
		}

		private Tile3DLayer GetHeightLayer(Int32 height) => m_Layers[height];

		private void AddLayersUpToGivenHeight(Int32 height)
		{
			for (var i = m_Layers.Count; i < height + 1; i++)
				m_Layers.Add(new Tile3DLayer(m_Size));

			DiscardExcessLayersCapacity();
		}

		/// <summary>
		///     List<> will increment capacity by doubling it, thus leaving significant unused additional capacity
		///     while we'd rather conserve memory usage.
		/// </summary>
		private void DiscardExcessLayersCapacity() => m_Layers.Capacity = m_Layers.Count;

		private Int32 ToTileIndex(GridCoord coord) => Grid3DUtility.ToIndex2D(ToLayerCoord(coord), m_Size.x);
		private GridCoord ToLayerCoord(GridCoord coord) => coord - ToChunkCoord(coord);
		private GridCoord ToChunkCoord(GridCoord coord) => new(coord.x / m_Size.x, coord.y, coord.z / m_Size.y);

		/// <summary>
		///     A collection of Tile3DLayer instances.
		/// </summary>
		[Serializable]
		private sealed class Tile3DLayers : IEnumerable<Tile3DLayer>
		{
			[CreateProperty] private readonly List<Tile3DLayer> m_Layers = new();
			internal Tile3DLayer this[Int32 index] => m_Layers[index];
			internal Int32 Count => m_Layers.Count;
			internal Int32 Capacity
			{
				[ExcludeFromCodeCoverage] get => m_Layers.Capacity;
				set => m_Layers.Capacity = value;
			}

			[ExcludeFromCodeCoverage]
			IEnumerator<Tile3DLayer> IEnumerable<Tile3DLayer>.GetEnumerator() => m_Layers.GetEnumerator();

			[ExcludeFromCodeCoverage]
			public IEnumerator GetEnumerator() => m_Layers.GetEnumerator();

			internal void Add(Tile3DLayer layer) => m_Layers.Add(layer);
		}
	}
}
