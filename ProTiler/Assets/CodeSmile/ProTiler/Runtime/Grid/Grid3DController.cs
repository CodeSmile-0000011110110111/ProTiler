// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System.Diagnostics.Contracts;
using UnityEngine;
using CellSize = UnityEngine.Vector3;
using CellGap = UnityEngine.Vector3;

namespace CodeSmile.ProTiler.Grid
{
	[FullCovered]
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Transform))]
	public class Grid3DController : MonoBehaviour
	{
		[SerializeField] private Grid3D m_Grid;

		[SerializeField] private CellSize m_CellSize = new(1f, 1f, 1f);
		[SerializeField] private CellGap m_CellGap;
		[SerializeField] private CellLayout m_CellLayout;
		public CellSize CellSize { [Pure] get => m_CellSize; set => m_CellSize = value; }
		public CellGap CellGap { [Pure] get => m_CellGap; set => m_CellGap = value; }
		public CellLayout CellLayout { [Pure] get => m_CellLayout; set => m_CellLayout = value; }
	}
}
