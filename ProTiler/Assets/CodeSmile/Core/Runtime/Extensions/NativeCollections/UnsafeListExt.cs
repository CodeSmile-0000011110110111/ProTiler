// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace CodeSmile.Core.Extensions.NativeCollections
{
	public static class UnsafeListExt
	{
		public static UnsafeList<T> NewWithLength<T>(Int32 length, Allocator allocator) where T:unmanaged
		{
			var list = new UnsafeList<T>(length, allocator);
			list.Resize(length);
			return list;
		}
	}
}
