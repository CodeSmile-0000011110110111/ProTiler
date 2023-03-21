// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile;
using CodeSmile.Tile;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;

namespace CodeSmileEditor.Tile
{
	[CustomEditor(typeof(TileLayer))]
	public class TileLayerEditor : Editor
	{
		private readonly EditorInputState m_InputState = new();
		private GridCoord m_StartSelectionCoord;
		private GridCoord m_CursorCoord;
		private GridRect m_SelectionRect;
		private bool m_IsDrawingTiles;
		private bool m_IsClearingTiles;
		private bool m_IsMouseInView;

		private float2 MousePos { get => Event.current.mousePosition; }
		private TileLayer Layer { get => (TileLayer)target; }
		private TileGrid Grid { get => ((TileLayer)target).Grid; }

		private void OnSceneGUI()
		{
			if (Selection.activeGameObject != Layer.gameObject)
				return;

			var editMode = TileEditorState.instance.EditMode;

			var previewRenderer = Layer.GetComponent<TilePreviewRenderer>();
			if (previewRenderer != null)
				previewRenderer.ShowPreview = m_IsMouseInView && editMode != EditMode.Selection;

			m_InputState?.Update();

			//Debug.Log($"{Time.frameCount}: {Event.current.type}");
			switch (Event.current.type)
			{
				case EventType.Layout:
					HandleUtilityExt.AddDefaultControl(GetHashCode());
					break;
				case EventType.MouseMove:
					// Note: MouseMove and MouseDrag are mutually exclusive! With button down only MouseDrag event is sent.
					// We do not need to check for mouse button down here since they can be assumed "up" at all times in MouseMove.
					UpdateStartSelectionCoord();
					UpdateCursorCoord();
					break;
				case EventType.MouseDown:
					if (IsLeftMouseButtonDown())
						StartTileDrawing(editMode);
					break;
				case EventType.MouseDrag:
					if (IsLeftMouseButtonDown())
						ContinueTileDrawing(editMode);
					break;
				case EventType.MouseUp:
					EndTileDrawing(editMode);
					break;
				case EventType.ScrollWheel:
					//ChangeTile();
					break;
				case EventType.KeyDown:
					HandleShortcuts();
					break;
				case EventType.KeyUp:
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
					m_IsMouseInView = true;
					break;
				case EventType.MouseLeaveWindow:
					m_IsMouseInView = false;
					EndTileDrawing(editMode);
					break;
				case EventType.Repaint:
					if (m_IsMouseInView && editMode == EditMode.PenDraw)
						DrawCursorHandle();
					break;
			}
		}

		private void StartTileDrawing(EditMode editMode)
		{
			UpdateStartSelectionCoord();
			UpdateCursorCoord();
			if (editMode == EditMode.PenDraw)
			{
				m_IsDrawingTiles = true;
				m_IsClearingTiles = Event.current.shift;
			}

			if (editMode != EditMode.Selection)
				Event.current.Use();
		}

		private void ContinueTileDrawing(EditMode editMode)
		{
			if (editMode == EditMode.PenDraw)
			{
				UpdateCursorCoord();
				Layer.DrawLine(m_StartSelectionCoord, m_CursorCoord, TileEditorState.instance.DrawingTileSetIndex);
			}
			UpdateStartSelectionCoord();

			if (editMode != EditMode.Selection)
				Event.current.Use();
		}

		private void EndTileDrawing(EditMode editMode)
		{
			if (m_IsDrawingTiles)
			{
				UpdateCursorCoord();
				if (editMode == EditMode.PenDraw)
					Layer.DrawLine(m_StartSelectionCoord, m_CursorCoord, TileEditorState.instance.DrawingTileSetIndex);

				m_IsDrawingTiles = false;
				m_IsClearingTiles = false;

				if (editMode != EditMode.Selection)
					Event.current.Use();
			}
		}

		private void HandleShortcuts()
		{
			var shouldUseEvent = false;
			switch (Event.current.keyCode)
			{
				case KeyCode.LeftArrow:
				case KeyCode.RightArrow:
				case KeyCode.UpArrow:
				case KeyCode.DownArrow:
					Layer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionNorth);
					Layer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionSouth);
					Layer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionEast);
					Layer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionWest);
					break;
			}

			//Debug.Log($"{Event.current.type}: {Event.current.keyCode}" );

			switch (Event.current.keyCode)
			{
				case KeyCode.F:
				{
					var camera = Camera.current;
					camera.transform.position = Layer.Grid.ToWorldPosition(m_CursorCoord);
					shouldUseEvent = true;
					break;
				}
				case KeyCode.H:
				{
					var tile = Layer.GetTileData(m_CursorCoord);
					if (tile.TileSetIndex < 0)
						break;

					if (tile.Flags.HasFlag(TileFlags.FlipHorizontal))
						Layer.ClearTileFlags(m_CursorCoord, TileFlags.FlipHorizontal);
					else
						Layer.SetTileFlags(m_CursorCoord, TileFlags.FlipHorizontal);
					break;
				}
				case KeyCode.V:
				{
					var tile = Layer.GetTileData(m_CursorCoord);
					if (tile.TileSetIndex < 0)
						break;

					if (tile.Flags.HasFlag(TileFlags.FlipVertical))
						Layer.ClearTileFlags(m_CursorCoord, TileFlags.FlipVertical);
					else
						Layer.SetTileFlags(m_CursorCoord, TileFlags.FlipVertical);
					break;
				}
				case KeyCode.LeftArrow:
					Layer.SetTileFlags(m_CursorCoord, TileFlags.DirectionWest);
					break;
				case KeyCode.RightArrow:
					Layer.SetTileFlags(m_CursorCoord, TileFlags.DirectionEast);
					break;
				case KeyCode.UpArrow:
					Layer.SetTileFlags(m_CursorCoord, TileFlags.DirectionNorth);
					break;
				case KeyCode.DownArrow:
					Layer.SetTileFlags(m_CursorCoord, TileFlags.DirectionSouth);
					break;
			}

			if (shouldUseEvent)
				Event.current.Use();
		}

		private void UpdateStartSelectionCoord()
		{
			var planeY = Layer.transform.position.y;
			if (HandleUtilityExt.GUIPointToGridCoord(MousePos, Grid, out var coord, planeY))
			{
				m_StartSelectionCoord = coord;
				UpdateSelectionRect();
			}
		}

		private void UpdateCursorCoord()
		{
			var planeY = Layer.transform.position.y;
			if (HandleUtilityExt.GUIPointToGridCoord(MousePos, Grid, out var coord, planeY))
			{
				m_CursorCoord = coord;
				Layer.DebugCursorCoord = coord;
				UpdateSelectionRect();
			}
		}

		private void UpdateSelectionRect() => m_SelectionRect = TileGrid.MakeRect(m_StartSelectionCoord, m_CursorCoord);

		private bool IsLeftMouseButtonDown() => m_InputState.IsButtonDown(MouseButton.LeftMouse);



		private void DrawCursorHandle()
		{
			var worldRect = TileGrid.ToWorldRect(m_SelectionRect, Grid.Size);
			var worldPos = Layer.transform.position;
			var cubePos = worldRect.GetWorldCenter() + worldPos;
			var cubeSize = worldRect.GetWorldSize(Layer.TileCursorHeight);
			Handles.DrawWireCube(cubePos, cubeSize);
		}
	}
}