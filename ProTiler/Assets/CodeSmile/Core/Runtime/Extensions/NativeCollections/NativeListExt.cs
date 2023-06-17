// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections;

namespace CodeSmile.Core.Extensions.NativeCollections
{
	public static class NativeListExt
	{
		public static NativeList<T> NewWithLength<T>(Int32 length, Allocator allocator, NativeArrayOptions options)
			where T : unmanaged
		{
			var list = new NativeList<T>(length, allocator);
			list.Resize(length, options);
			return list;
		}
	}
}
