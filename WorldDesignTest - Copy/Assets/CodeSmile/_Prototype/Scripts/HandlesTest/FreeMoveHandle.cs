// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.HandlesTest
{
	public enum CapType
	{
		Arrow,
		Circle,
		Cone,
		Cube,
		Cylinder,
		Dot,
		Rectangle,
		Sphere,
		CustomCapFunction,
	}

	[ExecuteInEditMode]
	public class FreeMoveHandle : MonoBehaviour
	{
		[Header("Handle Settings")]
		public bool MoveGameObject = true;
		public bool UseGetHandleSize = true;
		public float HandleSize = 1f;
		public CapType HandleCap;
		//public Vector3 SnapVector = Vector3.one;

		[Header("Modified Values")]
		public Vector3 FreeMovePosition;
	}
}