// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Tile
{
	public sealed class GameObjectPool : IDisposable
	{
		private readonly List<GameObject> m_GameObjects = new();
		private readonly GameObject m_Prefab;
		private readonly Transform m_Parent;
		private readonly HideFlags m_HideFlags;

		public IReadOnlyList<GameObject> GameObjects => m_GameObjects;
		public int Count => m_GameObjects.Count;

		public GameObjectPool(GameObject prefab, Transform parent, int poolSize, HideFlags hideFlags = HideFlags.None)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent), "parent must not be null");
			
			m_Prefab = prefab;
			m_Parent = parent;
			m_HideFlags = hideFlags;
			
			UpdatePoolSize(poolSize);
		}

		public void UpdatePoolSize(int poolSize)
		{
			if (poolSize != m_GameObjects.Count)
			{
				var newCount = poolSize - m_GameObjects.Count;

				if (newCount > 0)
					InstantiateObjects(newCount);
				else if (newCount < 0)
					DestroyObjects(math.abs(newCount));
			}
		}

		private void InstantiateObjects(int newCount)
		{
			for (var i = 0; i < newCount; i++)
			{
				var go = Object.Instantiate(m_Prefab, Vector3.zero, quaternion.identity, m_Parent);
				go.hideFlags = m_HideFlags;
				go.SetActive(false);
				m_GameObjects.Add(go);
			}
		}

		private void DestroyObjects(int newCount)
		{
			var removeIndex = math.max(0, m_GameObjects.Count - (newCount + 1));
			var toRemove = m_GameObjects.GetRange(removeIndex, newCount);
			m_GameObjects.RemoveRange(removeIndex, newCount);

			foreach (var gameObject in toRemove)
				gameObject.DestroyInAnyMode();
		}

		public void Dispose()
		{
			UpdatePoolSize(0);
		}
	}
}