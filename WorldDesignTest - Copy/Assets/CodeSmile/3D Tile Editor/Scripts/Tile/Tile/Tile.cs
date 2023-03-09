// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed class Tile
	{
		[SerializeField] private int m_TileSetIndex;
		[SerializeField] private TileFlags m_Flags;

		public Tile(int tileSetIndex, TileFlags flags = 0) => m_TileSetIndex = tileSetIndex;

		public int TileSetIndex { get => m_TileSetIndex; set => m_TileSetIndex = value; }
		public TileFlags Flags { get => m_Flags; set => m_Flags = value; }

		public override string ToString() => $"TileSet Index #{m_TileSetIndex}, Flags: {m_Flags}";
	}
}