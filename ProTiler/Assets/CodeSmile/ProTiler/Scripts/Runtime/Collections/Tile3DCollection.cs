// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler.Collections
{
	/// <summary>
	///     Represents a single, flat layer of tiles. Essentially a 2D array.
	/// </summary>
	[Serializable]
	public class Tile3DCollection
	{
		// TODO: change to Vector2Int
		[SerializeField] private int m_Width;
		[SerializeField] private int m_Height;
		//private Vector2Int m_Size;
		[SerializeField] private Tile3D[] m_Tiles;

		public Tile3D this[int index]
		{
			get => m_Tiles[index];
			set => m_Tiles[index] = value;
		}

		public Tile3D this[int x, int y]
		{
			get => m_Tiles[Grid3DUtility.ToIndex2D(x, y, m_Width)];
			set => m_Tiles[Grid3DUtility.ToIndex2D(x, y, m_Width)] = value;
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

		public Tile3DCollection(Vector2Int size)
			: this(size.x, size.y) {}

		public Tile3DCollection(int width, int height)
		{
			m_Width = width;
			m_Height = height;
			m_Tiles = new Tile3D[width * height];
		}

		public override string ToString() =>
			$"{nameof(Tile3DCollection)}(Size: {new Vector2Int(m_Width, m_Height)}, Capacity: {Capacity}, Count: {Count})";

		public void SetTiles(Tile3DCoord[] tileCoordDatas)
		{
			foreach (var coordData in tileCoordDatas)
				this[coordData.Coord.x, coordData.Coord.z] = coordData.m_Tile;
		}
	}
}
