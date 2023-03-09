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
	public sealed partial class TileLayer
	{
		[SerializeField] private float m_TileCursorHeight = 5f;
		[SerializeField] private TileGrid m_Grid = new(new GridSize(30, 1, 30));
		[SerializeField] private int m_TileSetIndex;
		[SerializeField] private TileSet m_TileSet = new();
		[HideInInspector] [SerializeField] private TileContainer m_TileContainer = new();
		[SerializeField] private TilePivot m_TilePivot;
		[SerializeField] private bool m_ClearTiles;
		[SerializeField] private int m_TileCount;
		public Action OnClearLayer;
		public Action<GridRect> OnSetTiles;

		public TilePivot TilePivot { get => m_TilePivot; set => m_TilePivot = value; }

		public TileGrid Grid { get => m_Grid; set => m_Grid = value; }
		public TileContainer TileContainer => m_TileContainer;
		public TileSet TileSet => m_TileSet;
		public float TileCursorHeight { get => m_TileCursorHeight; set => m_TileCursorHeight = value; }
		public int TileSetIndex
		{
			get => m_TileSetIndex;
			set
			{
				m_TileSetIndex = value;
				ClampTileSetIndex();
			}
		}
		private void ClampTileSetIndex() => m_TileSetIndex = Mathf.Clamp(m_TileSetIndex, 0, m_TileSet.Count - 1);

		private void UpdateTileCount() => m_TileCount = m_TileContainer.Count;

		public void SetTiles(GridRect gridSelection, bool clear = false)
		{
			var tile = clear ? null : m_TileSet.GetTile(m_TileSetIndex);
			var coords = m_TileContainer.SetTiles(gridSelection, tile);
			OnSetTiles?.Invoke(gridSelection);
			UpdateTileCount();
		}

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

		public void ClearTiles()
		{
			m_TileContainer.ClearTiles();
			UpdateTileCount();
			OnClearLayer?.Invoke();
		}

		public void SetTileFlags(GridCoord coord, TileFlags flags) => m_TileContainer.SetTileFlags(coord, flags);

		public void ClearTileFlags(GridCoord coord, TileFlags flags) => m_TileContainer.ClearTileFlags(coord, flags);
	}
}