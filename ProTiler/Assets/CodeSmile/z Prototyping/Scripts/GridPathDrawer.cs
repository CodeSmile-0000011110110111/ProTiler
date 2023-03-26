// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;
using UnityEngine.Serialization;

namespace CodeSmile
{
	[ExecuteInEditMode]
	public class GridPathDrawer : MonoBehaviour
	{
		[SerializeField] private TestPath m_TestPath = new();

		[SerializeField] private bool m_ResetPath;

		[SerializeField] private Grid m_GridSettings = new() { TileSize = new Vector2Int(30, 30) };

		private void Awake() => ResetPath();

		private void OnValidate()
		{
			if (m_ResetPath)
			{
				m_ResetPath = false;
				ResetPath();
			}
		}

		public TestPath TestPath => m_TestPath;
		public Grid GridSettings => m_GridSettings;

		public void ResetPath()
		{
			//Debug.Log("ResetPath");
			m_TestPath.Clear();
			m_TestPath.Add(Vector3.left * m_GridSettings.TileSize.x);
			m_TestPath.Add(Vector3.right * m_GridSettings.TileSize.x);
			m_TestPath.Add(Vector3.right * m_GridSettings.TileSize.x + Vector3.forward * m_GridSettings.TileSize.y);
			m_TestPath.Add(Vector3.left * m_GridSettings.TileSize.x + Vector3.forward * m_GridSettings.TileSize.y);
		}

		public void AddPoint(Vector3 point) => m_TestPath.Add(point);

		/*
		public void SetPointsAtIndex(int pointIndex, Vector3 position, Vector3 direction)
		{
			if (direction == Vector3.zero)
				return;

			Debug.Log($"dir: {direction}");
			for (var i = pointIndex; i < pointIndex + 1; i++)
			{
				var pos = position;
				if (direction.x != 0f)
					pos = new Vector3(position.x, 0f, 0f);
				if (direction.y != 0f)
					pos = new Vector3(0f, position.y, 0f);
				if (direction.z != 0f)
					pos = new Vector3(0f, 0f, position.z);

				m_Path[i] = pos;
			}
		}
		*/

		public struct Grid
		{
			public Vector2Int TileSize;
		}
	}
}