// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace CodeSmile.ProTiler.Model
{
	public struct SparseDataMapChunk<TData> : IEquatable<SparseDataMapChunk<TData>>, IDisposable where TData : unmanaged
	{
		private UnsafeParallelHashMap<int3, TData> m_Data;
		public UnsafeParallelHashMap<int3, TData> Data => m_Data;

		public void Dispose() => m_Data.Dispose();

		public Boolean Equals(SparseDataMapChunk<TData> other) => m_Data.Equals(other.m_Data);
		public override Boolean Equals(Object obj) => obj is SparseDataMapChunk<TData> other && Equals(other);
		public override Int32 GetHashCode() => m_Data.GetHashCode();
		public static Boolean operator ==(SparseDataMapChunk<TData> left, SparseDataMapChunk<TData> right) =>
			left.Equals(right);
		public static Boolean operator !=(SparseDataMapChunk<TData> left, SparseDataMapChunk<TData> right) =>
			!left.Equals(right);
	}
}
