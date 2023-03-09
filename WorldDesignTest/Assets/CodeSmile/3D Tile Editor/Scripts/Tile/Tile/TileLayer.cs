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

		public Action OnClearTiles;
		public Action<GridRect> OnSetTiles;
		public Action<GridCoord, TileFlags> OnSetTileFlags;

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

		public void SetTiles(GridRect gridSelection, bool clear = false)
		{
			//var prefabTile = clear ? null : m_TileSet.GetPrefabIndex(m_TileSetIndex);
			var coords = m_TileContainer.SetTiles(gridSelection, clear ? -1 : m_TileSetIndex);
			OnSetTiles?.Invoke(gridSelection);
			UpdateTileCount();
		}

		public void SetTile(GridCoord coord, Tile tile)
		{
			m_TileContainer.SetTile(coord, tile);
			OnSetTiles?.Invoke(new GridRect(new Vector2Int(coord.x, coord.z), new Vector2Int(1, 1)));
		}

		public void ClearTiles()
		{
			m_TileContainer.ClearTiles();
			UpdateTileCount();
			OnClearTiles?.Invoke();
		}

		public void SetTileFlags(GridCoord coord, TileFlags flags)
		{
			var tileFlags = m_TileContainer.SetTileFlags(coord, flags);
			OnSetTileFlags?.Invoke(coord, tileFlags);
		}

		public void ClearTileFlags(GridCoord coord, TileFlags flags)
		{
			var tileFlags = m_TileContainer.ClearTileFlags(coord, flags);
			OnSetTileFlags?.Invoke(coord, tileFlags);
		}

		public Tile GetTile(GridCoord coord) => m_TileContainer.GetTile(coord);
	}
}