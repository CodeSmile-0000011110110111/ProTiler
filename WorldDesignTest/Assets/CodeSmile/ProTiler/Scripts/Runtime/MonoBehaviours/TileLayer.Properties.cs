// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Diagnostics.CodeAnalysis;

namespace CodeSmile.Tile
{
	public sealed partial class TileLayer
	{
		//public TileDataContainer TileDataContainer { get => m_TileDataContainer; set => m_TileDataContainer = value; }
		public TileSet TileSet
		{
			get
			{
				if (m_TileSet == null)
					m_TileSet = GetExampleTileSet();

				return m_TileSet;
			}
			set => m_TileSet = value;
		}
		
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public TileGrid Grid => TileSet.Grid;

		public LayerType LayerType { get => m_LayerType; set => m_LayerType = value; }
		public TileBrush DrawBrush
		{
			get => m_DrawBrush;
			set
			{
				m_DrawBrush = value;
				DebugSetTileName();
			}
		}
	}
}