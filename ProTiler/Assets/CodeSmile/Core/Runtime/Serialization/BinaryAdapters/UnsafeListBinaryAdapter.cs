// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Core.Runtime.Extensions.NativeCollections;
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Binary;

namespace CodeSmile.Core.Serialization.BinaryAdapters
{
	public class UnsafeListBinaryAdapter<T> : IBinaryAdapter<UnsafeList<T>> where T : unmanaged
	{
		private readonly Allocator m_Allocator;

		public UnsafeListBinaryAdapter(Allocator allocator) => m_Allocator = allocator;

		public unsafe void Serialize(in BinarySerializationContext<UnsafeList<T>> context, UnsafeList<T> list)
		{
			var itemCount = list.Length;
			context.Writer->Add(itemCount);

			for (var i = 0; i < itemCount; i++)
				context.SerializeValue(list[i]);
		}

		public unsafe UnsafeList<T> Deserialize(in BinaryDeserializationContext<UnsafeList<T>> context)
		{
			var itemCount = context.Reader->ReadNext<Int32>();

			var list = UnsafeListExt.NewWithLength<T>(itemCount, m_Allocator);
			for (var i = 0; i < itemCount; i++)
				list[i] = context.DeserializeValue<T>();

			return list;
		}
	}
}
