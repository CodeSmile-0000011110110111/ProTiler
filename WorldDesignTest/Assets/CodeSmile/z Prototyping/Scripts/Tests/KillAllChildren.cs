// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile;
using UnityEngine;

public class KillAllChildren : MonoBehaviour
{
	public bool m_GetRidOfThem;

	private void OnValidate()
	{
		if (m_GetRidOfThem)
		{
			m_GetRidOfThem = false;

			StartCoroutine(new WaitForFramesElapsed(1, () =>
			{
				for (var i = transform.childCount - 1; i >= 0; i--)
				{
					var child = transform.GetChild(i);
					Debug.Log($"destroying {child}");
					DestroyImmediate(child.gameObject);
				}
			}));
		}
	}
}