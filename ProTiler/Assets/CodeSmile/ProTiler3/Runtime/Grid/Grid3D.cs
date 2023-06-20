// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using ChunkSize = UnityEngine.Vector2Int;
using CellSize = UnityEngine.Vector3;
using CellGap = UnityEngine.Vector3;

namespace CodeSmile.ProTiler3.Grid
{
	[Serializable]
	public struct Grid3D : IEquatable<Grid3D>
	{
		public ChunkSize ChunkSize;
		public CellSize CellSize;
		public CellGap CellGap;
		public CellLayout CellLayout;

		public static Boolean operator ==(Grid3D left, Grid3D right) => left.Equals(right);
		public static Boolean operator !=(Grid3D left, Grid3D right) => !left.Equals(right);

		public Grid3D(ChunkSize chunkSize, CellSize cellSize, CellGap cellGap, CellLayout cellLayout)
		{
			ChunkSize = chunkSize;
			CellSize = cellSize;
			CellGap = cellGap;
			CellLayout = cellLayout;
		}

		public Boolean Equals(Grid3D other) => ChunkSize.Equals(other.ChunkSize) && CellSize.Equals(other.CellSize) &&
		                                       CellGap.Equals(other.CellGap) && CellLayout == other.CellLayout;

		public override Boolean Equals(Object obj) => obj is Grid3D other && Equals(other);

		public override Int32 GetHashCode() => HashCode.Combine(ChunkSize, CellSize, CellGap, (Int32)CellLayout);
	}
}
