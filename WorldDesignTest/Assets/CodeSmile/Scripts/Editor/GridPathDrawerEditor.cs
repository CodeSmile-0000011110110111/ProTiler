// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.Scripts
{
	[CustomEditor(typeof(GridPathDrawer))]
	public class GridPathDrawerEditor : Editor
	{
		private void OnSceneGUI()
		{
			var pathDrawer = (GridPathDrawer)target;
			var path = pathDrawer.PathPoints.ToArray();
			Handles.DrawAAPolyLine(path);
			
			Handles.color = Handles.elementPreselectionColor;
			var pos = pathDrawer.transform.position;
			var size = pathDrawer.GridSettings.TileSize.x / 5f;
			for (int i = 0; i < path.Length; i++)
			{
				Handles.CylinderHandleCap(i, path[i], Quaternion.Euler(new Vector3(90f,0f,0f)), size, EventType.Repaint);
			}
		}
	}
}