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
	public enum DrawingMode
	{
		Pen,
		RectangleFill,
	}

	[CustomEditor(typeof(TileWorld))]
	public class TileWorldEditor : Editor
	{
		private readonly EditorInputState m_InputState = new();
		private GridCoord m_StartSelectionCoord;
		private GridCoord m_CursorCoord;
		private GridRect m_SelectionRect;
		private bool m_IsDrawingTiles;
		private bool m_IsClearingTiles;

		private readonly DrawingMode m_DrawingMode = DrawingMode.Pen;

		private float2 MousePos => Event.current.mousePosition;
		private TileWorld TileWorld => (TileWorld)target;
		private TileLayer ActiveLayer => TileWorld.ActiveLayer;
		private TileGrid ActiveLayerGrid => TileWorld.ActiveLayer.Grid;

		private void OnSceneGUI()
		{
			if (Selection.activeGameObject != TileWorld.gameObject)
				return;

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
					{
						UpdateStartSelectionCoord();
						UpdateCursorCoord();
						if (m_DrawingMode == DrawingMode.Pen)
							StartTileDrawing();
						Event.current.Use();
					}
					break;
				case EventType.MouseDrag:
					if (IsLeftMouseButtonDown())
					{
						if (m_DrawingMode == DrawingMode.Pen)
						{
							UpdateStartSelectionCoord();
							DrawTile(m_StartSelectionCoord, m_IsClearingTiles);
						}
						UpdateCursorCoord();
						Event.current.Use();
					}
					break;
				case EventType.MouseUp:
					if (m_IsDrawingTiles)
					{
						UpdateCursorCoord();
						if (m_DrawingMode == DrawingMode.Pen)
							DrawTile(m_StartSelectionCoord, m_IsClearingTiles);
						EndTileDrawing();
						Event.current.Use();
					}
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
					//Debug.Log($"{Event.current.type}");
					break;
				case EventType.MouseLeaveWindow:
					//Debug.Log($"{Event.current.type}");
					break;
				case EventType.Repaint:
					DrawCursor();
					break;
			}
		}

		private void StartTileDrawing()
		{
			m_IsDrawingTiles = true;
			m_IsClearingTiles = Event.current.shift;
			Undo.RecordObject(TileWorld, m_IsClearingTiles ? "Clear Tile" : "Draw Tile");
		}

		private void EndTileDrawing()
		{
			m_IsDrawingTiles = false;
			m_IsClearingTiles = false;
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

		/*
		private void ChangeTile()
		{
			// TODO: Refactor ...
			
			var ev = Event.current;
			if (ev.shift)
			{
				var delta = ev.delta.y >= 0 ? 1 : -1;
				ActiveLayer.SelectedTileSetIndex += delta;
				ActiveLayer.OnValidate();

				if (ev.control == false)
				{
					var tile = ActiveLayer.GetTile(m_CursorCoord);
					var newTile = new CodeSmile.Tile.Tile(tile);
					newTile.TileSetIndex = ActiveLayer.SelectedTileSetIndex;

					Undo.RecordObject(TileWorld, "Layer.SetTile");
					ActiveLayer.SetTile(m_CursorCoord, newTile);
				}
				ev.Use();
			}
		}
		*/

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
			if (clear)
				ActiveLayer.ClearTile(coord);
			else
				ActiveLayer.DrawTile(coord);

			EditorUtility.SetDirty(TileWorld);
		}

		private void DrawCursor()
		{
			var worldRect = TileGrid.ToWorldRect(m_SelectionRect, ActiveLayerGrid.Size);
			var worldPos = TileWorld.transform.position;
			var cubePos = worldRect.GetWorldCenter() + worldPos;
			var cubeSize = worldRect.GetWorldSize(ActiveLayer.TileCursorHeight);
			Handles.DrawWireCube(cubePos, cubeSize);
		}
	}
}