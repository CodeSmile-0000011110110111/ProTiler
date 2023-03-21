// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
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

		private int m_TileSetInstanceId;
		private TileLayerRenderer m_LayerRenderer;
		private TilePreviewRenderer m_PreviewRenderer;

		// TODO: refactor
		//public Action<GridCoord, TileData> OnSetTile;
		//public Action OnClearAllTiles;
		//public Action<IReadOnlyList<GridCoord>, IReadOnlyList<TileData>> OnSetTiles;
		//public Action<GridRect> OnSetTilesInRect;
		//public Action<GridCoord, TileFlags> OnTileFlagsChanged;

		private void Reset() => OnEnable(); // Editor will not call OnEnable when layer added during Reset

		private void OnEnable()
		{
			if (m_LayerType == LayerType.Tile)
			{
				m_LayerRenderer = gameObject.GetOrAddComponent<TileLayerRenderer>();
				m_PreviewRenderer = gameObject.GetOrAddComponent<TilePreviewRenderer>();
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

		/*
		public void SetTiles(GridRect gridSelection, bool clear = false)
		{
			//var prefabTile = clear ? null : m_TileSet.GetPrefabIndex(m_TileSetIndex);
			var coords = m_TileDataContainer.SetTiles(gridSelection, clear ? -1 : m_DebugSelectedTileSetIndex);
			OnSetTiles?.Invoke(gridSelection);
			UpdateDebugTileCount();
		}
		*/

		public void DrawLine(GridCoord start, GridCoord end, int tileSetIndex)
		{
			this.RecordUndoInEditor(tileSetIndex < 0 ? "Clear Tiles" : "Draw Tiles");
			var coords = start.MakeLine(end);
			var tiles = m_TileDataContainer.SetTiles(coords, tileSetIndex);
			UpdateDebugTileCount();
			this.SetDirtyInEditor();

			m_LayerRenderer.RedrawTiles(coords, tiles);
		}
		
		/*public void DrawTile(GridCoord coord, int tileSetIndex)
		{
			this.RecordUndoInEditor(tileSetIndex < 0 ? "Clear Tile" : "Draw Tile");
			var tileData = m_TileDataContainer.SetTile(coord, tileSetIndex);
			UpdateDebugTileCount();
			
			this.SetDirtyInEditor();
			OnSetTile?.Invoke(coord, tileData);
		}*/

		public void ClearAllTiles()
		{
			this.RecordUndoInEditor(nameof(ClearAllTiles));
			m_TileDataContainer.ClearAllTiles();
			UpdateDebugTileCount();
			this.SetDirtyInEditor();

			m_LayerRenderer.ForceRedraw();
		}

		public void SetTileFlags(GridCoord coord, TileFlags flags)
		{
			this.RecordUndoInEditor(nameof(SetTileFlags));
			var tileFlags = m_TileDataContainer.SetTileFlags(coord, flags);
			this.SetDirtyInEditor();
			
			m_LayerRenderer.UpdateTileFlagsAndRedraw(coord, tileFlags);
		}

		public void ClearTileFlags(GridCoord coord, TileFlags flags)
		{
			this.RecordUndoInEditor(nameof(ClearTileFlags));
			var tileFlags = m_TileDataContainer.ClearTileFlags(coord, flags);
			this.SetDirtyInEditor();

			m_LayerRenderer.UpdateTileFlagsAndRedraw(coord, tileFlags);
		}

		public TileData GetTileData(GridCoord coord) => m_TileDataContainer.GetTile(coord);

		public float3 GetTilePosition(GridCoord coord)
		{
			var tileOffset = m_TileSet != null ? m_TileSet.GetTileOffset() : float3.zero;
			return Grid.ToWorldPosition(coord) + tileOffset + (float3)transform.position;
		}
	}
}