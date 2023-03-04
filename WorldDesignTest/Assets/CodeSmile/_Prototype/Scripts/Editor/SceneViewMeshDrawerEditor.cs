// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.EditorTests;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.UnityEditor
{
	[CustomEditor(typeof(SceneViewMeshDrawer))]
	public class SceneViewMeshDrawerEditor : Editor
	{
		private void OnSceneGUI()
		{
			var sceneCamera = Camera.current;
			if (sceneCamera != null)
			{
				var mousePos = Event.current.mousePosition;

				var window = EditorWindow.GetWindow<TileInspectorTest>();
				if (window != null)
					window.UpdateTileGridPos(mousePos, (target as SceneViewMeshDrawer).transform.position);
				else
				{
					var ray = HandleUtility.GUIPointToWorldRay(mousePos);
					if (Ray.IntersectsVirtualPlane(ray, out var intersectPoint))
					{
						var gridSize = new Vector3Int(30, 1, 30);
						var gridPoint = HandlesExt.SnapPointToGrid(intersectPoint, gridSize);
						Debug.Log($"hit: {gridPoint} from intersect point {intersectPoint}");
					}
				}
			}
		}
	}
}