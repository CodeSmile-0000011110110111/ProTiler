// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Unity.Properties;
using LayerSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Data
{
	/// <summary>
	///     Represents a single, flat layer of tiles. Essentially a 2D array.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Tile3DLayer
	{
		[CreateProperty] private Tile3D[] m_Tiles;

		public Tile3D this[int index]
		{
			get => m_Tiles[index];
			set => m_Tiles[index] = value;
		}

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

		public Tile3DLayer(LayerSize size) => m_Tiles = new Tile3D[size.x * size.y];

		public void SetTiles(Tile3DCoord[] tileCoordDatas, int width)
		{
			foreach (var coordData in tileCoordDatas)
				this[Grid3DUtility.ToIndex2D(coordData.Coord.x, coordData.Coord.z, width)] = coordData.Tile;
		}

		[ExcludeFromCodeCoverage]
		public override string ToString() => $"{nameof(Tile3DLayer)}(Capacity: {Capacity}, Non-Empty: {Count})";
	}
}
