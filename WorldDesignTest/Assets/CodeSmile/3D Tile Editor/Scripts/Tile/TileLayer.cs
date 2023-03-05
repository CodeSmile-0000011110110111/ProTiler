// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed partial class TileLayer : IEnumerable<KeyValuePair<TileGridCoord, Tile>>
	{
		//[NonSerialized] private TileWorld m_World;
		[SerializeField] private float m_TileCursorHeight = 5f;
		[SerializeField] private TileGrid m_Grid;
		[SerializeField] private int m_ActiveTileSetIndex;
		[SerializeField] private TileSet m_TileSet = new();
		[SerializeField] private TileContainer m_TileContainer = new();
		[SerializeField] private TilePivot m_TilePivot;
		
		public IEnumerator<KeyValuePair<TileGridCoord, Tile>> GetEnumerator() => m_TileContainer.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public TilePivot TilePivot { get => m_TilePivot; set => m_TilePivot = value; }

		public TileGrid Grid { get => m_Grid; set => m_Grid = value; }
		public TileContainer TileContainer => m_TileContainer;
		public TileSet TileSet => m_TileSet;
		public float TileCursorHeight { get => m_TileCursorHeight; set => m_TileCursorHeight = value; }

		public void SetTileAt(TileGridCoord coord) => m_TileContainer.SetTileAt(coord, m_TileSet.GetTile(m_ActiveTileSetIndex));

		public Vector3 GetTileOffset()
		{
			var gridSize = m_Grid.Size;
			return m_TilePivot switch
			{
				TilePivot.Center => new Vector3(gridSize.x * .5f, gridSize.y * .5f, gridSize.z * .5f),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public Vector3 GetTileWorldPosition(TileGridCoord coord)
		{
			var gridSize = m_Grid.Size;
			return new Vector3(coord.x * gridSize.x, coord.y * gridSize.y, coord.z * gridSize.z) + GetTileOffset();
		}
	}
}