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
	public partial class TileLayerEditor : Editor
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

			var editMode = TileEditorState.instance.TileEditMode;

			var previewRenderer = Layer.GetComponent<TilePreviewRenderer>();
			if (previewRenderer != null)
				previewRenderer.ShowPreview = m_IsMouseInView && editMode != TileEditMode.Selection;

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
					OnMouseWheelChangeSelectedTileSetIndex();
					break;
				case EventType.KeyDown:
					HandleKeyDown();
					break;
				case EventType.KeyUp:
					HandleKeyUp();
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
					if (m_IsMouseInView && editMode == TileEditMode.PenDraw)
						DrawCursorHandle();
					break;
			}
		}

		private void OnMouseWheelChangeSelectedTileSetIndex()
		{
			var ev = Event.current;
			if (ev.control)
			{
				var delta = ev.delta.y >= 0 ? 1 : -1;
				SetDrawTileSetIndex(GetDrawTileSetIndex() + delta);
				SetLayerBrush();
				ev.Use();
			}
		}

		private void SetDrawTileSetIndex(int tileSetIndex)
		{
			tileSetIndex = math.clamp(tileSetIndex, 0, Layer.TileSet.Count - 1);
			TileEditorState.instance.DrawTileSetIndex = tileSetIndex;
		}

		private TileBrush CreateDrawBrush(bool clear) =>
			new(m_CursorCoord, clear ? Global.InvalidTileSetIndex : TileEditorState.instance.DrawTileSetIndex);

		private int GetDrawTileSetIndex() => TileEditorState.instance.DrawTileSetIndex;

		private void StartTileDrawing(TileEditMode tileEditMode)
		{
			UpdateStartSelectionCoord();
			UpdateCursorCoord();
			if (tileEditMode == TileEditMode.PenDraw)
			{
				m_IsDrawingTiles = true;
				SetLayerBrush();
			}

			if (tileEditMode != TileEditMode.Selection)
				Event.current.Use();
		}

		private void SetLayerBrush() => Layer.DrawBrush = CreateDrawBrush(m_IsClearingTiles);

		private void ContinueTileDrawing(TileEditMode tileEditMode)
		{
			if (tileEditMode == TileEditMode.PenDraw)
			{
				UpdateCursorCoord();
				DrawFromStartToCursor();
			}
			UpdateStartSelectionCoord();

			if (tileEditMode != TileEditMode.Selection)
				Event.current.Use();
		}

		private void EndTileDrawing(TileEditMode tileEditMode)
		{
			if (m_IsDrawingTiles)
			{
				UpdateCursorCoord();
				if (tileEditMode == TileEditMode.PenDraw)
					DrawFromStartToCursor();

				m_IsDrawingTiles = false;
				UpdateClearingState();

				if (tileEditMode != TileEditMode.Selection)
					Event.current.Use();
			}
		}

		private void DrawFromStartToCursor() => Layer.DrawLine(m_StartSelectionCoord, m_CursorCoord);

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
				SetLayerBrush();
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
			var cubeSize = worldRect.GetWorldSize(Layer.Grid.Size.y);
			Handles.DrawWireCube(cubePos, cubeSize);
		}

		private void UpdateClearingState()
		{
			if (m_IsDrawingTiles == false)
			{
				var isHoldingShift = Event.current.shift;
				if (m_IsClearingTiles != isHoldingShift)
				{
					m_IsClearingTiles = isHoldingShift;
					SetLayerBrush();
				}
			}
		}
	}
}