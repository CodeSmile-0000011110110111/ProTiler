// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile;
using CodeSmile.Tile;
using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;

// ReSharper disable HeapView.ObjectAllocation

namespace CodeSmileEditor.Tile
{
	[CustomEditor(typeof(TileLayer))]
	[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
	public partial class TileLayerEditor : Editor
	{
		private GridCoord m_StartSelectionCoord;
		private GridCoord m_CursorCoord;
		private GridRect m_SelectionRect;
		private bool m_IsDrawingTiles;
		private bool m_IsClearingTiles;
		private bool m_IsMouseInView;
		private TilePreviewRenderer m_PreviewRenderer;

		private float2 MousePos => Event.current.mousePosition;
		private TileLayer Layer => (TileLayer)target;
		private TileGrid Grid => ((TileLayer)target).Grid;

		private int DrawTileSetIndex
		{
			get => TileEditorState.instance.DrawTileSetIndex;
			set => TileEditorState.instance.DrawTileSetIndex = math.clamp(value, 0, Layer.TileSet.Count - 1);
		}

		private void OnSceneGUI()
		{
			if (Selection.activeGameObject != Layer.gameObject)
				return;

			var editMode = TileEditorState.instance.TileEditMode;
			UpdatePreviewRenderer(editMode);
			HandleEventsAndInput(editMode);
		}

		private void UpdatePreviewRenderer(TileEditMode editMode)
		{
			if (m_PreviewRenderer == null)
				m_PreviewRenderer = Layer.GetComponent<TilePreviewRenderer>();
			if (m_PreviewRenderer != null)
				m_PreviewRenderer.ShowPreview = m_IsMouseInView && editMode != TileEditMode.Selection;
		}

		private void ChangeSelectedTileSetIndex(int delta)
		{
			DrawTileSetIndex += delta;
			UpdateLayerDrawBrush();
		}

		private TileBrush CreateDrawBrush(bool clear) =>
			new(m_CursorCoord, clear ? Global.InvalidTileSetIndex : TileEditorState.instance.DrawTileSetIndex);

		private void UpdateLayerDrawBrush() => Layer.DrawBrush = CreateDrawBrush(m_IsClearingTiles);
		private void HideLayerDrawBrush() => Layer.DrawBrush = CreateDrawBrush(true);

		private void StartTileDrawing(TileEditMode editMode)
		{
			UpdateStartSelectionCoord();
			UpdateCursorCoord();
			if (editMode == TileEditMode.PenDraw || editMode == TileEditMode.RectFill)
			{
				m_IsDrawingTiles = true;
				UpdateLayerDrawBrush();
				Event.current.Use();
			}
		}

		private void ContinueTileDrawing(TileEditMode editMode)
		{
			UpdateCursorCoord();

			if (m_IsDrawingTiles)
			{
				if (editMode == TileEditMode.PenDraw)
				{
					DrawLineFromStartToCursor();
					UpdateStartSelectionCoord();
					UpdateLayerDrawBrush();
					Event.current.Use();
				}
				else if (editMode == TileEditMode.RectFill)
				{
					UpdateLayerDrawBrush();
					Event.current.Use();
				}
			}
			else
				UpdateStartSelectionCoord();
		}

		private void FinishTileDrawing(TileEditMode editMode)
		{
			if (m_IsDrawingTiles)
			{
				UpdateCursorCoord();
				UpdateLayerDrawBrush();

				if (editMode == TileEditMode.PenDraw)
				{
					DrawLineFromStartToCursor();
					Event.current.Use();
				}
				else if (editMode == TileEditMode.RectFill)
				{
					DrawRectFromStartToCursor();
					Event.current.Use();
				}

				m_IsDrawingTiles = false;
				UpdateClearingState();
			}
		}

		private void CancelTileDrawing(TileEditMode editMode)
		{
			m_IsDrawingTiles = false;
			UpdateCursorCoord();
			UpdateStartSelectionCoord();
			HideLayerDrawBrush();
		}

		private void UpdateClearingState()
		{
			if (m_IsDrawingTiles == false)
			{
				var shouldClear = Event.current.shift || IsRightMouseButtonDown();
				if (m_IsClearingTiles != shouldClear)
				{
					m_IsClearingTiles = shouldClear;
					UpdateLayerDrawBrush();
				}
			}
		}

		private void DrawLineFromStartToCursor() => Layer.DrawLine(m_StartSelectionCoord, m_CursorCoord);
		private void DrawRectFromStartToCursor() => Layer.DrawRect(m_StartSelectionCoord.MakeRect(m_CursorCoord));

		private void UpdateStartSelectionCoord()
		{
			m_StartSelectionCoord = GetMouseCursorCoord();
			UpdateSelectionRect();
		}

		private void UpdateCursorCoord()
		{
			m_CursorCoord = GetMouseCursorCoord();
			UpdateSelectionRect();
		}

		private void UpdateSelectionRect() => m_SelectionRect = TileGrid.MakeRect(m_StartSelectionCoord, m_CursorCoord);

		private GridCoord GetMouseCursorCoord()
		{
			var planeY = Layer.transform.position.y;
			if (HandleUtilityExt.GUIPointToGridCoord(MousePos, Grid, out var coord, planeY))
				return coord;

			return Global.InvalidGridCoord;
		}

		private void DrawCursorHandle(TileEditMode editMode)
		{
			if (editMode == TileEditMode.PenDraw || editMode == TileEditMode.RectFill)
			{
				var worldRect = TileGrid.ToWorldRect(m_SelectionRect, Grid.Size);
				var worldPos = Layer.transform.position;
				var cubePos = worldRect.GetWorldCenter() + worldPos;
				var cubeSize = worldRect.GetWorldSize(Layer.Grid.Size.y);
				Handles.DrawWireCube(cubePos, cubeSize);
			}
		}

		private void ModifyTileAttributes(TileEditMode editMode)
		{
			var ev = Event.current;
			var delta = ev.delta.y >= 0 ? 1 : -1;

			if (editMode != TileEditMode.Selection)
			{
				if (ev.shift && ev.control)
				{
					Layer.FlipTile(m_CursorCoord, delta);
					ev.Use();
				}
				else if (ev.shift)
				{
					Layer.RotateTile(m_CursorCoord, delta);
					ev.Use();
				}
				else if (ev.control)
				{
					ChangeSelectedTileSetIndex(delta);
					ev.Use();
				}
			}
		}
	}
}