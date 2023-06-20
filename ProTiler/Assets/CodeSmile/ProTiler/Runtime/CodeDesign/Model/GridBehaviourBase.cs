﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.v4.GridMap;
using UnityEngine;

namespace CodeSmile.ProTiler.CodeDesign.Model
{
	[ExecuteAlways]
	public abstract class GridBehaviourBase<T> : MonoBehaviour//, ISerializationCallbackReceiver
		where T : GridBase, new()
	{
		[SerializeReference] private T m_GridMap = new();
		[SerializeReference] protected GridMapBinarySerialization m_GridMapBinarySerialization = new();

		public T GridMap => m_GridMap;

		// public void OnBeforeSerialize() => SerializeMap();
		// public void OnAfterDeserialize() => DeserializeMap();
		// public void SerializeMap() => m_GridMapBinarySerialization.Serialize(m_GridMap, m_GridMap.SerializationAdapters);
		// public void DeserializeMap() => m_GridMap = m_GridMapBinarySerialization.Deserialize<T>(m_GridMap.SerializationAdapters);
	}
}
