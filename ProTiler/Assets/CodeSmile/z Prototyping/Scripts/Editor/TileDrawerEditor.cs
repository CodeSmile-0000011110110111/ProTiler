// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;
using UnityEditor_Handles = UnityEditor.Handles;

namespace CodeSmile.Editor
{
	[CustomEditor(typeof(TileDrawer))]
	public class TileDrawerEditor : UnityEditor.Editor
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
				UnityEditor_Handles.DrawWireCube(m_IntersectPoint[0], gridSize);
			}
			/*
			else if (evType == EventType.MouseMove)
			{
				var go = HandleUtility.PickGameObject(Event.current.mousePosition, false);
				if (go != null)
					Debug.Log($"picked: {go.name}");
			}*/
			else
			{
				var canPlaceTile = false;
				var pickedObject = HandleUtility.PickGameObject(mousePos, false);
				if (pickedObject != null)
				{
					//Debug.Log($"picked {picked.name}");
					m_IntersectPoint[0] = pickedObject.transform.position;
					Handles.SnapToGrid(m_IntersectPoint);
					canPlaceTile = true;
				}
				else
				{
					/*
					var ray = HandleUtility.GUIPointToWorldRay(mousePos);
					if (ray.IntersectsPlane(out m_IntersectPoint[0]))
					{
						HandlesExt.SnapIntersectPointsToGrid(m_IntersectPoint);
						canPlaceTile = true;
						//Debug.Log($"hit virtual plane at {m_IntersectPoint[0]}");
					}
					*/
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
	}
}