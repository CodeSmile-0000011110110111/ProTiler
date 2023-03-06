// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.UnityEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.Tile.UnityEditor
{
	[CustomEditor(typeof(TileWorld))]
	public class TileWorldEditor : Editor
	{
		private readonly EditorInputState m_InputState = new();
		private Vector3 m_CursorWorldPosition;
		private GridCoord m_CursorCurrentCoord;
		private GridCoord m_CursorStartCoord;

		private void OnSceneGUI()
		{
			m_InputState?.Update();

			//Debug.Log($"{Time.frameCount}: {Event.current.type}");
			switch (Event.current.type)
			{
				case EventType.MouseMove:
					SetMouseDownCursorCoord(Event.current.mousePosition);
					UpdateCursorCoord(Event.current.mousePosition);
					break;
				case EventType.MouseDown:
					if (IsLeftMouseButtonDown())
					{
						SetMouseDownCursorCoord(Event.current.mousePosition);
						SetEventUsed(MouseButton.LeftMouse);
					}
					break;
				case EventType.MouseUp:
					SetTilesInSelection();
					break;
				case EventType.MouseDrag:
					if (IsLeftMouseButtonDown())
					{
						UpdateCursorCoord(Event.current.mousePosition);
						SetEventUsed(MouseButton.LeftMouse);
					}
					break;
				case EventType.ScrollWheel:
					break;
				case EventType.Repaint:
					DrawTileCursor();
					break;
				case EventType.Layout:
					AddDefaultControl();
					break;
				case EventType.DragUpdated:
					Debug.Log("drag update");
					break;
				case EventType.DragPerform:
					Debug.Log("drag perform");
					break;
				case EventType.DragExited:
					Debug.Log("drag exit");
					break;
				case EventType.Ignore:
					break;
				case EventType.Used:
					break;
				case EventType.ValidateCommand:
					break;
				case EventType.ExecuteCommand:
					break;
				case EventType.ContextClick:
					Debug.Log("context click");
					break;
				case EventType.MouseEnterWindow:
					break;
				case EventType.MouseLeaveWindow:
					break;
				case EventType.TouchMove:
					break;
				case EventType.TouchEnter:
					break;
				case EventType.TouchLeave:
					break;
				case EventType.TouchStationary:
					break;
			}
		}

		private bool IsLeftMouseButtonDown() => m_InputState.IsButtonDown(MouseButton.LeftMouse);

		private void SetTilesInSelection()
		{
			var tileWorld = (TileWorld)target;
			tileWorld.ActiveLayer.SetTileAt(m_CursorCurrentCoord);
			EditorUtility.SetDirty(tileWorld);
		}

		private void SetEventUsed(MouseButton button) => Event.current.Use();

		private void SetMouseDownCursorCoord(Vector2 mousePos)
		{
			m_CursorStartCoord = GetGridCoord(mousePos);
			m_CursorStartCoord.y = 0;
			//Debug.Log($"mouse down coord: {m_CursorStartCoord}");
		}

		private void UpdateCursorCoord(Vector2 mousePos)
		{
			m_CursorCurrentCoord = GetGridCoord(mousePos);
			m_CursorCurrentCoord.y = 0;
		}

		private GridCoord GetGridCoord(Vector2 mousePos)
		{
			var ray = HandleUtility.GUIPointToWorldRay(mousePos);
			if (Ray.IntersectsVirtualPlane(ray, out m_CursorWorldPosition))
			{
				var tileWorld = (TileWorld)target;
				var gridSize = tileWorld.ActiveLayer.Grid.Size;
				return GridCoord.FloorToInt(HandlesExt.SnapPointToGrid(m_CursorWorldPosition, gridSize));
			}

			return default;
		}

		private void DrawTileCursor()
		{
			var tileWorld = (TileWorld)target;
			var activeLayer = tileWorld.ActiveLayer;

			var worldPos = tileWorld.transform.position;

			var centerPos = (Vector3)(m_CursorCurrentCoord - m_CursorStartCoord) / 2f + m_CursorStartCoord;
			//Debug.Log($"center: {centerPos} from current-start: {m_CursorCurrentCoord} - {m_CursorStartCoord}");
			var renderPos = activeLayer.GetTileWorldPosition(centerPos) + worldPos;

			var gridSize = (Vector3)activeLayer.Grid.Size;
			gridSize.y = activeLayer.TileCursorHeight;
			renderPos.y += gridSize.y * .5f;

			var cursorHeight = activeLayer.TileCursorHeight;
			var selection = GetSelectionRect();
			renderPos = new Vector3(selection.center.x + selection.width * gridSize.x, renderPos.y, selection.center.y+ selection.width * gridSize.z);
			var cubeSize = new Vector3(selection.width * gridSize.x, cursorHeight, selection.height * gridSize.z);
			Handles.DrawWireCube(renderPos, cubeSize);
		}

		private RectInt GetSelectionRect()
		{
			var coordMin = GridCoord.Min(m_CursorStartCoord, m_CursorCurrentCoord);
			var coordMax = GridCoord.Max(m_CursorStartCoord, m_CursorCurrentCoord);
			var selection = new RectInt(coordMin.x, coordMin.z,
				coordMin.x + coordMax.x + 1,
				coordMin.z + coordMax.z + 1);
			Debug.Log($"rect: {selection} from min/max: {coordMin} / {coordMax}");
			return selection;
		}

		private void AddDefaultControl()
		{
			var controlId = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
			HandleUtility.AddDefaultControl(controlId);
		}
	}
}