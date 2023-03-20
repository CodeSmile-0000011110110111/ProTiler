using System;
using System.Collections;
using System.Collections.Generic;
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
				var child = transform.GetChild(0);
				Debug.Log($"selected child: {child}");
				Selection.activeTransform = child;
			}
		}
	}
}
