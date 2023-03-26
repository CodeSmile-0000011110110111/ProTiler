// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEngine;
using UnityEditor_Handles = UnityEditor.Handles;

namespace CodeSmile.Editor
{
	[CustomEditor(typeof(GridPathDrawer))]
	public class GridPathDrawerEditor : UnityEditor.Editor
	{
		private int m_ClosestLineIndex;

		private void OnSceneGUI()
		{
			// create a temp plane to be able to click on "empty space" (XZ plane with y=0)
			//m_DistanceFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - m_DistanceZ);
			//m_Plane = new Plane(Vector3.forward, m_DistanceFromCamera);

			m_ClosestLineIndex = GetClosestLineIndex(30f);
			DrawPathLineHandles();
			DrawPathLines();
			DrawPathPoints();

			var ev = Event.current;
			switch (ev.type)
			{
				case EventType.Layout:
					//HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

					break;
				case EventType.Repaint:
					break;

				case EventType.MouseDown:
					break;
				case EventType.MouseUp:
					break;
				case EventType.MouseMove:
					break;
				case EventType.MouseDrag:
					break;
				case EventType.KeyDown:
					break;
				case EventType.KeyUp:
					break;
				case EventType.ScrollWheel:
					break;
				case EventType.DragUpdated:
					break;
				case EventType.DragPerform:
					break;
				case EventType.DragExited:
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
					break;
				case EventType.MouseEnterWindow:
					break;
				case EventType.MouseLeaveWindow:
					break;
				case EventType.TouchDown:
					break;
				case EventType.TouchUp:
					break;
				case EventType.TouchMove:
					break;
				case EventType.TouchEnter:
					break;
				case EventType.TouchLeave:
					break;
				case EventType.TouchStationary:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void DrawPathLineHandles()
		{
			var pathDrawer = (GridPathDrawer)target;
			var path = pathDrawer.TestPath.ToArray();
			for (var i = 0; i < path.Length - 1; i++)
			{
				var line = path[i + 1] - path[i];
				var left = Vector3.Cross(line, Vector3.up);
				var lineCenter = path[i] + line / 2f;
				var size = pathDrawer.GridSettings.TileSize.x ;
				DrawPathLineDragHandle(i, lineCenter, left, size);
				//DrawPathLineDragHandle(i, lineCenter, left * -1f, size);
			}
		}

		private void DrawPathLineDragHandle(int pointIndex, Vector3 position, Vector3 direction, float size)
		{
			EditorGUI.BeginChangeCheck();
			var initialValue = 1f;
			var snap = 1f;
			var change = UnityEditor_Handles.ScaleValueHandle(initialValue, position, Quaternion.identity, size, 
				(id, position, rotation, size, type) =>
			{
				UnityEditor_Handles.ArrowHandleCap(id, position, rotation, size, EventType.Repaint);
				UnityEditor_Handles.ArrowHandleCap(id, position, rotation, size, EventType.Layout);
			}, snap);
			
			if (initialValue - change != 0f)
				Debug.Log("value change: " + change);

			/*
			var newTargetPosition = Handles.Slider(position, direction, size,
				(id, position, rotation, size, type) =>
				{
					Handles.ArrowHandleCap(id, position, rotation, size, EventType.Repaint);
					Handles.ArrowHandleCap(id, position, rotation, size, EventType.Layout);
				}, 1f);

			if (EditorGUI.EndChangeCheck())
			{
				var pathDrawer = (GridPathDrawer)target;
				Undo.RecordObject(pathDrawer, "Move Path Line");

				pathDrawer.Path.MoveEdge(pointIndex, newTargetPosition, direction);
				Debug.Log($"moved to: {newTargetPosition}");
			}
			*/
		}

		private void ProcessMouseDownEvent()
		{
			var ev = Event.current;
			if (ev.type != EventType.MouseDown || ev.button != 0)
				return;

			var ray = HandleUtility.GUIPointToWorldRay(ev.mousePosition);
			if (Physics.Raycast(ray, out var hitInfo))
				Debug.Log($"mouse {ev.mousePosition}, ray {ray}, hit {hitInfo.point} => {hitInfo.transform.name}");

			ev.Use();
		}

		private int GetClosestLineIndex(float minDistance)
		{
			var pathDrawer = (GridPathDrawer)target;
			var points = pathDrawer.TestPath;
			var closestDistance = float.MaxValue;
			var closestIndex = -1;
			for (var i = 0; i < points.Count - 1; i++)
			{
				var distance = HandleUtility.DistanceToLine(points[i], points[i + 1]);
				if (distance < closestDistance && distance <= minDistance)
				{
					closestDistance = distance;
					closestIndex = i;
				}
			}

			return closestIndex;
		}

		private void DrawPathPoints()
		{
			var pathDrawer = (GridPathDrawer)target;
			var path = pathDrawer.TestPath;
			UnityEditor_Handles.color = UnityEditor_Handles.elementPreselectionColor;
			var size = 1f;
			for (var i = 0; i < path.Count; i++)
				UnityEditor_Handles.SphereHandleCap(i, path[i], Quaternion.Euler(new Vector3(90f, 0f, 0f)), size, EventType.Repaint);
			//Handles.CylinderHandleCap(i, path[i], Quaternion.Euler(new Vector3(90f, 0f, 0f)), size, EventType.Repaint);
		}

		private void DrawPathLines()
		{
			var pathDrawer = (GridPathDrawer)target;
			var path = pathDrawer.TestPath;
			for (var i = 0; i < path.Count - 1; i++)
			{
				UnityEditor_Handles.color = m_ClosestLineIndex == i ? UnityEditor_Handles.selectedColor : UnityEditor_Handles.elementColor;
				UnityEditor_Handles.DrawLine(path[i], path[i + 1], 5f);
			}
		}
	}
}