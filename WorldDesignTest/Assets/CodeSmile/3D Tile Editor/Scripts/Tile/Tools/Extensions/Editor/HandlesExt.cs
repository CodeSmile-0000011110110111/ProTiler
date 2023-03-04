// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.UnityEditor
{
	public static class HandlesExt
	{
		public static Vector3 SnapPointToGrid(Vector3 point, Vector3Int grid)
		{
			// multiply by inverse to avoid potential "div by zero"
			return new Vector3((int)(point.x * (1f / grid.x)), (int)(point.y * (1f / grid.y)), (int)(point.z * (1f / grid.z)));
		}
		
		public static void SnapIntersectPointsToGrid(Vector3[] intersectPoint)
		{
			//var gridSize = EditorSnapSettings.gridSize;
			//gridSize.y = 0f;
			//m_IntersectPoint[0] -= gridSize / 2f;
			Handles.SnapToGrid(intersectPoint);
			//m_IntersectPoint[0] += gridSize / 2f;
		}
	}
}