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
	public sealed class Tile
	{
		//[SerializeField] private GridCoord m_Coord;
		[SerializeField] private int m_TileSetIndex;
		[SerializeField] private TileFlags m_Flags;

		public Tile(Tile tile)
		{
			if (tile != null)
			{
				m_TileSetIndex = tile.m_TileSetIndex;
				m_Flags = tile.m_Flags;
			}
		}

		public Tile(int tileSetIndex, TileFlags flags = TileFlags.None)
		{
			//m_Coord = coord;
			m_TileSetIndex = tileSetIndex;
			m_Flags = flags;
		}

		public int TileSetIndex { get => m_TileSetIndex; set => m_TileSetIndex = math.max(0, value); }
		public TileFlags Flags { get => m_Flags; set => m_Flags = value; }

		public override string ToString() => $"Tile Index #{m_TileSetIndex}, Flags: {m_Flags}";
	}
}