// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace CodeSmile.EditorTests
{
	[ExecuteInEditMode]
	public class TestInstantiateDestroyCycle : MonoBehaviour
	{
		[SerializeField] private bool m_Update;
		[SerializeField] private bool m_SetHideFlags;
		[SerializeField] private bool m_SetActiveState;

		[SerializeField] private GameObject m_Prefab;
		[SerializeField] private int m_HowMany = 100;

		private readonly List<GameObject> m_Instances = new();

		private static void ClearHideFlags(GameObject instance)
		{
			if (instance.IsMissing() == false)
				instance.hideFlags = HideFlags.None;
		}

		private static void SetHideFlags(GameObject instance)
		{
			if (instance.IsMissing() == false)
				instance.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		}

		private static void SetInactive(GameObject instance)
		{
			if (instance.IsMissing() == false)
				instance.SetActive(false);
		}

		private static void SetActive(GameObject instance)
		{
			if (instance.IsMissing() == false)
				instance.SetActive(true);
		}

		private void OnValidate()
		{
			if (m_Update)
			{
				m_Update = false;
				StartCoroutine(InstantiateAndDestroyAfterDelay());
			}
		}

		private IEnumerator InstantiateAndDestroyAfterDelay()
		{
			yield return null;

			InstantiateAndDestroy();
		}

		private void DestroyAllChildrenAndHumanityAsWeKnowIt()
		{
			Profiler.BeginSample("DestroyAllChildren");
			transform.DestroyAllChildren();
			Profiler.EndSample();
		}


		private void InstantiateAndDestroy()
		{
			if (m_Prefab == null)
				return;

			if (m_SetHideFlags == false || m_Instances.Count != m_HowMany)
			{
				DestroyAllChildrenAndHumanityAsWeKnowIt();
				InstantiateInstances();
			}

			foreach (var instance in m_Instances)
				SetHideFlags(instance);

			StartCoroutine(Wait(1f, () => { DestroyInstances(); }));
		}

		private void InstantiateInstances()
		{
#if UNITY_EDITOR
			Profiler.BeginSample("InstantiateInstances");
			for (var i = 0; i < m_HowMany; i++)
			{
				var instance = PrefabUtility.InstantiatePrefab(m_Prefab, transform) as GameObject;
				m_Instances.Add(instance);
			}
			Profiler.EndSample();
#endif
		}

		private void DestroyInstances()
		{
			if (m_SetHideFlags == false)
			{
				Profiler.BeginSample("DestroyInstances");
				for (var i = 0; i < m_Instances.Count; i++)
					DestroyImmediate(m_Instances[i]);

				m_Instances.Clear();
				Profiler.EndSample();
			}

			foreach (var instance in m_Instances)
				SetHideFlags(instance);
		}

		private IEnumerator Wait(float seconds, Action callback)
		{
			yield return null;

			callback.Invoke();
		}
	}
}