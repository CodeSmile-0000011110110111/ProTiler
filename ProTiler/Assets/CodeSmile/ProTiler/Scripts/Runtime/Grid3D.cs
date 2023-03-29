// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.ProTiler
{
	public class Grid3D : MonoBehaviour
	{
		[SerializeField] private Vector3Int m_Size = new(1, 1, 1);
		[SerializeField] private Vector3 m_Gap;
		[SerializeField] private Grid3DLayout m_Layout;
		
		public Vector3Int Size { get => m_Size; set => m_Size = value; }
		public Vector3 Gap { get => m_Gap; set => m_Gap = value; }
		public Grid3DLayout Layout { get => m_Layout; set => m_Layout = value; }
		
		private void Awake() => Debug.Log("Grid3D: Awake");
		private void Reset() => Debug.Log("Grid3D: Reset");
		private void Start() => Debug.Log("Grid3D: Start");
		private void OnEnable() => Debug.Log("Grid3D: OnEnable");
	}
}