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
	public struct LinearDataMapChunk<TData> : IDisposable where TData : unmanaged
	{
		private readonly ChunkSize m_Size;
		private UnsafeList<TData> m_Data;
		public UnsafeList<TData> Data { get => m_Data; set => m_Data = value; }

		public LinearDataMapChunk(ChunkSize size)
		{
			m_Size = size;
			m_Data = new UnsafeList<TData>(0, Allocator.Domain);
		}

		public void Serialize(IBinaryWriter writer) {}

		public LinearDataMap<TData> Deserialize<TData>(IBinaryReader reader, Byte userDataVersion)
			where TData : unmanaged => new();

		public void Dispose() => m_Data.Dispose();

		public TData this[ChunkCoord localCoord]
		{
			get => GetData(localCoord);
			set => SetData(localCoord, value);
		}
		private TData GetData(ChunkCoord localCoord) => m_Data[CalculateIndex(localCoord)];

		public void SetData(ChunkCoord localCoord, TData data) =>
			m_Data[ResizeListIfNeeded(CalculateIndex(localCoord))] = data;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Int32 CalculateIndex(ChunkCoord localCoord) =>
			m_Size.x * m_Size.z * localCoord.y + localCoord.z * m_Size.z + localCoord.x;

		private Int32 ResizeListIfNeeded(Int32 index)
		{
			if (index >= m_Data.Length)
			{
				var layerSize = m_Size.x * m_Size.z;
				var accessedLayerIndex = (Int32)math.round((index + layerSize) / layerSize);
				var requestedListSize = layerSize * accessedLayerIndex;
				m_Data.Resize(requestedListSize);
			}

			return index;
		}
	}
}
