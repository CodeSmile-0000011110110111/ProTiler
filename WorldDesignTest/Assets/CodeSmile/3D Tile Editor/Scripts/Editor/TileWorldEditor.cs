// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile;
using CodeSmile.Tile;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;

namespace CodeSmileEditor.Tile
{
	[CustomEditor(typeof(TileWorld))]
	public class TileWorldEditor : Editor
	{
		private readonly EditorInputState m_InputState = new();
		private GridCoord m_StartSelectionCoord;
		private GridCoord m_CursorCoord;
		private GridRect m_SelectionRect;
		private bool m_IsDrawingTiles;
		private bool m_IsClearingTiles;
		private bool m_IsMouseInView;

		private float2 MousePos { get => Event.current.mousePosition; }
		private TileWorld TileWorld { get => (TileWorld)target; }
		private TileLayer ActiveLayer { get => TileWorld.ActiveLayer; }
		private TileGrid ActiveLayerGrid { get => TileWorld.ActiveLayer.Grid; }

		private void OnSceneGUI()
		{
			if (Selection.activeGameObject != TileWorld.gameObject)
				return;

			var editMode = TileEditorState.instance.EditMode;

			var previewRenderer = (target as TileWorld).GetComponent<TilePreviewRenderer>();
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
				DrawLine(m_StartSelectionCoord, m_CursorCoord, m_IsClearingTiles);
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
					DrawLine(m_StartSelectionCoord, m_StartSelectionCoord, m_IsClearingTiles);

				m_IsDrawingTiles = false;
				m_IsClearingTiles = false;

				if (editMode != EditMode.Selection)
					Event.current.Use();
			}
		}

		private void HandleShortcuts()
		{
			var didModify = false;
			var shouldUseEvent = false;
			switch (Event.current.keyCode)
			{
				case KeyCode.LeftArrow:
				case KeyCode.RightArrow:
				case KeyCode.UpArrow:
				case KeyCode.DownArrow:
					ActiveLayer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionNorth);
					ActiveLayer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionSouth);
					ActiveLayer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionEast);
					ActiveLayer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionWest);
					didModify = true;
					break;
			}

			//Debug.Log($"{Event.current.type}: {Event.current.keyCode}" );

			switch (Event.current.keyCode)
			{
				case KeyCode.F:
				{
					var camera = Camera.current;
					camera.transform.position = ActiveLayer.Grid.ToWorldPosition(m_CursorCoord);
					shouldUseEvent = true;
					break;
				}
				case KeyCode.H:
				{
					var tile = ActiveLayer.GetTile(m_CursorCoord);
					if (tile.TileSetIndex < 0)
						break;

					if (tile.Flags.HasFlag(TileFlags.FlipHorizontal))
					{
						Undo.RecordObject(TileWorld, "Layer.ClearTileFlags");
						ActiveLayer.SetTileFlags(m_CursorCoord, TileFlags.FlipVertical);
						ActiveLayer.ClearTileFlags(m_CursorCoord, TileFlags.FlipHorizontal);
					}
					else
					{
						Undo.RecordObject(TileWorld, "Layer.SetTileFlags");
						ActiveLayer.SetTileFlags(m_CursorCoord, TileFlags.FlipHorizontal);
					}
					didModify = true;
					break;
				}
				case KeyCode.V:
				{
					var tile = ActiveLayer.GetTile(m_CursorCoord);
					if (tile.TileSetIndex < 0)
						break;

					if (ActiveLayer.GetTile(m_CursorCoord).Flags.HasFlag(TileFlags.FlipVertical))
					{
						Undo.RecordObject(TileWorld, "Layer.ClearTileFlags");
						ActiveLayer.ClearTileFlags(m_CursorCoord, TileFlags.FlipVertical);
					}
					else
					{
						Undo.RecordObject(TileWorld, "Layer.SetTileFlags");
						ActiveLayer.SetTileFlags(m_CursorCoord, TileFlags.FlipVertical);
					}
					didModify = true;
					break;
				}
				case KeyCode.LeftArrow:
					Undo.RecordObject(TileWorld, "Layer.SetTileFlags");
					ActiveLayer.SetTileFlags(m_CursorCoord, TileFlags.DirectionWest);
					break;
				case KeyCode.RightArrow:
					Undo.RecordObject(TileWorld, "Layer.SetTileFlags");
					ActiveLayer.SetTileFlags(m_CursorCoord, TileFlags.DirectionEast);
					break;
				case KeyCode.UpArrow:
					Undo.RecordObject(TileWorld, "Layer.SetTileFlags");
					ActiveLayer.SetTileFlags(m_CursorCoord, TileFlags.DirectionNorth);
					break;
				case KeyCode.DownArrow:
					Undo.RecordObject(TileWorld, "Layer.SetTileFlags");
					ActiveLayer.SetTileFlags(m_CursorCoord, TileFlags.DirectionSouth);
					break;
			}

			if (didModify)
				EditorUtility.SetDirty(TileWorld);
			if (shouldUseEvent || didModify)
				Event.current.Use();
		}

		private void UpdateStartSelectionCoord()
		{
			var planeY = TileWorld.transform.position.y;
			if (HandleUtilityExt.GUIPointToGridCoord(MousePos, ActiveLayerGrid, out var coord, planeY))
			{
				m_StartSelectionCoord = coord;
				UpdateSelectionRect();
			}
		}

		private void UpdateCursorCoord()
		{
			var planeY = TileWorld.transform.position.y;
			if (HandleUtilityExt.GUIPointToGridCoord(MousePos, ActiveLayerGrid, out var coord, planeY))
			{
				m_CursorCoord = coord;
				ActiveLayer.DebugCursorCoord = coord;
				UpdateSelectionRect();
			}
		}

		private void UpdateSelectionRect() => m_SelectionRect = TileGrid.MakeRect(m_StartSelectionCoord, m_CursorCoord);

		private bool IsLeftMouseButtonDown() => m_InputState.IsButtonDown(MouseButton.LeftMouse);

		private void DrawTile(GridCoord coord, bool clear)
		{
			//Debug.Log($"\tDrawTile at {coord}");
			Undo.RecordObject(TileWorld, m_IsClearingTiles ? $"Clear Tile {coord}" : $"Draw Tile {coord}");

			if (clear)
				ActiveLayer.ClearTile(coord);
			else
				ActiveLayer.DrawTile(coord);

			EditorUtility.SetDirty(TileWorld);
		}

		private void DrawLine(GridCoord start, GridCoord end, bool clear)
		{
			var coords = start.MakeLine(end);
			foreach (var coord in coords)
				DrawTile(coord, clear);
		}

		private void DrawCursorHandle()
		{
			var worldRect = TileGrid.ToWorldRect(m_SelectionRect, ActiveLayerGrid.Size);
			var worldPos = TileWorld.transform.position;
			var cubePos = worldRect.GetWorldCenter() + worldPos;
			var cubeSize = worldRect.GetWorldSize(ActiveLayer.TileCursorHeight);
			Handles.DrawWireCube(cubePos, cubeSize);
		}
	}
}