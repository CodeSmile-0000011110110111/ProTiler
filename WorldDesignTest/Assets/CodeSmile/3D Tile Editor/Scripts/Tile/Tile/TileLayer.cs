// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;

namespace CodeSmile.Tile
{
	[Serializable]
	public sealed partial class TileLayer : IEnumerable<KeyValuePair<GridCoord, Tile>>
	{
		//[NonSerialized] private TileWorld m_World;
		[SerializeField] private float m_TileCursorHeight = 5f;
		[SerializeField] private TileGrid m_Grid;
		[SerializeField] private int m_ActiveTileSetIndex;
		[SerializeField] private TileSet m_TileSet = new();
		[HideInInspector][SerializeField] private TileContainer m_TileContainer = new();
		[SerializeField] private TilePivot m_TilePivot;
		[SerializeField] private bool m_ClearTiles;
		[SerializeField] private int m_TileCount;

		public IEnumerator<KeyValuePair<GridCoord, Tile>> GetEnumerator() => m_TileContainer.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public TilePivot TilePivot { get => m_TilePivot; set => m_TilePivot = value; }

		public TileGrid Grid { get => m_Grid; set => m_Grid = value; }
		public TileContainer TileContainer => m_TileContainer;
		public TileSet TileSet => m_TileSet;
		public float TileCursorHeight { get => m_TileCursorHeight; set => m_TileCursorHeight = value; }

		public void SetTiles(GridRect gridSelection) => m_TileContainer.SetTiles(gridSelection, m_TileSet.GetTile(m_ActiveTileSetIndex));

		public float3 GetTileOffset()
		{
			var gridSize = m_Grid.Size;
			return m_TilePivot switch
			{
				TilePivot.Center => new float3(gridSize.x * .5f, gridSize.y * .5f, gridSize.z * .5f),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		public float3 GetTileWorldPosition(GridCoord coord) => m_Grid.ToWorldPosition(coord) + GetTileOffset();

		public void ClearTiles() => m_TileContainer.Clear();
	}
}