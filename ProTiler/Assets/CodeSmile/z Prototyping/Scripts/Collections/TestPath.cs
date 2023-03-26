// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeSmile
{
	[Serializable]
	public class TestPath
	{
		[SerializeField] private List<Vector3> m_Points = new();
		public List<Vector3> Points => m_Points;

		public void MoveEdge(int edgeIndex, float offset, Side side)
		{
			
		}

		public void Clear() => m_Points.Clear();

		public void Add(in Vector3 point) => m_Points.Add(point);

		public void AddRange(in List<Vector3> points) => m_Points.AddRange(points);

		public Vector3 this[int index] { get => m_Points[index]; set => m_Points[index] = value; }

		public int Count => m_Points.Count;

		public Vector3[] ToArray() => m_Points.ToArray();
	}
}