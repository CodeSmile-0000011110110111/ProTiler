// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.ProTiler.Grid
{
	[RequireComponent(typeof(Transform))]
	[DisallowMultipleComponent]
	public class Grid3DBehaviour : MonoBehaviour
	{
		[SerializeField] private Vector3 m_CellSize = new(1f, 1f, 1f);
		[SerializeField] private Vector3 m_CellGap;
		[SerializeField] private CellLayout m_CellLayout;
		public Vector3 CellSize { get => m_CellSize; set => m_CellSize = value; }
		public Vector3 CellGap { get => m_CellGap; set => m_CellGap = value; }
		public CellLayout CellLayout { get => m_CellLayout; set => m_CellLayout = value; }
	}
}
