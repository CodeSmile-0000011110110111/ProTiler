// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Pooling
{
	public sealed class ComponentPool<T> : IDisposable where T : Component, new()
	{
		[NonSerialized] private readonly List<T> m_AllInstances = new();
		[NonSerialized] private readonly List<T> m_InactiveInstances = new();
		[NonSerialized] private readonly GameObject m_Prefab;
		[NonSerialized] private readonly Transform m_Parent;
		[NonSerialized] private readonly HideFlags m_HideFlags;

		public int Count => m_AllInstances.Count;
		public IReadOnlyList<T> AllInstances => m_AllInstances.AsReadOnly();
		public IReadOnlyList<T> InactiveInstances => m_InactiveInstances.AsReadOnly();

		public ComponentPool(GameObject prefab, Transform parent, int poolSize, HideFlags hideFlags = HideFlags.None)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent), "parent must not be null");

			m_Prefab = prefab;
			m_Parent = parent;
			m_HideFlags = hideFlags;

			UpdatePoolSize(poolSize);
		}

		public void Dispose() => UpdatePoolSize(0);

		internal void UpdatePoolSize(int poolSize)
		{
			if (poolSize != m_AllInstances.Count)
			{
				var newCount = poolSize - m_AllInstances.Count;

				if (newCount > 0)
					InstantiatePrefabs(newCount);
				else if (newCount < 0)
					DestroyObjects(math.abs(newCount));
			}
		}

		private void InstantiatePrefabs(int newCount)
		{
			for (var i = 0; i < newCount; i++)
			{
				var go = Object.Instantiate(m_Prefab, Vector3.zero, quaternion.identity, m_Parent);
				go.hideFlags = m_HideFlags;
				go.SetActive(false);

				// FIXME: this won't work with anything but components
				m_AllInstances.Add(go.GetComponent<T>());
				m_InactiveInstances.Add(go.GetComponent<T>());
			}
		}

		private void DestroyObjects(int newCount)
		{
			var removeIndex = math.max(0, m_AllInstances.Count - (newCount + 1));
			var toRemove = m_AllInstances.GetRange(removeIndex, newCount);
			m_AllInstances.RemoveRange(removeIndex, newCount);

			foreach (var gameObject in toRemove)
				gameObject.DestroyInAnyMode();
		}

		public T GetPooledObject(bool setActive = true)
		{
			if (m_InactiveInstances.Count == 0)
				throw new Exception("no more objects in pool");
			
			var lastIndex = m_InactiveInstances.Count - 1;
			var instance = m_InactiveInstances[lastIndex];
			m_InactiveInstances.RemoveAt(lastIndex);

			if (setActive)
				instance.gameObject.SetActive(true);

			return instance;
		}

		public void ReturnToPool(T instance, bool setInactive = true)
		{
#if DEBUG
			if (instance == null)
				throw new ArgumentNullException("instance must not be null");
			if (m_InactiveInstances.Contains(instance))
				throw new ArgumentException("instance already returned to pool");
#endif

			if (setInactive)
				instance.gameObject.SetActive(false);

			m_InactiveInstances.Add(instance);
		}
	}
}