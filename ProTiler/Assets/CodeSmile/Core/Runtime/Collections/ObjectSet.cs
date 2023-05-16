// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.Extensions;
using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Collections
{
	/// <summary>
	///     Contains unique UnityEngine.Object instances so they can be accessed by the same index in both editor and runtime.
	///     It does *not* use GetInstanceID for indexes because the InstanceID differs between Editor and PlayMode.
	///     Unorthodox Behaviour:
	///     Index out of bounds access returns DefaultObject rather than throwing exceptions.
	///     Will not store null references (throws exception).
	/// </summary>
	[Serializable]
	[FullCovered]
	public class ObjectSet<T> where T : Object
	{
		/// <summary>
		///     The default object that is returned for non-existing indexes. Defaults to null.
		/// </summary>
		[SerializeField] [ReadOnlyField] private int m_NextIndex;
		[SerializeField] [ReadOnlyField] private T m_DefaultObject;
		[SerializeField] [HideInInspector] private IndexedObjectsDictionary m_IndexedObjects = new();

		/// <summary>
		///     Get or set object at index.
		///     Getter calls TryGetObject.
		///     Setter calls ReplaceAt.
		/// </summary>
		/// <param name="index"></param>
		public T this[int index] { get => TryGetObject(index); set => AssignAt(index, value); }

		/// <summary>
		///     Default object is returned when trying to get an object with an index that is out of bounds.
		/// </summary>
		public T DefaultObject { get => m_DefaultObject; set => m_DefaultObject = value; }

		/// <summary>
		///     How many unique objects are in the set.
		/// </summary>
		public int Count => m_IndexedObjects.Count;

		/// <summary>
		///     Creates ObjectSet.
		/// </summary>
		/// <param name="defaultObject"></param>
		public ObjectSet()
			: this(null, 0) {}

		/// <summary>
		///     Creates ObjectSet with specified DefaultObject.
		/// </summary>
		/// <param name="defaultObject"></param>
		public ObjectSet(T defaultObject)
			: this(defaultObject, 0) {}

		/// <summary>
		///     Creates ObjectSet with specified DefaultObject and start index.
		/// </summary>
		/// <param name="defaultObject"></param>
		/// <param name="startIndex"></param>
		public ObjectSet(T defaultObject, int startIndex)
		{
			m_DefaultObject = defaultObject;
			m_NextIndex = startIndex;
		}

		/// <summary>
		///     Adds an object to the set. Does nothing if the object already exists.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool Add(T obj) => Add(obj, out var _);

		/// <summary>
		///     Adds an object to the set and returns its index. If the object already exists in the set, it returns the object's
		///     index but does not add it again.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">thrown when object is null</exception>
		public bool Add(T obj, out int index)
		{
#if DEBUG
			if (obj == null)
				throw new ArgumentNullException("item is null");
#endif

			if (Contains(obj))
			{
				index = m_IndexedObjects.First(x => x.Value == obj).Key;
				return false;
			}

			m_IndexedObjects.Add(m_NextIndex, obj);
			index = m_NextIndex;
			m_NextIndex++;
			return true;
		}

		/// <summary>
		///     Removes all objects from the set.
		/// </summary>
		public void Clear() => m_IndexedObjects.Clear();

		/// <summary>
		///     Tests if there is an object for the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns>True if there is an object reference for the index in the set.</returns>
		public bool Contains(int index) => m_IndexedObjects.ContainsKey(index);

		/// <summary>
		///     Tests if the object exists in the set.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>True if the object is contained in the set. False if not or if obj is null.</returns>
		public bool Contains(T obj) => m_IndexedObjects.ContainsValue(obj);

		/// <summary>
		///     Removes the object from the set. Does nothing if obj is null.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>True if the object existed and was removed, otherwise false. Also returns false when obj is null.</returns>
		public bool Remove(T obj)
		{
			if (obj != null)
			{
				foreach (var kvp in m_IndexedObjects)
				{
					if (kvp.Value.Equals(obj))
					{
						m_IndexedObjects.Remove(kvp.Key);
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		///     Removes an object at the index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns>True if an object with the index existed and was removed, otherwise false.</returns>
		public bool RemoveAt(int index) => m_IndexedObjects.Remove(index);

		/// <summary>
		///     Tries to get an object for an index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns>The object at the index, or DefaultObject if there is no object for the given index.</returns>
		private T TryGetObject(int index)
		{
			var obj = m_IndexedObjects.TryGetValue(index, out var existingObject) ? existingObject : m_DefaultObject;
			return obj == null || obj.IsMissing() ? m_DefaultObject : obj;
		}

		/// <summary>
		///     Tries to replace an object at the index.
		///     Note: If obj is null it will remove the object at the index since the set doesn't store null.
		/// </summary>
		/// <param name="index">0-based index</param>
		/// <param name="obj">the object to set</param>
		/// <exception cref="System.IndexOutOfRangeException">Thrown when index is < 0 or>= next index.</exception>
		/// <exception cref="System.ArgumentException">Thrown when obj already exists in set</exception>
		private void AssignAt(int index, T obj)
		{
#if DEBUG
			if (index < 0)
				throw new ArgumentException("index must not be negative");
			if (index > m_NextIndex)
				throw new ArgumentException($"index {index} is greater or equal than next index: {m_NextIndex}");
			if (Contains(obj))
				throw new ArgumentException("object already exists in set!");
#endif

			if (index == m_NextIndex)
				Add(obj);
			else if (obj == null)
				RemoveAt(index);
			else
				m_IndexedObjects[index] = obj;
		}

		[Serializable] private class IndexedObjectsDictionary : SerializedDictionary<int, T> {}
	}
}
