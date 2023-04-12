// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace CodeSmile.Collections
{
	/// <summary>
	///     Contains UnityEngine.Object instances so they can be accessed by the same index in both editor and runtime.
	///     Fault tolerant: Index out of bounds access returns a default object rather than throwing exceptions.
	///     Note: GetInstanceID cannot be used as an index because its value may differ between Editor and Runtime.
	/// </summary>
	[Serializable]
	public class ObjectSet<T> where T : Object
	{
		public T DefaultObject;
		private int m_NextIndex;
		private Dictionary<int, T> m_IndexedObjects = new();
		//private HashSet<ObjectIndexPair> m_ObjectIndexPairs;

		public T this[int index]
		{
			get => m_IndexedObjects.TryGetValue(index, out var existingObject) ? existingObject : DefaultObject;
			set => m_IndexedObjects[index] = Contains(index) ? value : throw new KeyNotFoundException($"index {index} does not exist in set");
		}

		public int Count => m_IndexedObjects.Count;

		public ObjectSet()
			: this(null) {}

		public ObjectSet(T defaultObject) => DefaultObject = defaultObject;

		public bool Add(T item) => Add(item, out var _);

		public bool Add(T item, out int index)
		{
			if (Contains(item))
			{
				index = m_IndexedObjects.FirstOrDefault(x => x.Value == item).Key;
				return false;
			}

			m_IndexedObjects.Add(m_NextIndex, item);
			index = m_NextIndex;
			m_NextIndex++;
			return true;
		}

		public void Clear() => m_IndexedObjects.Clear();

		public bool Contains(int index) => m_IndexedObjects.ContainsKey(index);
		public bool Contains(T item) => m_IndexedObjects.ContainsValue(item);

		public bool Remove(T item)
		{
			foreach (var kvp in m_IndexedObjects)
			{
				if (kvp.Value.Equals(item))
				{
					m_IndexedObjects.Remove(kvp.Key);
					return true;
				}
			}
			return false;
		}

		public bool Remove(int index) => m_IndexedObjects.Remove(index);

		/*
		private sealed class ObjectIndexPair : IEquatable<T>
		{
			public T Object;
			public int Index;
			public bool Equals(T other) => EqualityComparer<T>.Default.Equals(Object, other);
			public override bool Equals(object obj) => obj is T other && Equals(other);
			public override int GetHashCode() => Object != null ? Object.GetHashCode() : 0;
		}
		*/
	}
}
