// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.v4.GridMap;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Unity.Mathematics;
using Unity.Serialization.Binary;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Model
{
	public abstract class GridBase
	{
		private readonly List<DataMapBase> m_LinearMaps = new();
		private readonly List<DataMapBase> m_SparseMaps = new();

		protected readonly List<IBinaryAdapter> m_SerializationAdapters = new();
		private int3 m_ChunkSize = new(2, 0, 2);
		public IReadOnlyList<IBinaryAdapter> SerializationAdapters => m_SerializationAdapters;

		private GridBase()
			: this(0) {}

		public GridBase(Byte gridVersion) => AddGridMapSerializationAdapter(gridVersion);

		public void AddLinearDataMap<T>(Byte dataVersion, IDataMapStream stream = null) where T : unmanaged
		{
			m_LinearMaps.Add(new LinearDataMap<T>(stream));
			m_SerializationAdapters.Add(new DataMapBaseBinaryAdapter<LinearDataMap<T>>(dataVersion));
		}

		public void AddSparseDataMap<T>(Byte dataVersion, IDataMapStream stream = null) where T : unmanaged
		{
			m_SparseMaps.Add(new SparseDataMap<T>(stream));
			m_SerializationAdapters.Add(new DataMapBaseBinaryAdapter<SparseDataMap<T>>(dataVersion));
		}

		// public void AddSerializationAdapter<T>(GridBaseBinaryAdapter<T> adapter) where T : GridBase, new() =>
		// 	m_SerializationAdapters.Add(adapter);

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

		[ExcludeFromCodeCoverage]
		public override String ToString() => $"{nameof(GridBase)}(ChunkSize: {m_ChunkSize})";

		private void AddGridMapSerializationAdapter(Byte gridVersion)
		{
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
			var type = typeof(GridBaseBinaryAdapter<>).MakeGenericType(GetType());
			var versionParam = new Object[] { gridVersion };
			var culture = CultureInfo.InvariantCulture;
			var adapter = Activator.CreateInstance(type, bindingFlags, null, versionParam, culture);

			m_SerializationAdapters.Add(adapter as IBinaryAdapter);
		}
	}
}
