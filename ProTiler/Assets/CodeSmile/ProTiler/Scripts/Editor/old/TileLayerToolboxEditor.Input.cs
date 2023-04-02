// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor.InputState;
using CodeSmile.InputState;
using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Data;
using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler
{
	public partial class TileLayerToolboxEditor
	{
		private IInputState m_Input;

		private void RegisterInputEvents()
		{
			if (m_Input == null)
			{
				Debug.Log("REGISTER INput events");
				var input = new EditorInputState();
				input.OnMouseButtonDown += OnMouseButtonDown;
				input.OnMouseButtonUp += OnMouseButtonUp;
				input.OnScrollWheel += OnScrollWheel;
				input.OnKeyDown += OnKeyDown;
				input.OnKeyUp += OnKeyUp;
				m_Input = input;
			}
		}

		private void UnregisterInputEvents()
		{
			if (m_Input != null)
			{
				Debug.Log("Unregister Input events");
				var input = m_Input as EditorInputState;
				input.OnMouseButtonDown -= OnMouseButtonDown;
				input.OnMouseButtonUp -= OnMouseButtonUp;
				input.OnScrollWheel -= OnScrollWheel;
				input.OnKeyDown -= OnKeyDown;
				input.OnKeyUp -= OnKeyUp;
			}
		}

		private void UpdateInputStates() => m_Input.Update();

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
			CancelTileDrawing();
			//FinishTileDrawing(TileEditorState.instance.TileEditMode);
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
			{
				if (StartTileDrawing(ProTilerState.instance.TileEditMode))
					Event.current.Use();
			}
		}

		private void OnMouseButtonUp(MouseButton button)
		{
			if (button == MouseButton.RightMouse)
				CancelTileDrawing();
			else if (button == MouseButton.LeftMouse)
			{
				if (FinishTileDrawing(ProTilerState.instance.TileEditMode))
					Event.current.Use();
			}
		}

		private void OnMouseDrag()
		{
			if (IsRightMouseButtonDown())
				CancelTileDrawing();
			else if (IsOnlyLeftMouseButtonDown())
			{
				if (ContinueTileDrawing(ProTilerState.instance.TileEditMode))
					Event.current.Use();
			}
		}

		private void OnScrollWheel(IInputState inputState, float scrollDelta)
		{
			var editorState = ProTilerState.instance;
			var editMode = editorState.TileEditMode;
			if (editMode != TileEditMode.Selection)
			{
				var delta = scrollDelta >= 0 ? 1 : -1;
				var shift = inputState.IsShiftKeyDown;
				var ctrl = inputState.IsCtrlKeyDown;
				if (shift && ctrl)
				{
					Toolbox.FlipTile(m_CursorCoord, delta);
					Event.current.Use();
				}
				else if (shift)
				{
					Toolbox.RotateTile(m_CursorCoord, delta);
					Event.current.Use();
				}
				else if (ctrl)
				{
					editorState.DrawTileSetIndex = math.clamp(editorState.DrawTileSetIndex + delta,
						TileData.InvalidTileSetIndex, Toolbox.Layer.TileSet.Count - 1);
					UpdateLayerDrawBrush();
					Event.current.Use();
				}
			}
		}

		private bool IsOnlyLeftMouseButtonDown() => m_Input.IsOnlyButtonDown(MouseButton.LeftMouse);

		private bool IsRightMouseButtonDown() => m_Input.IsButtonDown(MouseButton.RightMouse);
	}
}
