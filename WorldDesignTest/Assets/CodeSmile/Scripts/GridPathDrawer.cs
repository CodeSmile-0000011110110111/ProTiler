// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile.Scripts
{
	[ExecuteInEditMode]
	public class GridPathDrawer : MonoBehaviour
	{
		[SerializeField] private List<Vector3> m_PathPoints = new();
		[SerializeField] private Grid m_GridSettings = new() { TileSize = new Vector2Int(30, 30) };

		private void Awake() => ResetPath();

		public List<Vector3> PathPoints => m_PathPoints;
		public Grid GridSettings => m_GridSettings;

		public void ResetPath()
		{
			Debug.Log("ResetPath");
			m_PathPoints.Clear();
			m_PathPoints.Add(Vector3.left * m_GridSettings.TileSize.x);
			m_PathPoints.Add(Vector3.right * m_GridSettings.TileSize.x);
			m_PathPoints.Add(Vector3.right * m_GridSettings.TileSize.x + Vector3.forward * m_GridSettings.TileSize.y);
			m_PathPoints.Add(Vector3.left * m_GridSettings.TileSize.x + Vector3.forward * m_GridSettings.TileSize.y);
		}

		public struct Grid
		{
			public Vector2Int TileSize;
		}
	}
}