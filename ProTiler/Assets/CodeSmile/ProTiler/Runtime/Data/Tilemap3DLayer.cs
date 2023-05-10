// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using LayerSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     Represents a single, flat layer of tiles. Essentially a 2D array.
	/// </summary>
	[Serializable]
	public class Tilemap3DLayer
	{
		private LayerSize m_LayerSize;
		private Tile3D[] m_Tiles;

		public Tile3D this[int index]
		{
			get => m_Tiles[index];
			set => m_Tiles[index] = value;
		}

		public Tile3D this[int x, int y]
		{
			get => m_Tiles[Grid3DUtility.ToIndex2D(x, y, Width)];
			set => m_Tiles[Grid3DUtility.ToIndex2D(x, y, Width)] = value;
		}
		public int Width => m_LayerSize.x;
		public int Height => m_LayerSize.y;

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

		public Tilemap3DLayer()
			: this(new LayerSize(16, 16)) {}

		public Tilemap3DLayer(LayerSize size)
		{
			m_LayerSize = size;
			m_Tiles = new Tile3D[Width * Height];
		}

		[ExcludeFromCodeCoverage]
		public override string ToString() =>
			$"{nameof(Tilemap3DLayer)}(Size: {m_LayerSize}, Capacity: {Capacity}, Count: {Count})";

		public void SetTiles(Tile3DCoord[] tileCoordDatas)
		{
			foreach (var coordData in tileCoordDatas)
				this[coordData.Coord.x, coordData.Coord.z] = coordData.Tile;
		}
	}
}
