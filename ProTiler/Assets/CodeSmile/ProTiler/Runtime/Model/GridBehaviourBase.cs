// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Serialization.Binary;
using UnityEngine;

namespace CodeSmile.ProTiler.CodeDesign.Model
{
	[ExecuteAlways]
	public abstract class GridBehaviourBase<T> : MonoBehaviour //, ISerializationCallbackReceiver
		where T : GridBase, new()
	{
		[SerializeReference] private T m_GridMap = new();
		//[SerializeReference] protected GridMapBinarySerialization m_GridMapBinarySerialization = new();

		public T GridMap => m_GridMap;

		// public void OnBeforeSerialize() => SerializeMap();
		// public void OnAfterDeserialize() => DeserializeMap();
		// public void SerializeMap() => m_GridMapBinarySerialization.Serialize(m_GridMap, m_GridMap.SerializationAdapters);
		// public void DeserializeMap() => m_GridMap = m_GridMapBinarySerialization.Deserialize<T>(m_GridMap.SerializationAdapters);
	}

	// Consider: make abstract base?
	[Serializable]
	public class GridMapBinarySerialization
	{
		[SerializeField] protected Byte[] m_SerializedGridMap;
		[SerializeField] private SerializedChunkWrapper[] m_SerializedChunks;

		public IReadOnlyList<IBinaryAdapter> GetDefaultAdapters()
		{
			var adapters = new List<IBinaryAdapter>();
			return adapters.AsReadOnly();
		}

		public void Serialize<T>(T gridMap, IReadOnlyList<IBinaryAdapter> adapters) where T : GridBase =>
			m_SerializedGridMap = CodeSmile.Serialization.Serialize.ToBinary(gridMap, adapters);

		public T Deserialize<T>(IReadOnlyList<IBinaryAdapter> adapters) where T : GridBase
		{
			if (m_SerializedGridMap == null || m_SerializedGridMap.Length == 0)
				return null;

			return CodeSmile.Serialization.Serialize.FromBinary<T>(m_SerializedGridMap, adapters);
		}

		[Serializable]
		private struct SerializedChunkWrapper
		{
			[SerializeField] private Byte[] m_SerializedChunk;
			public Byte[] SerializedChunk
			{
				get => m_SerializedChunk;
				set => m_SerializedChunk = value;
			}
		}
	}
}
