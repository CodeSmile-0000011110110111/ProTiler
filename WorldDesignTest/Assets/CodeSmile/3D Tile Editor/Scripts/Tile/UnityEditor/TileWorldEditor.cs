// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.UnityEditor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmile.Tile.UnityEditor
{
	[CustomEditor(typeof(TileWorld))]
	public class TileWorldEditor : Editor
	{
		private readonly EditorInputState m_InputState = new();
		private Vector3 m_CursorWorldPosition;
		private TileGridCoord m_CursorTileGridCoord;

		private void OnSceneGUI()
		{
			m_InputState?.Update();

			//Debug.Log($"{Time.frameCount}: {Event.current.type}");
			switch (Event.current.type)
			{
				case EventType.MouseMove:
					break;
				case EventType.MouseDown:
					if (IsLeftMouseButtonDown())
					{
						SetTileAt();
						SetEventUsed(MouseButton.LeftMouse);
					}
					break;
				case EventType.MouseDrag:
					if (IsLeftMouseButtonDown())
					{
						// FIXME: prevent multiple draws on same coord
						SetTileAt();
						SetEventUsed(MouseButton.LeftMouse);
					}
					break;
				case EventType.ScrollWheel:
					break;
				case EventType.Repaint:
					UpdateTileCursorPosition(Event.current.mousePosition);
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

		private void SetTileAt()
		{
			var tileWorld = (TileWorld)target;
			tileWorld.ActiveLayer.SetTileAt(m_CursorTileGridCoord);
			EditorUtility.SetDirty(tileWorld);
		}

		private void SetEventUsed(MouseButton button) => Event.current.Use();

		private void UpdateTileCursorPosition(Vector2 mousePos)
		{
			var ray = HandleUtility.GUIPointToWorldRay(mousePos);
			if (Ray.IntersectsVirtualPlane(ray, out m_CursorWorldPosition))
			{
				var tileWorld = (TileWorld)target;
				var gridSize = tileWorld.ActiveLayer.Grid.Size;
				m_CursorTileGridCoord = new TileGridCoord(HandlesExt.SnapPointToGrid(m_CursorWorldPosition, gridSize));
				m_CursorTileGridCoord.y = 0;

				//Debug.Log($"Cursor: grid pos {m_CursorTileGridCoord}, world pos {m_CursorWorldPosition}");
			}
		}

		private void DrawTileCursor()
		{
			var tileWorld = (TileWorld)target;
			var activeLayer = tileWorld.ActiveLayer;
			var renderPos = activeLayer.GetTileWorldPosition(m_CursorTileGridCoord) + tileWorld.transform.position;
			var gridSize = (Vector3)activeLayer.Grid.Size;
			gridSize.y = activeLayer.TileCursorHeight;
			renderPos.y += gridSize.y * .5f;
			Handles.DrawWireCube(renderPos, gridSize);
		}

		private void AddDefaultControl()
		{
			var controlId = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
			HandleUtility.AddDefaultControl(controlId);
		}
	}
}