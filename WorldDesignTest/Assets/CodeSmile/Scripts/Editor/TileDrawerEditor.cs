// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.Scripts
{
	[CustomEditor(typeof(TileDrawer))]
	public class TileDrawerEditor : Editor
	{
		private readonly Vector3[] m_IntersectPoint = new Vector3[1];

		private void OnSceneGUI()
		{
			var evType = Event.current.type;
			if (evType == EventType.Ignore || evType == EventType.Used)
				return;

			var tileDrawer = (TileDrawer)target;
			//var pos = go.transform.position;
			//var size = HandleUtility.GetHandleSize(pos);

			var mousePos = Event.current.mousePosition;
			//Debug.Log($"[{Time.frameCount}]: {evType}");

			if (evType == EventType.Layout)
			{
				var controlId = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
				HandleUtility.AddDefaultControl(controlId);
			}
			else if (evType == EventType.Repaint)
			{
				var gridSize = EditorSnapSettings.gridSize;
				Handles.DrawWireCube(m_IntersectPoint[0], gridSize);
			}
			else
			{
				var canPlaceTile = false;
				var pickedObject = HandleUtility.PickGameObject(mousePos, false);
				if (pickedObject != null)
				{
					//Debug.Log($"picked {picked.name}");
					m_IntersectPoint[0] = pickedObject.transform.position;
					SnapIntersectPointToGrid();
					canPlaceTile = true;
				}
				else
				{
					var ray = HandleUtility.GUIPointToWorldRay(mousePos);
					if (Ray.IntersectsVirtualPlane(ray, out m_IntersectPoint[0]))
					{
						SnapIntersectPointToGrid();
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

						tileDrawer.CreateTileAt(m_IntersectPoint[0]);
						Event.current.Use();
					}
				}
			}
		}

		private void SnapIntersectPointToGrid()
		{
			var gridSize = EditorSnapSettings.gridSize;
			gridSize.y = 0f;
			//m_IntersectPoint[0] -= gridSize / 2f;
			Handles.SnapToGrid(m_IntersectPoint);
			//m_IntersectPoint[0] += gridSize / 2f;
		}
	}
}