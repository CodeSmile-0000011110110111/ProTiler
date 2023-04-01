// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.ProTiler.Data
{
	[Serializable]
	public struct TileData : IEquatable<TileData>
	{
		public const int InvalidTileSetIndex = -1;
		
		[SerializeField] private int m_TileSetIndex;
		[SerializeField] private TileFlagsOld m_Flags;

		public int TileSetIndex { get => m_TileSetIndex; set => m_TileSetIndex = math.max(InvalidTileSetIndex, value); }
		public TileFlagsOld Flags { get => m_Flags; set => m_Flags = value; }
		public bool IsValid => m_TileSetIndex >= 0;
		public bool IsInvalid => m_TileSetIndex < 0;

		public static bool operator ==(TileData left, TileData right) => left.Equals(right);
		public static bool operator !=(TileData left, TileData right) => !left.Equals(right);

		public TileData(TileData tileData)
		{
			m_TileSetIndex = tileData.m_TileSetIndex;
			m_Flags = tileData.m_Flags;
			EnsureRotationIsSet();
		}

		public TileData(int tileSetIndex, TileFlagsOld flags = TileFlagsOld.None)
		{
			m_TileSetIndex = tileSetIndex;
			m_Flags = flags;
			EnsureRotationIsSet();
		}

		public bool Equals(TileData other) => m_TileSetIndex == other.m_TileSetIndex && m_Flags == other.m_Flags;

		private void EnsureRotationIsSet()
		{
			if ((m_Flags & TileFlagsOld.AllDirections) == TileFlagsOld.None)
				m_Flags |= TileFlagsOld.DirectionNorth;
		}

		public TileFlagsOld Rotate(int delta)
		{
			EnsureRotationIsSet();
			var direction = m_Flags & TileFlagsOld.AllDirections;
			m_Flags &= ~TileFlagsOld.AllDirections;

			var newDirection = direction switch
			{
				TileFlagsOld.DirectionNorth => delta < 0 ? TileFlagsOld.DirectionWest : TileFlagsOld.DirectionEast,
				TileFlagsOld.DirectionWest => delta < 0 ? TileFlagsOld.DirectionSouth : TileFlagsOld.DirectionNorth,
				TileFlagsOld.DirectionSouth => delta < 0 ? TileFlagsOld.DirectionEast : TileFlagsOld.DirectionWest,
				TileFlagsOld.DirectionEast => delta < 0 ? TileFlagsOld.DirectionNorth : TileFlagsOld.DirectionSouth,
				_ => TileFlagsOld.None
			};
			m_Flags |= newDirection;
			return newDirection;
		}

		public TileFlagsOld Flip(int delta)
		{
			var flip = m_Flags & TileFlagsOld.AllFlips;
			m_Flags &= ~TileFlagsOld.AllFlips;

			var newFlip = flip switch
			{
				TileFlagsOld.None => delta < 0 ? TileFlagsOld.FlipHorizontal : TileFlagsOld.FlipVertical,
				TileFlagsOld.FlipHorizontal => delta < 0 ? TileFlagsOld.AllFlips : TileFlagsOld.None,
				TileFlagsOld.AllFlips => delta < 0 ? TileFlagsOld.FlipVertical : TileFlagsOld.FlipHorizontal,
				TileFlagsOld.FlipVertical => delta < 0 ? TileFlagsOld.None : TileFlagsOld.AllFlips,
				_ => TileFlagsOld.None
			};
			m_Flags |= newFlip;
			return newFlip;
		}

		public override string ToString() => $"T(#{m_TileSetIndex} | {m_Flags})";

		public TileFlagsOld ClearFlags(TileFlagsOld flags)
		{
			m_Flags &= ~flags;
			return m_Flags;
		}

		public TileFlagsOld SetFlags(TileFlagsOld flags)
		{
			m_Flags |= flags;
			return m_Flags;
		}

		public override bool Equals(object obj) => obj is TileData other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(m_TileSetIndex, (int)m_Flags);
		public static readonly TileData InvalidTileData = new(TileData.InvalidTileSetIndex);
		public static readonly GridCoord InvalidGridCoord = new(int.MinValue, int.MinValue, int.MinValue);
	}
}
