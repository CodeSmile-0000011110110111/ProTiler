// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.CodeDesign.v4.GridMap;
using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using ChunkCoord = Unity.Mathematics.int3;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.ProTiler.CodeDesign.v4.DataMaps
{
	public struct LinearDataMapChunk<TData> : IDisposable where TData : unmanaged
	{
		private readonly ChunkSize m_Size;
		private UnsafeList<TData> m_Data;
		public UnsafeList<TData> Data => m_Data;

		public LinearDataMapChunk(ChunkSize size)
		{
			m_Size = size;
			m_Data = new UnsafeList<TData>(0, Allocator.Domain);
		}

		public void Serialize(IBinaryWriter writer) {}

		public LinearDataMap<TData> Deserialize<TData>(IBinaryReader reader, Byte userDataVersion)
			where TData : unmanaged => new();

		public void Dispose() => m_Data.Dispose();

		public void Add(ChunkCoord coord, TData data)
		{
			ResizeListToEncompassLayer(coord.y);

			m_Data[CalculateIndex(coord)] = data;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Int32 CalculateIndex(ChunkCoord coord) => m_Size.x * m_Size.z * coord.y + coord.z * m_Size.z + coord.x;

		private void ResizeListToEncompassLayer(Int32 height)
		{
			var requestedListSize = m_Size.x * m_Size.z * (height + 1);
			if (m_Data.Length < requestedListSize)
				m_Data.Resize(requestedListSize);
		}

		public TData this[ChunkCoord chunkCoord] => m_Data[CalculateIndex(chunkCoord)];
	}
}
