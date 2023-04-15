// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace CodeSmile.Collections
{
	/// <summary>
	/// Contains UnityEngine.Object instances so they can be accessed by the same index in both editor and runtime.
	/// It does *not* use GetInstanceID for indexes because the InstanceID differs between Editor and PlayMode.
	///
	///	Unorthodox Behaviour:
	///		Index out of bounds access returns DefaultObject rather than throwing exceptions.
	///		Will not store null references (throws exception).
	/// </summary>
	[Serializable]
	public class ObjectSet<T> where T : Object
	{
		public T DefaultObject;
		private int m_NextIndex;
		private Dictionary<int, T> m_IndexedObjects = new();
		//private HashSet<ObjectIndexPair> m_ObjectIndexPairs;

		public T this[int index] => m_IndexedObjects.TryGetValue(index, out var existingObject) ? existingObject : DefaultObject;

		public int Count => m_IndexedObjects.Count;

		public ObjectSet()
			: this(null) {}

		public ObjectSet(T defaultObject) => DefaultObject = defaultObject;

		public bool Add(T item) => Add(item, out var _);

		public bool Add(T item, out int index)
		{
#if DEBUG
			if (item == null)
				throw new ArgumentNullException("item is null");
#endif

			if (Contains(item))
			{
				index = m_IndexedObjects.First(x => x.Value == item).Key;
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
			if (item != null)
			{
				foreach (var kvp in m_IndexedObjects)
				{
					if (kvp.Value.Equals(item))
					{
						m_IndexedObjects.Remove(kvp.Key);
						return true;
					}
				}
			}

			return false;
		}

		public bool RemoveAt(int index) => m_IndexedObjects.Remove(index);

		public void ReplaceAt(int index, T item)
		{
#if DEBUG
			if (index < 0)
				throw new IndexOutOfRangeException("index is negative");
			if (index >= m_NextIndex)
				throw new IndexOutOfRangeException($"index {index} is greater or equal than next index {m_NextIndex}");
			if (Contains(item))
				throw new ArgumentException("item already exists in set!");
#endif

			if (item == null)
				m_IndexedObjects.Remove(index);
			else
				m_IndexedObjects[index] = item;
		}

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
