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
	///     Stores grid data in a linear list. Data is any unmanaged type.
	///     Allocates memory when needed, incrementing by one "layer" where layer size is the chunk size X*Z dimensions.
	///     If initialized with a ChunkSize, chunkSize.y determines the number of height layers to pre-allocate.
	///     Use y==0 to defer allocation until assignment.
	///     Note that out of bounds exceptions may be thrown when reading the chunk, reading does not resize the list.
	///     Size property contains the zero based chunk size and is updated as the chunk allocates more height layers,
	///     where Size.y reflects the current "height" or number of allocated layers.
	///     For example, ChunkSize(2,2,2) means the allowed coordinate range goes from (0,0,0) to (1,1,1)
	///     or 2 layers.
	/// </summary>
	/// <typeparam name="TData"></typeparam>
	public struct LinearDataMapChunk<TData> : IEquatable<LinearDataMapChunk<TData>>, IDisposable where TData : unmanaged
	{
		private ChunkSize m_Size;
		private UnsafeList<TData> m_Data;

		/// <summary>
		///     Get/set TData by index. Setting data may resize the list.
		/// </summary>
		/// <param name="index"></param>
		public TData this[Int32 index]
		{
			get => m_Data[index];
			set => m_Data[ResizeListIfNeeded(index)] = value;
		}

		/// <summary>
		///     Get/set TData by local coord. Setting data to a coord.y whose layer is not allocated will resize the list
		///     to encompass that layer.
		/// </summary>
		/// <param name="localCoord"></param>
		public TData this[ChunkCoord localCoord]
		{
			get => this[ToIndex(localCoord, m_Size)];
			set => this[ToIndex(localCoord, m_Size)] = value;
		}

		/// <summary>
		///     The size of the chunk. Dimensions X*Z will never change. Y will increase when setting data to a layer
		///     that has not been allocated. If Size.y is zero the chunk has not allocated native memory yet.
		/// </summary>
		public ChunkCoord Size => m_Size;

		/// <summary>
		///     Readonly access to the internal UnsafeList for fast enumeration.
		/// </summary>
		public UnsafeList<TData>.ReadOnly Data => m_Data.AsReadOnly();

		public static Boolean operator ==(LinearDataMapChunk<TData> left, LinearDataMapChunk<TData> right) =>
			left.Equals(right);

		public static Boolean operator !=(LinearDataMapChunk<TData> left, LinearDataMapChunk<TData> right) =>
			!left.Equals(right);

		/// <summary>
		///     Creates a chunk with the given size. Negative size values are clamped to 0.
		/// </summary>
		/// <param name="size"></param>
		public LinearDataMapChunk(ChunkSize size)
			: this(size, new UnsafeList<TData>(0, Allocator.Domain)) {}

		/// <summary>
		///     Creates a chunk with size and an UnsafeList holding data. The list must be allocated.
		/// </summary>
		/// <param name="size"></param>
		/// <param name="data"></param>
		public LinearDataMapChunk(ChunkSize size, UnsafeList<TData> data)
		{
			if (data.IsCreated == false)
				throw new ArgumentException("UnsafeList<TData> passed into ctor is not allocated");

			m_Size = math.max(ChunkSize.zero, size);
			m_Data = data;
			ExpandListSizeToHeightLayer(m_Size.y);
		}

		/// <summary>
		///     Disposes internal native memory.
		/// </summary>
		public void Dispose() => m_Data.Dispose();

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

		/// <summary>
		///     Gets data by index. Use this within a using() statement where the chunk is immutable.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public TData GetData(Int32 index) => this[index];

		/// <summary>
		///     Sets data by index. Use this within a using() statement where the chunk is immutable.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="data"></param>
		public void SetData(Int32 index, TData data) => this[index] = data;

		/// <summary>
		///     Sets data by local coord. Use this within a using() statement where the chunk is immutable.
		/// </summary>
		/// <param name="localCoord"></param>
		public TData GetData(ChunkCoord localCoord) => this[localCoord];

		/// <summary>
		///     Sets data by local coord. Use this within a using() statement where the chunk is immutable.
		/// </summary>
		/// <param name="localCoord"></param>
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

		public unsafe Boolean Equals(LinearDataMapChunk<TData> other) =>
			m_Data.Ptr == other.m_Data.Ptr && m_Data.Length == other.m_Data.Length && m_Size.Equals(other.m_Size);

		public unsafe override String ToString() =>
			$"{nameof(LinearDataMapChunk<TData>)}({m_Size}, {m_Data.Length} length, {(IntPtr)m_Data.Ptr} ptr, {m_Data.Allocator.ToAllocator} allocator)";
	}
}
