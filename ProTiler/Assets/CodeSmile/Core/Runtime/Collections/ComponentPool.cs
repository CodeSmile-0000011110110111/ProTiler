// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Collections
{
	/// <summary>
	///     Pools a component of a prefab. This also works with GameObject instances except that
	///     you will have to pool Transform component instead and access GO via gameObject property.
	///     CAUTION: remember to call Dispose() or use a using() statement to ensure pooled instances
	///     get destroyed. Failure to do so will lead to stray objects in the pool's parent
	///     or throw an exception if the finalizer runs.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[FullCovered]
	public sealed class ComponentPool<T> : IDisposable where T : Component
	{
		private readonly List<T> m_AllInstances = new();
		private readonly List<T> m_InactiveInstances = new();
		private readonly Transform m_Parent;
		private readonly HideFlags m_HideFlags;
		private readonly GameObject m_Template;

		/// <summary>
		///     Number of all instances.
		/// </summary>
		public int Count => m_AllInstances.Count;
		/// <summary>
		///     Enumerable list of pooled instances, both active and inactive.
		/// </summary>
		public IReadOnlyList<T> AllInstances => m_AllInstances.AsReadOnly();

		/// <summary>
		///     Creates a new component pool.
		/// </summary>
		/// <param name="template">A prefab asset reference. Must not be null.</param>
		/// <param name="parent">The parent object instances will be parented to. Must not be null. Must be a scene object.</param>
		/// <param name="poolSize">Number of initial instances to create in the pool.</param>
		/// <param name="hideFlags">The HideFlags to apply to each instance.</param>
		/// <exception cref="ArgumentNullException">Thrown when prefab or parent is null.</exception>
		/// <exception cref="ArgumentException">Thrown when prefab is not a prefab asset, or parent is not a scene object.</exception>
		public ComponentPool(GameObject template, GameObject parent, int poolSize, HideFlags hideFlags = HideFlags.None)
		{
			if (template == null)
				throw new ArgumentNullException(nameof(template), "prefab must not be null");
			if (parent == null)
				throw new ArgumentNullException(nameof(parent), "parent must not be null");

			m_Template = template;
			m_Parent = parent.transform;
			m_HideFlags = hideFlags;

			SetPoolSize(poolSize);
		}

		/// <summary>
		///     Clears the pool and destroys all pooled instances, including active ones.
		/// </summary>
		public void Dispose() => Clear();

#if DEBUG
		[ExcludeFromCodeCoverage] ~ComponentPool()
		{
			if (Count != 0)
				throw new ObjectDisposedException($"a {nameof(ComponentPool<T>)} has not been disposed!");
		}
#endif

		/// <summary>
		///     Clears the pool and destroys all pooled instances, including active ones.
		/// </summary>
		public void Clear() => SetPoolSize(0);

		/// <summary>
		///     Get an inactive instance from the pool.
		/// </summary>
		/// <returns>An instance or null if there are no more inactive instances.</returns>
		public T GetFromPool()
		{
			if (m_InactiveInstances.Count == 0)
				return null;

			var lastIndex = m_InactiveInstances.Count - 1;
			var instance = m_InactiveInstances[lastIndex];
			m_InactiveInstances.RemoveAt(lastIndex);

			return instance;
		}

		/// <summary>
		///     Puts an active instance back into the pool.
		/// </summary>
		/// <param name="instance">The instance to return to the pool. Must not be null.</param>
		/// <exception cref="ArgumentNullException">thrown when instance is null</exception>
		public void ReturnToPool(T instance)
		{
#if DEBUG
			if (instance == null)
				throw new ArgumentNullException("instance must not be null");
#endif
			m_InactiveInstances.Add(instance);
		}

		/// <summary>
		/// Increments or decrements the number of instances the pool holds.
		/// This will either Instantiate new instances or Destroy excess objects.
		/// </summary>
		/// <param name="poolSize"></param>
		public void SetPoolSize(int poolSize)
		{
			if (poolSize != m_AllInstances.Count)
			{
				var newCount = poolSize - m_AllInstances.Count;

				if (newCount > 0)
					InstantiatePrefabs(newCount);
				else if (newCount < 0)
					DestroyObjects(Mathf.Abs(newCount));
			}
		}

		private void InstantiatePrefabs(int newCount)
		{
			for (var i = 0; i < newCount; i++)
			{
				var go = Object.Instantiate(m_Template, Vector3.zero, Quaternion.identity, m_Parent);
				go.hideFlags = m_HideFlags;

				// TODO: (maybe) this won't work with anything but components
				var component = go.GetComponent<T>();
				m_AllInstances.Add(component);
				m_InactiveInstances.Add(component);
			}
		}

		private void DestroyObjects(int newCount)
		{
			var removeIndex = Mathf.Max(0, m_AllInstances.Count - (newCount + 1));
			var toRemove = m_AllInstances.GetRange(removeIndex, newCount);
			m_AllInstances.RemoveRange(removeIndex, newCount);

			foreach (var remove in toRemove)
				remove.gameObject.DestroyInAnyMode();
		}
	}
}
