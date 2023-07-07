// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Serialization;
using CodeSmile.Serialization;
using CodeSmile.Serialization.BinaryAdapters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Unity.Collections;
using Unity.Serialization.Binary;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using LocalCoord = Unity.Mathematics.int3;
using LocalPos = Unity.Mathematics.float3;
using WorldCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.ProTiler.Model
{
	public abstract class GridMapBase : IDisposable
	{
		protected readonly List<DataMapBase> m_LinearMaps = new();
		protected readonly List<DataMapBase> m_SparseMaps = new();
		protected ChunkSize m_ChunkSize = DataMapBase.s_MinimumChunkSize;

		public ChunkSize ChunkSize { get => m_ChunkSize; internal set => m_ChunkSize = value; }
		public IReadOnlyList<DataMapBase> LinearMaps => m_LinearMaps.AsReadOnly();
		public IReadOnlyList<DataMapBase> SparseMaps => m_SparseMaps.AsReadOnly();

		//protected readonly List<IBinaryAdapter> m_SerializationAdapters = new();
		//public IReadOnlyList<IBinaryAdapter> SerializationAdapters => m_SerializationAdapters;

		// making parameterless ctor private forces subclasses to implement a parameterized ctor
		private GridMapBase()
			: this(DataMapBase.s_MinimumChunkSize, 0) {}

		public GridMapBase(ChunkSize chunkSize, Byte gridVersion) => m_ChunkSize = chunkSize;

		public void Dispose()
		{
			DisposeDataMaps(m_LinearMaps);
			DisposeDataMaps(m_SparseMaps);
		}

		internal List<DataMapBase> GetLinearMaps() => m_LinearMaps;
		internal void SetLinearMaps(IReadOnlyList<DataMapBase> linearMaps)
		{
			DisposeDataMaps(m_LinearMaps);
			m_LinearMaps.AddRange(linearMaps);
		}

		internal List<DataMapBase> GetSparseMaps() => m_SparseMaps;
		internal void SetSparseMaps(IReadOnlyList<DataMapBase> sparseMaps)
		{
			DisposeDataMaps(m_SparseMaps);
			m_SparseMaps.AddRange(sparseMaps);
		}

		//AddGridMapSerializationAdapter(gridVersion);
		public void AddLinearDataMap<TData>(Byte dataVersion, IDataMapStream stream = null)
			where TData : unmanaged, IBinarySerializable =>
			m_LinearMaps.Add(new LinearDataMap<TData>(m_ChunkSize /*, stream*/));

		//m_SerializationAdapters.Add(new LinearDataMapBinaryAdapter<TData>(0));
		public void AddSparseDataMap<TData>(Byte dataVersion, IDataMapStream stream = null)
			where TData : unmanaged, IBinarySerializable =>
			m_SparseMaps.Add(new SparseDataMap<TData>(m_ChunkSize /*, stream*/));
		//m_SerializationAdapters.Add(new SparseDataMapBinaryAdapter<TData>(0, dataVersion));
		// public void AddSerializationAdapter<T>(GridBaseBinaryAdapter<T> adapter) where T : GridBase, new() =>
		// 	m_SerializationAdapters.Add(adapter);

		public virtual void Serialize<TGridMap>(in BinarySerializationContext<TGridMap> context)
			where TGridMap : GridMapBase, new()
		{
			// context.SerializeValue(m_ChunkSize);
			// SerializeMaps(context, m_LinearMaps);
			// SerializeMaps(context, m_SparseMaps);
		}

		/*
		private unsafe void SerializeMaps<TGridMap>(BinarySerializationContext<TGridMap> context,
			List<DataMapBase> list)
			where TGridMap : GridMapBase, new()
		{
			var itemCount = list.Count;
			context.Writer->Add(itemCount);
			for (var i = 0; i < itemCount; i++)
				context.SerializeValue(list[i]);
		}
		*/

		public virtual GridMapBase Deserialize<TGridMap>(in BinaryDeserializationContext<TGridMap> context,
			Byte version)
			where TGridMap : GridMapBase, new() =>
			// m_ChunkSize = context.DeserializeValue<ChunkSize>();
			this;

		[ExcludeFromCodeCoverage]
		public override String ToString() => $"{nameof(GridMapBase)}(ChunkSize: {m_ChunkSize})";

		private void AddGridMapSerializationAdapter(Byte gridVersion)
		{
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
			var type = typeof(GridMapBinaryAdapter<>).MakeGenericType(GetType());
			var versionParam = new Object[] { gridVersion };
			var culture = CultureInfo.InvariantCulture;
			var adapter = Activator.CreateInstance(type, bindingFlags, null, versionParam, culture);

			//m_SerializationAdapters.Add(adapter as IBinaryAdapter);
		}

		public static List<IBinaryAdapter> GetBinaryAdapters(Byte dataAdapterVersion)
		{
			throw new NotImplementedException();
			/*
			var adapters = LinearDataMapChunk<TData>.GetBinaryAdapters(dataAdapterVersion);
			adapters.Add(new NativeParallelHashMapBinaryAdapter<ChunkKey, LinearDataMapChunk<TData>>(Allocator.Domain));
			adapters.Add(new LinearDataMapBinaryAdapter<TData>(MapAdapterVersion));
			return adapters;
		*/
		}

		private void DisposeDataMaps(List<DataMapBase> dataMaps)
		{
			foreach (var dataMap in dataMaps)
				dataMap.Dispose();
			dataMaps.Clear();
		}
	}
}
