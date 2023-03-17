// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.EditorTests
{
	[ExecuteInEditMode]
	public class ContinuousInstantiateDestroyCycleTest : MonoBehaviour
	{
		[SerializeField] private GameObject m_Prefab;
		[SerializeField] private int m_GameObjectCount;
		[SerializeField] private float m_DelayBeforeInstantiate;
		[SerializeField] private float m_DelayBeforeDestroy;

		private float m_NextTime;

		private void Start()
		{
			Debug.Log(nameof(ContinuousInstantiateDestroyCycleTest));
			m_NextTime = Time.realtimeSinceStartup + m_DelayBeforeInstantiate;
		}

		private void OnRenderObject()
		{
			if (Time.realtimeSinceStartup > m_NextTime)
			{
				if (transform.childCount > 0)
					DestroyObjects();
				else
					InstantiateObjects();
			}
		}

		private void InstantiateObjects()
		{
			Debug.Log($"Instantiating {m_GameObjectCount}");
			m_NextTime = Time.realtimeSinceStartup + m_DelayBeforeDestroy;

			for (var i = 0; i < m_GameObjectCount; i++)
				Instantiate(m_Prefab, new Vector3(i, i, i), Quaternion.identity, transform);

			//StartCoroutine(new WaitForSecondsElapsed(m_DelayBeforeDestroy, DestroyObjects, true));
		}

		private void DestroyObjects()
		{
			Debug.Log("Destroying all children");
			m_NextTime = Time.realtimeSinceStartup + m_DelayBeforeInstantiate;
			transform.DestroyAllChildren();
			//StartCoroutine(new WaitForSecondsElapsed(m_DelayBeforeInstantiate, InstantiateObjects, true));
		}
	}
}