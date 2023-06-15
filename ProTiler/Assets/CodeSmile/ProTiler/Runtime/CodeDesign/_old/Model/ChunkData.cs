// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Properties;

namespace CodeSmile.ProTiler.Runtime.CodeDesign._old.Model
{
	public struct ChunkDataSparse<TData> : IChunkDataSparse<TData> where TData : unmanaged
	{
		[CreateProperty] public UnsafeParallelHashMap<int3, TData> sparseData;
	}

	public struct ChunkDataLinear<TData> : IChunkDataLinear<TData> where TData : unmanaged
	{
		[CreateProperty] public UnsafeList<TData> linearData;
	}
}
