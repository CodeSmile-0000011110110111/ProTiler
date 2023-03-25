// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile;
using UnityEngine;

namespace CodeSmileEditor.Tile
{
	public partial class TileLayerEditor
	{
		private void HandleEventsAndInput()
		{
			m_Input.UpdateInputStates();

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
				case EventType.DragUpdated:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.DragPerform:
					Debug.Log($"{Event.current.type}");
					break;
				case EventType.DragExited:
					Debug.Log($"{Event.current.type}");
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
					OnMouseEnterWindow();
					break;
				case EventType.MouseLeaveWindow:
					OnMouseLeaveWindow();
					break;
				case EventType.Repaint:
					OnRepaint();
					break;

				// handled by EditorInputState
				case EventType.MouseDown:
				case EventType.MouseUp:
				case EventType.KeyDown:
				case EventType.KeyUp:
				case EventType.ScrollWheel:
					break;
			}

			UpdateClearingState();
			UpdateLayerDrawBrush();
		}

		private void OnKeyDown(KeyCode keyCode)
		{
			if (keyCode == KeyCode.Escape)
				CancelTileDrawing();
		}

		private void OnKeyUp(KeyCode keyCode) {}
		private void OnMouseEnterWindow() => m_IsMouseInView = true;

		private void OnMouseLeaveWindow()
		{
			m_IsMouseInView = false;
			FinishTileDrawing();
		}

		private void OnMouseMove()
		{
			// Note: MouseMove and MouseDrag are mutually exclusive! With button down only MouseDrag event is sent.
			// We do not need to check for mouse button down here since they can be assumed "up" at all times in MouseMove.
			UpdateStartSelectionCoord();
			UpdateCursorCoord();
		}

		private void OnMouseButtonDown(MouseButton button)
		{
			if (IsRightMouseButtonDown())
				CancelTileDrawing();
			else if (IsOnlyLeftMouseButtonDown())
				StartTileDrawing();
		}
		private void OnMouseButtonUp(MouseButton button)
		{
			if (button == MouseButton.RightMouse)
				CancelTileDrawing();
			else if (button == MouseButton.LeftMouse)
				FinishTileDrawing();
		}

		private void OnMouseDrag()
		{
			if (IsRightMouseButtonDown())
				CancelTileDrawing();
			else if (IsOnlyLeftMouseButtonDown())
				ContinueTileDrawing();
		}

		private void OnScrollWheel(IInputState inputState, float delta)
		{
			var editMode = TileEditorState.instance.TileEditMode;
			if (editMode != TileEditMode.Selection)
				if (Layer.ModifyTileAttributes(m_CursorCoord, delta, inputState.IsShiftKeyDown, inputState.IsCtrlKeyDown))
					Event.current.Use();
		}

		private void OnLayout() => HandleUtilityExt.AddDefaultControl(GetHashCode());

		private void OnRepaint()
		{
			if (m_IsMouseInView && IsRightMouseButtonDown() == false)
				DrawCursorHandle(TileEditorState.instance.TileEditMode);
		}

		private bool IsOnlyLeftMouseButtonDown() => m_Input.IsOnlyButtonDown(MouseButton.LeftMouse);

		private bool IsRightMouseButtonDown() => m_Input.IsButtonDown(MouseButton.RightMouse);

	}
}