// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.v4.GridMap;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Serialization.Binary;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Model
{
	public abstract class GridBase
	{
		private readonly List<DataMapBase> m_LinearMaps = new();
		private readonly List<DataMapBase> m_SparseMaps = new();

		protected readonly List<IBinaryAdapter> m_SerializationAdapters = new();
		private int3 m_ChunkSize = new(2, 2, 2);
		public IReadOnlyList<IBinaryAdapter> SerializationAdapters => m_SerializationAdapters;

		public void AddLinearDataMap<T>(byte userDataVersion, IDataMapStream stream = null) where T : unmanaged
		{
			m_LinearMaps.Add(new LinearDataMap<T>(stream));
			//m_SerializationAdapters.Add(new LinearDataMapChunkBinaryAdapter<T>());
			m_SerializationAdapters.Add(new DataMapBaseBinaryAdapter<LinearDataMap<T>>(userDataVersion));
		}

		public void AddSparseDataMap<T>(byte userDataVersion, IDataMapStream stream = null) where T : unmanaged
		{
			m_SparseMaps.Add(new SparseDataMap<T>(stream));
			//m_SerializationAdapters.Add(new SparseDataMapChunkBinaryAdapter<T>());
			m_SerializationAdapters.Add(new DataMapBaseBinaryAdapter<SparseDataMap<T>>(userDataVersion));
		}

		public virtual void Serialize<TGridMap>(in BinarySerializationContext<TGridMap> context)
			where TGridMap : GridBase, new()
		{
			context.SerializeValue(m_ChunkSize);
			SerializeMaps(context, m_LinearMaps);
			SerializeMaps(context, m_SparseMaps);
		}

		private unsafe void SerializeMaps<TGridMap>(BinarySerializationContext<TGridMap> context,
			List<DataMapBase> list)
			where TGridMap : GridBase, new()
		{
			var itemCount = list.Count;
			context.Writer->Add(itemCount);
			for (var i = 0; i < itemCount; i++)
				context.SerializeValue(list[i]);
		}

		// TODO: write maps
		public virtual GridBase Deserialize<TGridMap>(in BinaryDeserializationContext<TGridMap> context,
			Byte version)
			where TGridMap : GridBase, new()
		{
			m_ChunkSize = context.DeserializeValue<int3>();
			return this;
		}

		public override String ToString() => $"{nameof(GridBase)}(ChunkSize: {m_ChunkSize})";

		protected void AddGridMapSerializationAdapter<T>(Byte version) where T : GridBase, new() =>
			m_SerializationAdapters.Add(new GridBaseBinaryAdapter<T>(0));
	}
}
