// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed class TileChunk
	{
		[NonSerialized] private TileWorld m_World;
		private TileGrid m_Grid = new();
		private List<TileLayer> m_Layers = new();
	}
}