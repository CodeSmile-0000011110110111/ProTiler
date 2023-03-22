// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.Tile
{
	public sealed partial class TileLayer
	{
		public TileDataContainer TileDataContainer { get => m_TileDataContainer; set => m_TileDataContainer = value; }
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
		public TileGrid Grid
		{
			get
			{
				if (m_TileSet != null)
					return m_TileSet.Grid;

				return Global.DefaultGrid;
			}
			set => throw new NotImplementedException();
		}
		public float TileCursorHeight
		{
			get => m_TileSet != null ? m_TileSet.TileCursorHeight : 1;
			set
			{
				if (m_TileSet != null)
					m_TileSet.TileCursorHeight = value;
			}
		}

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