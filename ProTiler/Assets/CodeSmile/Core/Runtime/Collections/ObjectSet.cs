// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace CodeSmile.ProTiler.Collections
{
	/// <summary>
	/// Contains UnityEngine.Object instances so they can be accessed by the same index in both editor and runtime,
	/// because GetInstanceID changes between Editor and Runtime.
	/// </summary>
	[Serializable]
	public class ObjectSet<T> where T : Object
	{
		private List<T> m_Objects;

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= m_Objects.Count)
					return null;

				return m_Objects[index];
			}
		}
		public int Count => m_Objects.Count;

		public ObjectSet() => m_Objects = new List<T>();

		public int AddOrGetExisting(T obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			var existingIndex = m_Objects.FindIndex(o => o.GetInstanceID() == obj.GetInstanceID());
			if (existingIndex >= 0)
				return existingIndex;

			m_Objects.Add(obj);
			return m_Objects.Count - 1;
		}
	}
}
