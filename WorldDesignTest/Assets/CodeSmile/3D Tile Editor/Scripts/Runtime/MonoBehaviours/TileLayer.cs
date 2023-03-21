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

	public sealed partial class TileLayer : MonoBehaviour
	{
		[Header("Layer Settings")]
		[SerializeField] private LayerType m_LayerType = LayerType.Tile;
		[SerializeField] private TileSet m_TileSet;
		[HideInInspector] [SerializeField] private TileDataContainer m_TileDataContainer = new();

		[Header("Debug")]
		[SerializeField] private bool m_DebugClearTilesButton;
		[SerializeField] private int m_DebugSelectedTileSetIndex;
		[ReadOnlyField] [SerializeField] private string m_DebugSelectedTileName;
		[ReadOnlyField] [SerializeField] private GridCoord m_DebugCursorCoord;
		[ReadOnlyField] [SerializeField] private int m_DebugTileCount;

		[NonSerialized] private int m_TileSetInstanceId;
		
		public Action<GridCoord, TileData> OnSetTile;

		// TODO: refactor
		public Action OnClearTiles;
		public Action<GridRect> OnSetTiles;
		public Action<GridCoord, TileFlags> OnTileFlagsChanged;

		private void OnEnable()
		{
			if (m_LayerType == LayerType.Tile)
			{
				gameObject.GetOrAddComponent<TileLayerRenderer>();
				gameObject.GetOrAddComponent<TilePreviewRenderer>();
			}
			else
				throw new Exception($"layer type {m_LayerType} not supported");
		}

		public override string ToString() => name;
		private void DebugClampTileSetIndex()
		{
			if (m_TileSet != null && m_TileSet.IsEmpty == false)
				m_DebugSelectedTileSetIndex = Mathf.Clamp(m_DebugSelectedTileSetIndex, 0, m_TileSet.Count - 1);
		}

		private void UpdateDebugTileCount() => m_DebugTileCount = m_TileDataContainer.Count;

		private void SetTileData(GridCoord coord, TileData tileData)
		{
			m_TileDataContainer.SetTile(coord, tileData);
			UpdateDebugTileCount();
		}

		/*
		public void SetTiles(GridRect gridSelection, bool clear = false)
		{
			//var prefabTile = clear ? null : m_TileSet.GetPrefabIndex(m_TileSetIndex);
			var coords = m_TileDataContainer.SetTiles(gridSelection, clear ? -1 : m_DebugSelectedTileSetIndex);
			OnSetTiles?.Invoke(gridSelection);
			UpdateDebugTileCount();
		}
		*/

		public void DrawTile(GridCoord coord)
		{
			var tileData = GetTileData(coord);
			if (tileData.TileSetIndex < 0)
				tileData = new TileData(m_DebugSelectedTileSetIndex);
			else
				tileData.TileSetIndex = m_DebugSelectedTileSetIndex;

			this.RecordUndoInEditor(nameof(DrawTile));
			SetTileData(coord, tileData);
			this.SetDirtyInEditor();
			
			OnSetTile?.Invoke(coord, tileData);
		}

		public void ClearTile(GridCoord coord)
		{
			this.RecordUndoInEditor(nameof(ClearTile));
			SetTileData(coord, Global.InvalidTileData);
			this.SetDirtyInEditor();

			OnSetTile?.Invoke(coord, Global.InvalidTileData);
		}

		public void ClearTiles()
		{
			this.RecordUndoInEditor(nameof(ClearTiles));
			m_TileDataContainer.ClearTiles();
			this.SetDirtyInEditor();

			OnClearTiles?.Invoke();
			UpdateDebugTileCount();
		}

		public void SetTileFlags(GridCoord coord, TileFlags flags)
		{
			this.RecordUndoInEditor(nameof(SetTileFlags));
			var tileFlags = m_TileDataContainer.SetTileFlags(coord, flags);
			this.SetDirtyInEditor();

			OnTileFlagsChanged?.Invoke(coord, tileFlags);
		}

		public void ClearTileFlags(GridCoord coord, TileFlags flags)
		{
			this.RecordUndoInEditor(nameof(ClearTileFlags));
			var tileFlags = m_TileDataContainer.ClearTileFlags(coord, flags);
			this.SetDirtyInEditor();

			OnTileFlagsChanged?.Invoke(coord, tileFlags);
		}

		public TileData GetTileData(GridCoord coord) => m_TileDataContainer.GetTile(coord);

		public float3 GetTilePosition(GridCoord coord)
		{
			var tileOffset = m_TileSet != null ? m_TileSet.GetTileOffset() : float3.zero;
			return Grid.ToWorldPosition(coord) + tileOffset + (float3)transform.position;
		}
	}
}