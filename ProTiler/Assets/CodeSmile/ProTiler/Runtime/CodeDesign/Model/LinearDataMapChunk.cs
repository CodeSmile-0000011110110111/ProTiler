// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using ChunkCoord = Unity.Mathematics.int3;
using ChunkSize = Unity.Mathematics.int3;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Model
{
	/// <summary>
	///     Stores data linearly. Data is any unmanaged type.
	///     Allocates memory when needed, incrementing by one "layer size" where layer size is the chunk size X*Z dimensions.
	///     If initialized with a ChunkSize, the y determines the number of height layers to pre-allocate. Use y==0 to
	///     defer allocation for assignment. Note that out of bounds exceptions are thrown when reading the chunk.
	///     Size property contains the zero based chunk size and is updated as the chunk allocates more height layers,
	///     where Size.y reflects the current "height" or number of layers.
	///     For example, ChunkSize(2,2,2) means the allowed coordinate range goes from (0,0,0) to (1,1,1).
	/// </summary>
	/// <typeparam name="TData"></typeparam>
	public struct LinearDataMapChunk<TData> : IDisposable where TData : unmanaged
	{
		private ChunkSize m_Size;
		public ChunkCoord Size => m_Size;
		private UnsafeList<TData> m_Data;
		internal UnsafeList<TData> Data
		{
			get => m_Data;
			set
			{
				if (m_Data.IsCreated)
					m_Data.Dispose();
				m_Data = value;
			}
		}

		public LinearDataMapChunk(ChunkSize size)
		{
			m_Size.x = math.max(0, size.x);
			m_Size.y = math.max(0, size.y);
			m_Size.z = math.max(0, size.z);

			m_Data = new UnsafeList<TData>(0, Allocator.Domain);
			ResizeListToHeightLayer(m_Size.y);
		}

		public void Serialize(IBinaryWriter writer) {}

		public LinearDataMap<TData> Deserialize<TData>(IBinaryReader reader, Byte userDataVersion)
			where TData : unmanaged => new();

		public void Dispose() => m_Data.Dispose();

		public TData this[Int32 index]
		{
			get => m_Data[index];
			set => m_Data[ResizeListIfNeeded(index)] = value;
		}
		public TData this[ChunkCoord localCoord]
		{
			get => this[ToIndex(localCoord)];
			set => this[ToIndex(localCoord)] = value;
		}
		public TData GetData(Int32 index) => this[index];
		public void SetData(Int32 index, TData data) => this[index] = data;
		public TData GetData(ChunkCoord localCoord) => this[localCoord];
		public void SetData(ChunkCoord localCoord, TData data) => this[localCoord] = data;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Int32 ToIndex(ChunkCoord localCoord) =>
			m_Size.x * m_Size.z * localCoord.y + localCoord.z * m_Size.z + localCoord.x;

		private Int32 ResizeListIfNeeded(Int32 chunkIndex)
		{
			if (chunkIndex >= m_Data.Length)
			{
				var accessedHeightLayer = ToHeightLayer(chunkIndex);
				ResizeListToHeightLayer(accessedHeightLayer);
			}

			return chunkIndex;
		}

		private Int32 ToHeightLayer(Int32 chunkIndex)
		{
			var layerSize = m_Size.x * m_Size.z;
			var accessedLayerIndex = (Int32)math.round((chunkIndex + layerSize) / layerSize);
			return accessedLayerIndex;
		}

		private void ResizeListToHeightLayer(Int32 layerIndex)
		{
			layerIndex = math.max(0, layerIndex);
			var layerSize = m_Size.x * m_Size.z;
			var requestedListSize = layerSize * layerIndex;

			m_Data.Resize(requestedListSize);
			m_Size.y = layerIndex;
		}
	}
}
