// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.UnityEditor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmile.Tile.UnityEditor
{
	public sealed partial class EditorInputState
	{
		[CustomEditor(typeof(TileWorld))]
		public class TileWorldDrawer : Editor
		{
			private readonly EditorInputState m_InputState = new();
			private Vector3 m_CursorWorldPosition;
			private Vector3 m_CursorGridPosition;

			private bool m_LeftMouseButtonDown;

			private void OnSceneGUI()
			{
				m_InputState?.Update();

				//Debug.Log(Event.current.type);

				switch (Event.current.type)
				{
					case EventType.MouseMove:
						break;
					case EventType.MouseDown:
						if (m_InputState.IsButtonDown(MouseButton.LeftMouse))
							Event.current.Use();
						break;
					case EventType.MouseDrag:
						UpdateTileCursorPosition(Event.current.mousePosition);
						TryDrawBrush();
						DrawTileCursor();
						if (m_InputState.IsButtonDown(MouseButton.LeftMouse))
							Event.current.Use();
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

				/*
				else if (evType == EventType.MouseMove)
				{
					var go = HandleUtility.PickGameObject(Event.current.mousePosition, false);
					if (go != null)
						Debug.Log($"picked: {go.name}");
				}*/
				{
					/*
					var canPlaceTile = false;
					var pickedObject = HandleUtility.PickGameObject(mousePos, false);
					if (pickedObject != null)
					{
						//Debug.Log($"picked {picked.name}");
						m_IntersectPoint[0] = pickedObject.transform.position;
						HandlesExt.SnapIntersectPointsToGrid(m_IntersectPoint);
						canPlaceTile = true;
					}
					else
					{
						var ray = HandleUtility.GUIPointToWorldRay(mousePos);
						if (Ray.IntersectsVirtualPlane(ray, out m_IntersectPoint[0]))
						{
							HandlesExt.SnapIntersectPointsToGrid(m_IntersectPoint);
							canPlaceTile = true;
							//Debug.Log($"hit virtual plane at {m_IntersectPoint[0]}");
						}
					}
	
					if (canPlaceTile)
					{
						if (Event.current.button == 0 && (evType == EventType.MouseDown || evType == EventType.MouseDrag ||
						                                  evType == EventType.TouchDown || evType == EventType.TouchMove))
						{
							Debug.Log($"[{Time.frameCount}]: {evType}");
							HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
	
							// delete what's there
							if (pickedObject != null)
							{
								DestroyImmediate(pickedObject);
								pickedObject = null;
							}
	
							tileWorld.CreateTileAt(m_IntersectPoint[0]);
							Event.current.Use();
						}
					}
					*/
				}
			}

			private void TryDrawBrush()
			{
				if (m_LeftMouseButtonDown)
				{
					var tileWorld = (TileWorld)target;
					tileWorld.DrawTile(m_CursorGridPosition);
				}
			}

			private void UpdateTileCursorPosition(Vector2 mousePos)
			{
				var ray = HandleUtility.GUIPointToWorldRay(mousePos);
				if (Ray.IntersectsVirtualPlane(ray, out m_CursorWorldPosition))
				{
					var gridSize = ((TileWorld)target).Grid.Size;
					m_CursorGridPosition = HandlesExt.SnapPointToGrid(m_CursorWorldPosition, gridSize);

					//Debug.Log($"Cursor: grid pos {m_CursorGridPosition}, world pos {m_CursorWorldPosition}");
				}
			}

			private void DrawTileCursor()
			{
				var tileWorld = (TileWorld)target;
				var gridSize = ((TileWorld)target).Grid.Size;
				var gridPos = m_CursorGridPosition;
				var renderPos = Vector3.zero;

				switch (tileWorld.TilePivot)
				{
					case TilePivot.Center:
						renderPos = new Vector3(gridPos.x * gridSize.x + gridSize.x * .5f,
							gridPos.y * gridSize.y + gridSize.y * .5f,
							gridPos.z * gridSize.z + gridSize.z * .5f);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				Handles.DrawWireCube(renderPos, gridSize);
			}

			private void AddDefaultControl()
			{
				var controlId = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
				HandleUtility.AddDefaultControl(controlId);
			}
		}
	}
}