// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.ProTiler
{
	public class Tilemap3DRenderer : MonoBehaviour
	{
		//[SerializeField] private Vector3Int m_ChunkSize = new(32, 0, 32); // set this up-front (project setting?)
		[SerializeField] private int m_DrawDistance;
	}
}