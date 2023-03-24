// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmileEditor.Tile
{
	public partial class TileLayerEditor
	{
		private readonly EditorInputState m_InputState = new();

		private void HandleEventsAndInput(TileEditMode editMode)
		{
			m_InputState.Update();

			switch (Event.current.type)
			{
				case EventType.Layout:
					OnLayout();
					break;
				case EventType.MouseMove:
					OnMouseMove(editMode);
					break;
				case EventType.MouseDown:
					OnMouseDown(editMode);
					break;
				case EventType.MouseDrag:
					OnMouseDrag(editMode);
					break;
				case EventType.MouseUp:
					OnMouseUp(editMode);
					break;
				case EventType.ScrollWheel:
					OnScrollWheel(editMode);
					break;
				case EventType.KeyDown:
					OnKeyDown(editMode);
					break;
				case EventType.KeyUp:
					OnKeyUp(editMode);
					break;
				case EventType.TouchMove:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.TouchEnter:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.TouchLeave:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.TouchStationary:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.DragUpdated:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.DragPerform:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.DragExited:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.Ignore:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.Used:
					//Debug.Log($"{Event.current.type}");
					break;
				case EventType.ValidateCommand:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.ExecuteCommand:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.ContextClick:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.MouseEnterWindow:
					OnMouseEnterWindow(editMode);
					break;
				case EventType.MouseLeaveWindow:
					OnMouseLeaveWindow(editMode);
					break;
				case EventType.Repaint:
					OnRepaint(editMode);
					break;
			}
		}

		private void OnKeyDown(TileEditMode editMode)
		{
			if (Event.current.keyCode == KeyCode.Escape)
				CancelTileDrawing(editMode);

			UpdateClearingState();
		}

		private void OnKeyUp(TileEditMode editMode) => UpdateClearingState();
		private void OnMouseEnterWindow(TileEditMode editMode) => m_IsMouseInView = true;

		private void OnMouseLeaveWindow(TileEditMode editMode)
		{
			m_IsMouseInView = false;
			FinishTileDrawing(editMode);
		}

		private void OnMouseMove(TileEditMode editMode)
		{
			// Note: MouseMove and MouseDrag are mutually exclusive! With button down only MouseDrag event is sent.
			// We do not need to check for mouse button down here since they can be assumed "up" at all times in MouseMove.
			UpdateStartSelectionCoord();
			UpdateCursorCoord();
			if (editMode != TileEditMode.Selection)
				UpdateLayerDrawBrush();
		}

		private void OnMouseDown(TileEditMode editMode)
		{
			if (IsRightMouseButtonDown())
				CancelTileDrawing(editMode);
			else if (IsOnlyLeftMouseButtonDown())
				StartTileDrawing(editMode);
		}

		private void OnMouseDrag(TileEditMode editMode)
		{
			if (IsRightMouseButtonDown())
				CancelTileDrawing(editMode);
			else if (IsOnlyLeftMouseButtonDown())
				ContinueTileDrawing(editMode);
		}

		private void OnMouseUp(TileEditMode editMode)
		{
			if (WasRightMouseButtonDown())
				CancelTileDrawing(editMode);
			else if (WasLeftMouseButtonDown())
				FinishTileDrawing(editMode);
		}

		private void OnScrollWheel(TileEditMode editMode) => ModifyTileAttributes(editMode);

		private void OnLayout() => HandleUtilityExt.AddDefaultControl(GetHashCode());

		private void OnRepaint(TileEditMode editMode)
		{
			if (m_IsMouseInView && IsRightMouseButtonDown() == false)
				DrawCursorHandle(editMode);
		}

		private bool IsOnlyLeftMouseButtonDown() => m_InputState.IsOnlyButtonDown(MouseButton.LeftMouse);
		private bool WasLeftMouseButtonDown() => m_InputState.WasButtonDown(MouseButton.LeftMouse);
		private bool IsRightMouseButtonDown() => m_InputState.IsButtonDown(MouseButton.RightMouse);
		private bool WasRightMouseButtonDown() => m_InputState.WasButtonDown(MouseButton.RightMouse);
	}
}