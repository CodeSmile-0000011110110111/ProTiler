﻿// Copyright (C) 2021-2023 Steffen Itterheim
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
		[HideInInspector] [SerializeField] private TileDataContainer m_TileDataContainer = new();

		[Header("Debug")]
		[SerializeField] private bool m_ClearTilesButton;
		[SerializeField] private int m_DebugSelectedTileSetIndex;
		[ReadOnlyField] [SerializeField] private string m_DebugSelectedTileName;
		[ReadOnlyField] [SerializeField] private GridCoord m_DebugCursorCoord;
		[ReadOnlyField] [SerializeField] private int m_DebugTileCount;

		[NonSerialized] private TileWorld m_TileWorld;

		public Action<GridCoord, TileData> OnSetTile;
		
		// TODO: refactor
		public Action OnClearTiles;
		public Action<GridRect> OnSetTiles;
		public Action<GridCoord, TileFlags> OnSetTileFlags;
		
		public string Name => m_Name;
		public TileWorld TileWorld { get => m_TileWorld; internal set => m_TileWorld = value; }

		public TileDataContainer TileDataContainer { get => m_TileDataContainer; set => m_TileDataContainer = value; }
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

		private void UpdateDebugTileCount() => m_DebugTileCount = m_TileDataContainer.Count;

		public float3 GetTilePosition(GridCoord coord) =>
			Grid.ToWorldPosition(coord) + TileSet.GetTileOffset() + (float3)m_TileWorld.transform.position;

		public void SetTiles(GridRect gridSelection, bool clear = false)
		{
			//var prefabTile = clear ? null : m_TileSet.GetPrefabIndex(m_TileSetIndex);
			var coords = m_TileDataContainer.SetTiles(gridSelection, clear ? -1 : m_DebugSelectedTileSetIndex);
			OnSetTiles?.Invoke(gridSelection);
			UpdateDebugTileCount();
		}

		public void DrawTile(GridCoord coord)
		{
			var tile = GetTile(coord);
			if (tile.TileSetIndex < 0)
				tile = new TileData(m_DebugSelectedTileSetIndex);
			else
				tile.TileSetIndex = m_DebugSelectedTileSetIndex;

			SetTile(coord, tile);
		}

		public void SetTile(GridCoord coord, TileData tileData)
		{
			m_TileDataContainer.SetTile(coord, tileData);
			OnSetTile?.Invoke(coord, tileData);
			UpdateDebugTileCount();
		}

		public void ClearTile(GridCoord coord) => SetTile(coord, Global.InvalidTileData);

		public void ClearTiles()
		{
			m_TileDataContainer.ClearTiles();
			OnClearTiles?.Invoke();
			UpdateDebugTileCount();
		}

		public void SetTileFlags(GridCoord coord, TileFlags flags)
		{
			var tileFlags = m_TileDataContainer.SetTileFlags(coord, flags);
			OnSetTileFlags?.Invoke(coord, tileFlags);
		}

		public void ClearTileFlags(GridCoord coord, TileFlags flags)
		{
			var tileFlags = m_TileDataContainer.ClearTileFlags(coord, flags);
			OnSetTileFlags?.Invoke(coord, tileFlags);
		}

		public TileData GetTile(GridCoord coord) => m_TileDataContainer.GetTile(coord);
	}
}