// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int2;
using GridCoord = Unity.Mathematics.int3;
using LayerCoord = Unity.Mathematics.int3;

namespace CodeSmile.ProTiler3.Model
{
	/// <summary>
	///     TileData for a specific coordinate.
	/// </summary>
	[FullCovered]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Tile3DCoord : IEquatable<Tile3DCoord>
	{
		public GridCoord Coord;
		public Tile3D Tile;

		public static Boolean operator ==(Tile3DCoord left, Tile3DCoord right) => left.Equals(right);
		public static Boolean operator !=(Tile3DCoord left, Tile3DCoord right) => !left.Equals(right);

		public Tile3DCoord(GridCoord coord)
			: this(coord, new Tile3D()) {}

		public Tile3DCoord(GridCoord coord, Tile3D tile)
		{
			Coord = coord;
			Tile = tile;
		}

		public Boolean Equals(Tile3DCoord other) => Coord.Equals(other.Coord) && Tile.Equals(other.Tile);

		internal ChunkCoord GetChunkCoord(ChunkSize chunkSize) => Tilemap3DUtility.GridToChunkCoord(Coord, chunkSize);

		internal LayerCoord GetLayerCoord(ChunkSize chunkSize) => Tilemap3DUtility.GridToLayerCoord(Coord, chunkSize);

		[ExcludeFromCodeCoverage] public override String ToString() => $"Coord{Coord}, {Tile}";

		public override Boolean Equals(Object obj) => obj is Tile3DCoord other && Equals(other);

		public override Int32 GetHashCode() => HashCode.Combine(Coord, Tile);
	}
}
