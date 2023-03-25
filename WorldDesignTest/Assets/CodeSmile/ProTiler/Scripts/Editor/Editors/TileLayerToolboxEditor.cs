// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tile;
using UnityEditor;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmileEditor.Tile
{
	[CustomEditor(typeof(TileLayerToolbox))]
	public partial class TileLayerToolboxEditor : Editor
	{
		private bool m_IsMouseInView;

		private GridCoord m_StartSelectionCoord;
		private GridCoord m_CursorCoord;
		private GridRect m_SelectionRect;
		private bool m_IsDrawingTiles;
		private bool m_IsClearingTiles;
		private TileLayerToolbox Toolbox => (TileLayerToolbox)target;

		private void OnEnable() => RegisterInputEvents();

		private void OnDisable() => UnregisterInputEvents();

		public void OnSceneGUI()
		{
			if (Selection.activeGameObject != Toolbox.gameObject)
				return;

			UpdateTileDrawPreviewEnabledState();
			UpdateInputStates();

			switch (Event.current.type)
			{
				case EventType.Layout:
					OnLayout();
					break;
				case EventType.MouseMove:
					OnMouseMove();
					break;
				case EventType.MouseDrag:
					OnMouseDrag();
					break;
				case EventType.MouseEnterWindow:
					OnMouseEnterWindow();
					break;
				case EventType.MouseLeaveWindow:
					OnMouseLeaveWindow();
					break;
				case EventType.Repaint:
					OnRepaint();
					break;
			}

			UpdateClearingState();
			UpdateLayerDrawBrush();
		}

		private void UpdateTileDrawPreviewEnabledState()
		{
			var editMode = TileEditorState.instance.TileEditMode;
			Toolbox.TileDrawPreviewEnabled = m_IsMouseInView && editMode != TileEditMode.Selection;
		}

		private GridCoord GetMouseCursorCoord()
		{
			var planeY = Toolbox.transform.position.y;
			if (HandleUtilityExt.GUIPointToGridCoord(m_Input.MousePosition, Toolbox.Layer.Grid, out var coord, planeY))
				return coord;

			return Const.InvalidGridCoord;
		}

		public bool StartTileDrawing(TileEditMode editMode)
		{
			var useEvent = false;

			UpdateStartSelectionCoord();
			UpdateCursorCoord();

			if (editMode == TileEditMode.PenDraw || editMode == TileEditMode.RectFill)
			{
				m_IsDrawingTiles = true;
				useEvent = true;
			}

			return useEvent;
		}

		private bool ContinueTileDrawing(TileEditMode editMode)
		{
			var useEvent = false;

			UpdateCursorCoord();
			if (m_IsDrawingTiles)
			{
				if (editMode == TileEditMode.PenDraw)
				{
					DrawLineFromStartToCursor();
					UpdateStartSelectionCoord();
					useEvent = true;
				}
				else if (editMode == TileEditMode.RectFill)
					useEvent = true;
			}
			else
				UpdateStartSelectionCoord();

			return useEvent;
		}

		private bool FinishTileDrawing(TileEditMode editMode)
		{
			var useEvent = false;

			if (m_IsDrawingTiles)
			{
				UpdateCursorCoord();

				if (editMode == TileEditMode.PenDraw)
				{
					DrawLineFromStartToCursor();
					useEvent = true;
				}
				else if (editMode == TileEditMode.RectFill)
				{
					DrawRectFromStartToCursor();
					useEvent = true;
				}

				m_IsDrawingTiles = false;
			}

			return useEvent;
		}

		private void CancelTileDrawing()
		{
			m_IsDrawingTiles = false;
			UpdateCursorCoord();
			UpdateStartSelectionCoord();
			SetClearDrawBrush();
		}

		private void UpdateClearingState()
		{
			if (m_IsDrawingTiles == false)
			{
				var shouldClear = Event.current.shift || IsRightMouseButtonDown();
				if (m_IsClearingTiles != shouldClear)
					m_IsClearingTiles = shouldClear;
			}
		}

		private void DrawLineFromStartToCursor() => Toolbox.DrawLine(m_StartSelectionCoord, m_CursorCoord);
		private void DrawRectFromStartToCursor() => Toolbox.DrawRect(m_StartSelectionCoord.MakeRect(m_CursorCoord));

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

		private TileBrush CreateDrawBrush(bool clear) =>
			new(m_CursorCoord, clear ? Const.InvalidTileSetIndex : TileEditorState.instance.DrawTileSetIndex);

		private void UpdateLayerDrawBrush()
		{
			var newBrush = CreateDrawBrush(m_IsClearingTiles);
			if (Toolbox.DrawBrush.Equals(newBrush) == false)
				Toolbox.DrawBrush = newBrush;
		}

		private void SetClearDrawBrush() => Toolbox.DrawBrush = CreateDrawBrush(true);
	}
}