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
		private static readonly TileGrid DefaultGrid = new();

		[SerializeField] private string m_Name = "Layer";
		[SerializeField] private int m_SelectedTileSetIndex;
		[ReadOnlyField] [SerializeField] private string m_SelectedTileName;

		[SerializeField] private GridCoord m_CursorCoord;

		[SerializeField] private TileSet m_TileSet;
		[HideInInspector] [SerializeField] private TileContainer m_TileContainer = new();
		[SerializeField] private bool m_ClearTiles;
		[ReadOnlyField] [SerializeField] private int m_TileCount;

		[NonSerialized] private TileWorld m_TileWorld;

		public Action OnClearTiles;
		public Action<GridRect> OnSetTiles;
		public Action<GridCoord, TileFlags> OnSetTileFlags;
		public TileLayer(TileWorld world) => m_TileWorld = world;

		public override string ToString() => m_Name;
		public string Name => m_Name;
		public TileWorld TileWorld { get => m_TileWorld; internal set => m_TileWorld = value; }

		public TileContainer TileContainer { get => m_TileContainer; set => m_TileContainer = value; }
		//public TileSet TileSet => m_TileSet;
		public TileSet TileSet
		{
			get
			{
				if (m_TileSet == null)
					m_TileSet = ScriptableObject.CreateInstance(typeof(TileSet)) as TileSet;
				return m_TileSet;
			}
			set => throw new NotImplementedException();
		}
		public TileGrid Grid
		{
			get
			{
				if (m_TileSet != null)
					return m_TileSet.Grid;

				return DefaultGrid;
			}
			set => throw new NotImplementedException();
		}

		public float TileCursorHeight { get => TileSet.TileCursorHeight; set => TileSet.TileCursorHeight = value; }
		public int SelectedTileSetIndex
		{
			get => m_SelectedTileSetIndex;
			set
			{
				m_SelectedTileSetIndex = value;
				ClampTileSetIndex();
			}
		}
		public GridCoord CursorCoord { get => m_CursorCoord; set => m_CursorCoord = value; }
		public TileWorld World { get => m_TileWorld; set => m_TileWorld = value; }
		private void ClampTileSetIndex() => m_SelectedTileSetIndex = Mathf.Clamp(m_SelectedTileSetIndex, 0, TileSet.Count - 1);

		private void UpdateTileCount() => m_TileCount = m_TileContainer.Count;

		public float3 GetTilePosition(GridCoord coord) =>
			Grid.ToWorldPosition(coord) + TileSet.GetTileOffset() + (float3)m_TileWorld.transform.position;

		public void SetTiles(GridRect gridSelection, bool clear = false)
		{
			//var prefabTile = clear ? null : m_TileSet.GetPrefabIndex(m_TileSetIndex);
			var coords = m_TileContainer.SetTiles(gridSelection, clear ? -1 : m_SelectedTileSetIndex);
			OnSetTiles?.Invoke(gridSelection);
			UpdateTileCount();
		}

		public void SetTile(GridCoord coord, Tile tile)
		{
			m_TileContainer.SetTile(coord, tile);
			OnSetTiles?.Invoke(new GridRect(coord.ToCoord2d(), new Vector2Int(1, 1)));
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