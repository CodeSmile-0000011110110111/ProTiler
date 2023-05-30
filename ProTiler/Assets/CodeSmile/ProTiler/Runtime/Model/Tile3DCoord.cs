// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;
using LayerCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Model
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

		public Tile3DCoord(GridCoord coord, Tile3D tile)
		{
			Coord = coord;
			Tile = tile;
		}

		public Boolean Equals(Tile3DCoord other) => Coord.Equals(other.Coord) && Tile.Equals(other.Tile);

		internal ChunkCoord GetChunkCoord(ChunkSize chunkSize) =>
			Tilemap3DUtility.GridToChunkCoord(Coord, chunkSize);

		internal LayerCoord GetLayerCoord(ChunkSize chunkSize) =>
			Tilemap3DUtility.GridToLayerCoord(Coord, chunkSize);

		[ExcludeFromCodeCoverage] public override String ToString() => $"{Coord}, {Tile}";

		public override Boolean Equals(Object obj) => obj is Tile3DCoord other && Equals(other);

		public override Int32 GetHashCode() => HashCode.Combine(Coord, Tile);
	}
}
