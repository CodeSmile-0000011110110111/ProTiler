// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.ProTiler.Data
{
	[Serializable]
	public sealed class TileDataDictionary : Dictionary<int3, TileData>, ISerializationCallbackReceiver
	{
		[SerializeField] private List<int3> m_Keys = new();
		[SerializeField] private List<TileData> m_Values = new();

		public void OnBeforeSerialize()
		{
			m_Keys.Clear();
			m_Values.Clear();

			foreach (var kvp in this)
			{
				m_Keys.Add(kvp.Key);
				m_Values.Add(kvp.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			Clear();
			for (var i = 0; i < m_Keys.Count; i++)
				Add(m_Keys[i], m_Values[i]);

			m_Keys.Clear();
			m_Values.Clear();
		}
	}
}