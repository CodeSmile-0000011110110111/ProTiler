// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

	[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
	public sealed partial class TileLayer : MonoBehaviour
	{
		private static TileSet s_ExampleTileSet;

		[Header("Layer Settings")]
		[SerializeField] private LayerType m_LayerType = LayerType.Tile;
		[SerializeField] private TileSet m_TileSet;
		[HideInInspector] [SerializeField] private TileDataContainer m_TileDataContainer = new();

		[Header("Debug")]
		[SerializeField] private bool m_DebugClearTilesButton;
		[ReadOnlyField] [SerializeField] private string m_DebugSelectedTileName;

		private int m_TileSetInstanceId;
		private TileLayerRenderer m_LayerRenderer;
		private TilePreviewRenderer m_PreviewRenderer;
		private TileBrush m_DrawBrush = new(GridCoord.zero, 0);
		private TileLayerRenderer LayerRenderer
		{
			get
			{
				if (m_LayerRenderer == null)
					m_LayerRenderer = gameObject.GetOrAddComponent<TileLayerRenderer>();
				return m_LayerRenderer;
			}
		}
		private TilePreviewRenderer PreviewRenderer
		{
			get
			{
				if (m_PreviewRenderer == null)
					m_PreviewRenderer = gameObject.GetOrAddComponent<TilePreviewRenderer>();
				return m_PreviewRenderer;
			}
		}

		public int TileCount { get => m_TileDataContainer.Count; }

		private void Reset() => OnEnable(); // Editor will not call OnEnable when layer added during Reset

		private void OnEnable()
		{
			if (m_LayerType != LayerType.Tile)
				throw new Exception($"layer type {m_LayerType} not supported");

			if (m_LayerRenderer == null)
				m_LayerRenderer = gameObject.GetOrAddComponent<TileLayerRenderer>();
			if (m_PreviewRenderer == null)
				m_PreviewRenderer = gameObject.GetOrAddComponent<TilePreviewRenderer>();
		}

		public override string ToString() => name;

		private TileSet GetExampleTileSet()
		{
			if (s_ExampleTileSet == null)
				s_ExampleTileSet = Resources.Load<TileSet>(Global.TileEditorResourceTileSetsPath + "ExampleTileSet");

			return s_ExampleTileSet;
		}

		public IDictionary<GridCoord, TileData> GetTilesInRect(GridRect rect) => m_TileDataContainer.GetTilesInRect(rect);

		public TileData GetTileData(GridCoord coord) => m_TileDataContainer.GetTile(coord);

		public float3 GetTilePosition(GridCoord coord) => Grid.ToWorldPosition(coord) + TileSet.GetTileOffset() + (float3)transform.position;

		public void DrawLine(GridCoord start, GridCoord end)
		{
			this.RecordUndoInEditor(m_DrawBrush.IsClearing ? "Clear Tiles" : "Draw Tiles");
			var coords = start.MakeLine(end);
			var tiles = m_TileDataContainer.SetTiles(coords, m_DrawBrush.TileSetIndex);
			this.SetDirtyInEditor();

			LayerRenderer.RedrawTiles(coords, tiles);
		}

		public void ClearAllTiles()
		{
			this.RecordUndoInEditor(nameof(ClearAllTiles));
			m_TileDataContainer.ClearAllTiles();
			this.SetDirtyInEditor();

			LayerRenderer.ForceRedraw();
		}

		public void SetTileFlags(GridCoord coord, TileFlags flags)
		{
			this.RecordUndoInEditor(nameof(SetTileFlags));
			var tileFlags = m_TileDataContainer.SetTileFlags(coord, flags);
			this.SetDirtyInEditor();

			LayerRenderer.UpdateTileFlagsAndRedraw(coord, tileFlags);
		}

		public void ClearTileFlags(GridCoord coord, TileFlags flags)
		{
			this.RecordUndoInEditor(nameof(ClearTileFlags));
			var tileFlags = m_TileDataContainer.ClearTileFlags(coord, flags);
			this.SetDirtyInEditor();

			LayerRenderer.UpdateTileFlagsAndRedraw(coord, tileFlags);
		}

		public void RotateTile(GridCoord coord, int delta)
		{
			this.RecordUndoInEditor(nameof(RotateTile));
			var tileFlags = m_TileDataContainer.RotateTile(coord, delta);
			this.SetDirtyInEditor();

			LayerRenderer.UpdateTileFlagsAndRedraw(coord, tileFlags);
		}

		public void FlipTile(GridCoord coord, int delta)
		{
			this.RecordUndoInEditor(nameof(FlipTile));
			var tileFlags = m_TileDataContainer.FlipTile(coord, delta);
			this.SetDirtyInEditor();

			LayerRenderer.UpdateTileFlagsAndRedraw(coord, tileFlags);
		}
	}
}