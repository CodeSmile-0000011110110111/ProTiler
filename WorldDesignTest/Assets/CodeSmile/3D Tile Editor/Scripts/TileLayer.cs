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
	public enum LayerType
	{
		Tile,
		Terrain,
		Object,
		Data,
	}

	[Serializable]
	public sealed partial class TileLayer
	{
		[Header("Layer Settings")]
		[SerializeField] private string m_Name = "Layer";
		[SerializeField] private LayerType m_LayerType = LayerType.Tile;
		[SerializeField] private TileSet m_TileSet;
		[HideInInspector] [SerializeField] private TileContainer m_TileContainer = new();

		[Header("Debug")]
		[SerializeField] private bool m_ClearTilesButton;
		[SerializeField] private int m_DebugSelectedTileSetIndex;
		[ReadOnlyField] [SerializeField] private string m_DebugSelectedTileName;
		[ReadOnlyField] [SerializeField] private GridCoord m_DebugCursorCoord;
		[ReadOnlyField] [SerializeField] private int m_DebugTileCount;

		[NonSerialized] private TileWorld m_TileWorld;

		public Action<GridCoord, Tile> OnSetTile;
		
		// TODO: refactor
		public Action OnClearTiles;
		public Action<GridRect> OnSetTiles;
		public Action<GridCoord, TileFlags> OnSetTileFlags;
		
		public string Name => m_Name;
		public TileWorld TileWorld { get => m_TileWorld; internal set => m_TileWorld = value; }

		public TileContainer TileContainer { get => m_TileContainer; set => m_TileContainer = value; }
		public TileSet TileSet
		{
			get
			{
				if (m_TileSet == null)
					m_TileSet = ScriptableObject.CreateInstance<TileSet>();
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

		public float TileCursorHeight { get => TileSet.TileCursorHeight; set => TileSet.TileCursorHeight = value; }
		public int SelectedTileSetIndex
		{
			get => m_DebugSelectedTileSetIndex;
			set
			{
				m_DebugSelectedTileSetIndex = value;
				ClampTileSetIndex();
			}
		}
		public GridCoord DebugCursorCoord { get => m_DebugCursorCoord; set => m_DebugCursorCoord = value; }
		public TileWorld World { get => m_TileWorld; set => m_TileWorld = value; }
		public LayerType LayerType { get => m_LayerType; set => m_LayerType = value; }
		public TileLayer(TileWorld world) => m_TileWorld = world;

		public override string ToString() => m_Name;
		private void ClampTileSetIndex() => m_DebugSelectedTileSetIndex = Mathf.Clamp(m_DebugSelectedTileSetIndex, 0, TileSet.Count - 1);

		private void DebugUpdateTileCount() => m_DebugTileCount = m_TileContainer.Count;

		public float3 GetTilePosition(GridCoord coord) =>
			Grid.ToWorldPosition(coord) + TileSet.GetTileOffset() + (float3)m_TileWorld.transform.position;

		public void SetTiles(GridRect gridSelection, bool clear = false)
		{
			//var prefabTile = clear ? null : m_TileSet.GetPrefabIndex(m_TileSetIndex);
			var coords = m_TileContainer.SetTiles(gridSelection, clear ? -1 : m_DebugSelectedTileSetIndex);
			OnSetTiles?.Invoke(gridSelection);
			DebugUpdateTileCount();
		}

		public void DrawTile(GridCoord coord)
		{
			var tile = GetTile(coord);
			if (tile != null)
				tile.TileSetIndex = m_DebugSelectedTileSetIndex;
			else
				tile = new Tile(m_DebugSelectedTileSetIndex);

			SetTile(coord, tile);
		}

		public void SetTile(GridCoord coord, Tile tile)
		{
			m_TileContainer.SetTile(coord, tile);
			OnSetTile?.Invoke(coord, tile);
		}

		public void ClearTile(GridCoord coord) => SetTile(coord, null);

		public void ClearTiles()
		{
			m_TileContainer.ClearTiles();
			DebugUpdateTileCount();
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