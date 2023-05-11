// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Properties;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Data
{
	public class Tile3DLayerCollection : List<Tile3DLayer> {}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Tilemap3DChunk
	{
		[CreateProperty] private ChunkSize m_Size;
		[CreateProperty] private Tile3DLayerCollection m_Layers;

		/// <summary>
		///     Gets a layer at the given index (aka "height").
		///     Caution: No bounds check is performed.
		/// </summary>
		/// <param name="layerIndex"></param>
		public Tile3DLayer this[int layerIndex]
		{
			get => m_Layers[layerIndex];
			set => m_Layers[layerIndex] = value;
		}

		public ChunkSize Size => m_Size;

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

		public Tilemap3DChunk(ChunkSize size)
		{
			m_Size = size;
			m_Layers = new Tile3DLayerCollection();
			m_Layers.Capacity = 1;
		}

		public void SetTiles(IEnumerable<Tile3DCoord> tileCoords)
		{
			foreach (var coordData in tileCoords)
			{
				var coord = coordData.Coord;
				var layer = GetOrCreateHeightLayer(coord.y);
				var tileIndex = ToTileIndex(coord);
				layer[tileIndex] = coordData.Tile;
			}
		}

		private Tile3DLayer GetOrCreateHeightLayer(int height)
		{
			if (height < m_Layers.Count)
				return m_Layers[height];

			AddLayersUpToGivenHeight(height);
			return m_Layers[height];
		}

		private void AddLayersUpToGivenHeight(int height)
		{
			var increasedLayerCount = height + 1;
			for (var i = m_Layers.Count; i < increasedLayerCount; i++)
				m_Layers.Add(new Tile3DLayer(m_Size));
		}

		private int ToTileIndex(GridCoord coord) => Grid3DUtility.ToIndex2D(ToLayerCoord(coord), m_Size.x);
		private GridCoord ToLayerCoord(GridCoord coord) => coord - ToChunkCoord(coord);
		private GridCoord ToChunkCoord(GridCoord coord) => new(coord.x / m_Size.x, coord.y, coord.z / m_Size.y);
	}
}
