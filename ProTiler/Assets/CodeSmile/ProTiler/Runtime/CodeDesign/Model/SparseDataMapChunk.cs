// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Model
{
	public struct SparseDataMapChunk<TData> where TData : unmanaged
	{
		private UnsafeParallelHashMap<int3, TData> m_Data;
		public UnsafeParallelHashMap<int3, TData> Data => m_Data;
	}
}
