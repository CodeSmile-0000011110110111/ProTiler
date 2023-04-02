using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabReference : MonoBehaviour
{
	public GameObject m_Prefab;
	public int m_Index = -1;

	private void OnValidate()
	{
		if (m_Prefab != null)
			Debug.Log($"{m_Prefab.name} has instance ID: {m_Prefab.GetInstanceID()}");
	}
}
