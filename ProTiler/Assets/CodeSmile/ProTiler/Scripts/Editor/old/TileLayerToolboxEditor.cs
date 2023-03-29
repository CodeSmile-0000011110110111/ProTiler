// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor.ProTiler.Extensions;
using CodeSmile.Extensions;
using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Data;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Editor.ProTiler
{
	[CustomEditor(typeof(TileLayerToolbox))]
	public partial class TileLayerToolboxEditor : UnityEditor.Editor
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
			var editMode = ProTilerState.instance.TileEditMode;
			ShowDrawBrush(m_IsMouseInView && editMode != TileEditMode.Selection);
		}

		private GridCoord GetMouseCursorCoord()
		{
			var gridSize = Toolbox.Layer.Grid.Size;

			return HandleUtilityExt.GUIPointToGridCoord
				(m_Input.MousePosition, new Vector3Int(gridSize.x, gridSize.y, gridSize.z), out var coord, Toolbox.transform.position.y)
				? new int3(coord.x, coord.y, coord.z) 
				: TileData.InvalidGridCoord;
		}

		public bool StartTileDrawing(TileEditMode editMode)
		{
			var useEvent = false;

			UpdateStartSelectionCoord();
			UpdateCursorCoord();

			if (editMode is TileEditMode.PenDraw or TileEditMode.RectFill)
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
			ShowDrawBrush(false);
		}

		private void UpdateClearingState()
		{
			if (m_IsDrawingTiles == false)
			{
				var shouldClear = m_Input.IsAltKeyDown || IsRightMouseButtonDown();
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

		private void UpdateSelectionRect()
		{
			var coordMin = math.min(m_StartSelectionCoord, m_CursorCoord);
			var coordMax = math.max(m_StartSelectionCoord, m_CursorCoord);
			m_SelectionRect = new GridRect(coordMin.x, coordMin.z, coordMax.x - coordMin.x + 1, coordMax.z - coordMin.z + 1);
		}

		private TileBrush CreateDrawBrush(bool clear) =>
			new(m_CursorCoord, clear ? TileData.InvalidTileSetIndex : ProTilerState.instance.DrawTileSetIndex);

		private void UpdateLayerDrawBrush()
		{
			var newBrush = CreateDrawBrush(m_IsClearingTiles);
			if (Toolbox.DrawBrush.Equals(newBrush) == false)
				Toolbox.DrawBrush = newBrush;
		}

		private void ShowDrawBrush(bool show = true) => Toolbox.TileDrawPreviewEnabled = show;
		private void OnLayout() => HandleUtilityExt.AddDefaultControl(GetHashCode());

		private void OnRepaint()
		{
			if (m_IsMouseInView && IsRightMouseButtonDown() == false)
				DrawCursorHandle(ProTilerState.instance.TileEditMode);
		}
	}
}