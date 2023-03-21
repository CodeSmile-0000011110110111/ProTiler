// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	[Serializable]
	public struct TileData
	{
		[SerializeField] private int m_TileSetIndex;
		[SerializeField] private TileFlags m_Flags;

		public int TileSetIndex { get => m_TileSetIndex; set => m_TileSetIndex = math.max(0, value); }
		public TileFlags Flags { get => m_Flags; set => m_Flags = value; }

		public TileData(TileData tileData)
		{
			m_TileSetIndex = tileData.m_TileSetIndex;
			m_Flags = tileData.m_Flags;
		}

		public TileData(int tileSetIndex, TileFlags flags = TileFlags.None)
		{
			m_TileSetIndex = tileSetIndex;
			m_Flags = flags;
		}

		public override string ToString() => $"TD(#{m_TileSetIndex} | {m_Flags})";
	}
}