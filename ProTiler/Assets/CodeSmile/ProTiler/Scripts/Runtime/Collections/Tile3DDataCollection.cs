// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile.ProTiler.Collections
{
	/// <summary>
	///     Represents a single, flat layer of tiles. Essentially a 2D array.
	/// </summary>
	[Serializable]
	public class Tile3DDataCollection
	{
		private int m_Width;
		private int m_Height;
		private Tile3DData[] m_Tiles;

		public Tile3DData this[int index]
		{
			get => m_Tiles[index];
			set => m_Tiles[index] = value;
		}

		public Tile3DData this[int x, int y]
		{
			get => m_Tiles[Grid3DUtility.ToIndex(x, y, m_Width)];
			set => m_Tiles[Grid3DUtility.ToIndex(x, y, m_Width)] = value;
		}
		public int Width => m_Width;
		public int Height => m_Height;

		public int Count
		{
			get
			{
				var count = 0;
				foreach (var tileData in m_Tiles)
				{
					if (tileData.IsEmpty == false)
						count++;
				}
				return count;
			}
		}

		public int Capacity => m_Tiles.Length;

		public Tile3DDataCollection(Vector2Int size)
			: this(size.x, size.y) {}

		public Tile3DDataCollection(int width, int height)
		{
			m_Width = width;
			m_Height = height;
			m_Tiles = new Tile3DData[width * height];
		}

		public void SetTiles(Tile3DCoordData[] tileChangeData)
		{
			foreach (var changeData in tileChangeData)
				this[changeData.Coord.x, changeData.Coord.z] = changeData.TileData;
		}
	}
}
