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
		[SerializeField] private TileFlags m_Flags;

		public int TileSetIndex { get => m_TileSetIndex; set => m_TileSetIndex = math.max(InvalidTileSetIndex, value); }
		public TileFlags Flags { get => m_Flags; set => m_Flags = value; }
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

		public TileData(int tileSetIndex, TileFlags flags = TileFlags.None)
		{
			m_TileSetIndex = tileSetIndex;
			m_Flags = flags;
			EnsureRotationIsSet();
		}

		public bool Equals(TileData other) => m_TileSetIndex == other.m_TileSetIndex && m_Flags == other.m_Flags;

		private void EnsureRotationIsSet()
		{
			if ((m_Flags & TileFlags.AllDirections) == TileFlags.None)
				m_Flags |= TileFlags.DirectionNorth;
		}

		public TileFlags Rotate(int delta)
		{
			EnsureRotationIsSet();
			var direction = m_Flags & TileFlags.AllDirections;
			m_Flags &= ~TileFlags.AllDirections;

			var newDirection = direction switch
			{
				TileFlags.DirectionNorth => delta < 0 ? TileFlags.DirectionWest : TileFlags.DirectionEast,
				TileFlags.DirectionWest => delta < 0 ? TileFlags.DirectionSouth : TileFlags.DirectionNorth,
				TileFlags.DirectionSouth => delta < 0 ? TileFlags.DirectionEast : TileFlags.DirectionWest,
				TileFlags.DirectionEast => delta < 0 ? TileFlags.DirectionNorth : TileFlags.DirectionSouth,
				_ => TileFlags.None
			};
			m_Flags |= newDirection;
			return newDirection;
		}

		public TileFlags Flip(int delta)
		{
			var flip = m_Flags & TileFlags.AllFlips;
			m_Flags &= ~TileFlags.AllFlips;

			var newFlip = flip switch
			{
				TileFlags.None => delta < 0 ? TileFlags.FlipHorizontal : TileFlags.FlipVertical,
				TileFlags.FlipHorizontal => delta < 0 ? TileFlags.AllFlips : TileFlags.None,
				TileFlags.AllFlips => delta < 0 ? TileFlags.FlipVertical : TileFlags.FlipHorizontal,
				TileFlags.FlipVertical => delta < 0 ? TileFlags.None : TileFlags.AllFlips,
				_ => TileFlags.None
			};
			m_Flags |= newFlip;
			return newFlip;
		}

		public override string ToString() => $"T(#{m_TileSetIndex} | {m_Flags})";

		public TileFlags ClearFlags(TileFlags flags)
		{
			m_Flags &= ~flags;
			return m_Flags;
		}

		public TileFlags SetFlags(TileFlags flags)
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