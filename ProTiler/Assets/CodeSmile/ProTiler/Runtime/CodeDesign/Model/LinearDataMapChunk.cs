// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model._remove;
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
	public struct LinearDataMapChunk<TData> : IEquatable<LinearDataMapChunk<TData>>, IDisposable where TData : unmanaged
	{
		private ChunkSize m_Size;
		private UnsafeList<TData> m_Data;

		public TData this[Int32 index]
		{
			get => m_Data[index];
			set => m_Data[ResizeListIfNeeded(index)] = value;
		}
		public TData this[ChunkCoord localCoord]
		{
			get => this[ToIndex(localCoord, m_Size)];
			set => this[ToIndex(localCoord, m_Size)] = value;
		}

		public ChunkCoord Size => m_Size;
		public UnsafeList<TData>.ReadOnly Data => m_Data.AsReadOnly();

		public static Boolean operator ==(LinearDataMapChunk<TData> left, LinearDataMapChunk<TData> right) =>
			left.Equals(right);

		public static Boolean operator !=(LinearDataMapChunk<TData> left, LinearDataMapChunk<TData> right) =>
			!left.Equals(right);

		public LinearDataMapChunk(ChunkSize size)
			: this(size, new UnsafeList<TData>(0, Allocator.Domain)) {}

		public LinearDataMapChunk(ChunkSize size, UnsafeList<TData> data)
		{
			m_Size.x = math.max(0, size.x);
			m_Size.y = math.max(0, size.y);
			m_Size.z = math.max(0, size.z);
			m_Data = data;
			ExpandListSizeToHeightLayer(m_Size.y);
		}

		public void Dispose() => m_Data.Dispose();
		public override String ToString() => $"{nameof(LinearDataMapChunk<TData>)}({m_Size}, {m_Data.Length} length, {m_Data.Allocator.ToAllocator} allocator)";

		public unsafe Boolean Equals(LinearDataMapChunk<TData> other) =>
			m_Size.Equals(other.m_Size) && m_Data.Length == other.m_Data.Length;

		/// <summary>
		///     Gets the internal UnsafeList for direct read/write data operations. Replacing an existing data
		///     with another instance is the allowed "write" operation.
		///     CAUTION: you must not resize, remove or add items to the list!
		///     The list size is controlled by the chunk and it expects the list's size to be a multiple of
		///     ChunkSize Y number of "layers" multiplied by X*Z dimension of each layer. Deviating from this
		///     design is not allowed, but if you need this, you can make your own kind of MyMapChunk struct
		///     together with a MyMap and a GridBase subclass that manages a list of MyMap
		///     (see GridBase as a template).
		/// </summary>
		/// <returns></returns>
		public UnsafeList<TData> GetWritableData() => m_Data;

		public void Serialize(IBinaryWriter writer) {}

		public LinearDataMap<TData> Deserialize<TData>(IBinaryReader reader, Byte userDataVersion)
			where TData : unmanaged => new();

		public TData GetData(Int32 index) => this[index];
		public void SetData(Int32 index, TData data) => this[index] = data;
		public TData GetData(ChunkCoord localCoord) => this[localCoord];
		public void SetData(ChunkCoord localCoord, TData data) => this[localCoord] = data;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Int32 ToIndex(ChunkCoord localCoord, ChunkSize chunkSize) =>
			localCoord.y * chunkSize.z * chunkSize.x + localCoord.z * chunkSize.x + localCoord.x;

		private Int32 ResizeListIfNeeded(Int32 chunkIndex)
		{
			if (chunkIndex >= m_Data.Length)
			{
				var accessedHeightLayer = ToHeightLayer(chunkIndex);
				ExpandListSizeToHeightLayer(accessedHeightLayer);
			}

			return chunkIndex;
		}

		private Int32 ToHeightLayer(Int32 chunkIndex)
		{
			var layerSize = m_Size.x * m_Size.z;
			return (Int32)math.round((chunkIndex + layerSize) / layerSize);
		}

		private void ExpandListSizeToHeightLayer(Int32 layerIndex)
		{
			layerIndex = math.max(0, layerIndex);
			var layerSize = m_Size.x * m_Size.z;
			var requestedListSize = layerSize * layerIndex;

			m_Data.Resize(requestedListSize);
			m_Size.y = layerIndex;
		}

		public override Boolean Equals(Object obj) => obj is LinearDataMapChunk<TData> other && Equals(other);
		public override Int32 GetHashCode() => HashCode.Combine(m_Size, m_Data);
	}
}
