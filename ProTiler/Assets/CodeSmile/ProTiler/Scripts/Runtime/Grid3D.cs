// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.ProTiler
{

	[RequireComponent(typeof(Transform))]
	public class Grid3D : MonoBehaviour
	{
		public enum Layout
		{
			Rectangular,
			Hexagonal,
		}

		[SerializeField] private Vector3Int m_CellSize = new(1, 1, 1);
		[SerializeField] private Vector3 m_CellGap;
		[SerializeField] private Layout m_CellLayout;
		public Vector3Int CellSize { get => m_CellSize; set => m_CellSize = value; }
		public Vector3 CellGap { get => m_CellGap; set => m_CellGap = value; }
		public Layout CellLayout { get => m_CellLayout; set => m_CellLayout = value; }
	}
}
