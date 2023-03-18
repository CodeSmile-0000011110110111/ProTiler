// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.EditorTests
{
	public class CalculatedField : MonoBehaviour
	{
		private const float TargetFramerate = 60f;
		
		[SerializeField] private int m_Ticks;
		[SerializeField] [ReadOnlyField] private float m_CalculatedSeconds;

		private void OnValidate() => m_CalculatedSeconds = m_Ticks / TargetFramerate;
	}
}