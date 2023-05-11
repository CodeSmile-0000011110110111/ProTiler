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
				if (IsInitialized == false)
					return 0;

				var count = 0;
				for (var index = 0; index < m_Tiles.Length; index++)
					count += m_Tiles[index].IsEmpty ? 0 : 1;
				return count;
			}
		}

		public int Capacity => IsInitialized ? m_Tiles.Length : 0;
		public bool IsInitialized => m_Tiles != null;

		public Tile3DLayer(LayerSize size)
		{
			m_Tiles = null;
			AllocateTilesBuffer(size);
		}

		private void AllocateTilesBuffer(LayerSize size)
		{
			if (size.x < 0 || size.y < 0)
				throw new ArgumentException($"negative size is not allowed: {size}");

			var capacity = size.x * size.y;
			m_Tiles = capacity > 0 ? new Tile3D[capacity] : null;
		}

		public void SetTiles(Tile3DCoord[] tileCoords, int width)
		{
			foreach (var coordData in tileCoords)
				this[Grid3DUtility.ToIndex2D(coordData.Coord.x, coordData.Coord.z, width)] = coordData.Tile;
		}

		[ExcludeFromCodeCoverage]
		public override string ToString() => $"{nameof(Tile3DLayer)}(Capacity: {Capacity}, Non-Empty: {Count})";

		public void Resize(LayerSize size) => AllocateTilesBuffer(size);
	}
}
