// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Tile
{
	public sealed class ObjectPool<T> : IDisposable where T : Object, new()
	{
		private readonly List<T> m_Instances = new();
		private readonly GameObject m_Prefab;
		private readonly Transform m_Parent;
		private readonly HideFlags m_HideFlags;

		public IReadOnlyList<T> Instances => m_Instances;
		public int Count => m_Instances.Count;

		public ObjectPool(GameObject prefab, Transform parent, int poolSize, HideFlags hideFlags = HideFlags.None)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent), "parent must not be null");

			m_Prefab = prefab;
			m_Parent = parent;
			m_HideFlags = hideFlags;

			UpdatePoolSize(poolSize);
		}

		public void Dispose() => UpdatePoolSize(0);

		public void UpdatePoolSize(int poolSize)
		{
			if (poolSize != m_Instances.Count)
			{
				var newCount = poolSize - m_Instances.Count;

				if (newCount > 0)
					InstantiateGameObjects(newCount);
				else if (newCount < 0)
					DestroyObjects(math.abs(newCount));
			}
		}

		private void InstantiateGameObjects(int newCount)
		{
			for (var i = 0; i < newCount; i++)
			{
				var go = Object.Instantiate(m_Prefab, Vector3.zero, quaternion.identity, m_Parent) as T;
				go.hideFlags = m_HideFlags;
				(go as GameObject).SetActive(false);
				m_Instances.Add(go);
			}
		}

		private void DestroyObjects(int newCount)
		{
			var removeIndex = math.max(0, m_Instances.Count - (newCount + 1));
			var toRemove = m_Instances.GetRange(removeIndex, newCount);
			m_Instances.RemoveRange(removeIndex, newCount);

			foreach (var gameObject in toRemove)
				gameObject.DestroyInAnyMode();
		}
	}
}