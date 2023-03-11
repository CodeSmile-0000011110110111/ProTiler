// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.UnityEditor;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;

namespace CodeSmile.Tile.UnityEditor
{
	[CustomEditor(typeof(TileWorld))]
	public class TileWorldEditor : Editor
	{
		private readonly EditorInputState m_InputState = new();
		private GridCoord m_StartSelectionCoord;
		private GridRect m_SelectionRect;
		private bool m_IsPaintingTiles;

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
					UpdateCurrentSelectionCoord();
					break;
				case EventType.MouseDown:
					if (IsLeftMouseButtonDown())
					{
						m_IsPaintingTiles = true;
						UpdateStartSelectionCoord();
						Event.current.Use();
					}
					break;
				case EventType.MouseDrag:
					if (IsLeftMouseButtonDown())
					{
						UpdateCurrentSelectionCoord();
						Event.current.Use();
					}
					break;
				case EventType.MouseUp:
					if (m_IsPaintingTiles)
					{
						m_IsPaintingTiles = false;
						PaintSelectedTiles();
						Event.current.Use();
					}
					break;
				case EventType.ScrollWheel:
					ChangeTile();
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
					//Debug.Log($"{Event.current.type}");
					break;
				case EventType.ExecuteCommand:
					//Debug.Log($"{Event.current.type}");
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
					DrawSelection();
					break;
			}
		}

		private void HandleShortcuts()
		{
			var didModify = false;
			var shouldUseEvent = false;
			var cursorCoord = ActiveLayer.CursorCoord;
			switch (Event.current.keyCode)
			{
				case KeyCode.LeftArrow:
				case KeyCode.RightArrow:
				case KeyCode.UpArrow:
				case KeyCode.DownArrow:
					ActiveLayer.ClearTileFlags(cursorCoord, TileFlags.DirectionNorth);
					ActiveLayer.ClearTileFlags(cursorCoord, TileFlags.DirectionSouth);
					ActiveLayer.ClearTileFlags(cursorCoord, TileFlags.DirectionEast);
					ActiveLayer.ClearTileFlags(cursorCoord, TileFlags.DirectionWest);
					didModify = true;
					break;
			}

			//Debug.Log($"{Event.current.type}: {Event.current.keyCode}" );

			switch (Event.current.keyCode)
			{
				case KeyCode.F:
				{
					var camera = Camera.current;
					camera.transform.position = ActiveLayer.Grid.ToWorldPosition(cursorCoord);
					shouldUseEvent = true;
					break;
				}
				case KeyCode.H:
				{
					var tile = ActiveLayer.GetTile(cursorCoord);
					if (tile == null)
						break;

					if (tile.Flags.HasFlag(TileFlags.FlipHorizontal))
						ActiveLayer.ClearTileFlags(cursorCoord, TileFlags.FlipHorizontal);
					else
						ActiveLayer.SetTileFlags(cursorCoord, TileFlags.FlipHorizontal);
					didModify = true;
					break;
				}
				case KeyCode.V:
				{
					var tile = ActiveLayer.GetTile(cursorCoord);
					if (tile == null)
						break;

					if (ActiveLayer.GetTile(cursorCoord).Flags.HasFlag(TileFlags.FlipVertical))
						ActiveLayer.ClearTileFlags(cursorCoord, TileFlags.FlipVertical);
					else
						ActiveLayer.SetTileFlags(cursorCoord, TileFlags.FlipVertical);
					didModify = true;
					break;
				}
				case KeyCode.LeftArrow:
					ActiveLayer.SetTileFlags(cursorCoord, TileFlags.DirectionWest);
					break;
				case KeyCode.RightArrow:
					ActiveLayer.SetTileFlags(cursorCoord, TileFlags.DirectionEast);
					break;
				case KeyCode.UpArrow:
					ActiveLayer.SetTileFlags(cursorCoord, TileFlags.DirectionNorth);
					break;
				case KeyCode.DownArrow:
					ActiveLayer.SetTileFlags(cursorCoord, TileFlags.DirectionSouth);
					break;
			}

			if (didModify)
				EditorUtility.SetDirty(TileWorld);
			if (shouldUseEvent || didModify)
				Event.current.Use();
		}

		private void ChangeTile()
		{
			var ev = Event.current;
			if (ev.shift)
			{
				var delta = ev.delta.y >= 0 ? 1 : -1;
				ActiveLayer.SelectedTileSetIndex += delta;
				ActiveLayer.OnValidate();

				if (ev.control == false)
				{
					var cursorCoord = ActiveLayer.CursorCoord;
					var tile = ActiveLayer.GetTile(cursorCoord);
					var newTile = new Tile(tile);
					newTile.TileSetIndex = ActiveLayer.SelectedTileSetIndex;
					ActiveLayer.SetTile(cursorCoord, newTile);
				}
				ev.Use();
			}
		}

		private void UpdateStartSelectionCoord()
		{
			if (HandleUtilityExt.GUIPointToGridCoord(MousePos, ActiveLayerGrid, out var coord))
			{
				m_StartSelectionCoord = coord;
				UpdateSelectionRect();
			}
		}

		private void UpdateCurrentSelectionCoord()
		{
			if (HandleUtilityExt.GUIPointToGridCoord(MousePos, ActiveLayerGrid, out var coord))
			{
				ActiveLayer.CursorCoord = coord;
				UpdateSelectionRect();
			}
		}

		private void UpdateSelectionRect() => m_SelectionRect = TileGrid.MakeRect(m_StartSelectionCoord, ActiveLayer.CursorCoord);

		private float2 MousePos => Event.current.mousePosition;
		private TileWorld TileWorld => (TileWorld)target;
		private TileLayer ActiveLayer => TileWorld.ActiveLayer;
		private TileGrid ActiveLayerGrid => TileWorld.ActiveLayer.Grid;

		private bool IsLeftMouseButtonDown() => m_InputState.IsButtonDown(MouseButton.LeftMouse);

		private void PaintSelectedTiles()
		{
			ActiveLayer.SetTiles(m_SelectionRect, Event.current.shift);
			//Undo.RecordObject(ActiveLayer, "Paint Tiles");
			EditorUtility.SetDirty(TileWorld);
		}

		private void DrawSelection()
		{
			var worldRect = TileGrid.ToWorldRect(m_SelectionRect, ActiveLayerGrid.Size);
			var worldPos = TileWorld.transform.position;
			var cubePos = worldRect.GetWorldCenter() + worldPos;
			var cubeSize = worldRect.GetWorldSize(ActiveLayer.TileCursorHeight);
			Handles.DrawWireCube(cubePos, cubeSize);
		}
	}
}