// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

public class SelectFirstChild : MonoBehaviour
{
	public bool m_SelectFirstChild;

	private void OnValidate()
	{
		if (m_SelectFirstChild)
		{
			m_SelectFirstChild = false;

			if (transform.childCount > 0)
			{
#if UNITY_EDITOR
				var child = transform.GetChild(0);
				Debug.Log($"selected child: {child}");
				Selection.activeTransform = child;
#endif
			}
		}
	}
}