// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Binary;

namespace CodeSmile.Core.Runtime.Serialization.BinaryAdapters
{
	public class UnsafeParallelHashMapBinaryAdapter<TKey, TValue> : IBinaryAdapter<UnsafeParallelHashMap<TKey, TValue>>
		where TKey : unmanaged, IEquatable<TKey> where TValue : unmanaged
	{
		private readonly Allocator m_Allocator;

		public UnsafeParallelHashMapBinaryAdapter(Allocator allocator) => m_Allocator = allocator;

		public unsafe void Serialize(in BinarySerializationContext<UnsafeParallelHashMap<TKey, TValue>> context,
			UnsafeParallelHashMap<TKey, TValue> map)
		{
			context.Writer->Add(map.Count());

			foreach (var pair in map)
			{
				context.SerializeValue(pair.Key);
				context.SerializeValue(pair.Value);
			}
		}

		public unsafe UnsafeParallelHashMap<TKey, TValue> Deserialize(
			in BinaryDeserializationContext<UnsafeParallelHashMap<TKey, TValue>> context)
		{
			var itemCount = context.Reader->ReadNext<Int32>();

			var map = new UnsafeParallelHashMap<TKey, TValue>(itemCount, m_Allocator);
			for (var i = 0; i < itemCount; i++)
			{
				var key = context.DeserializeValue<TKey>();
				var value = context.DeserializeValue<TValue>();
				map[key] = value;
			}

			return map;
		}
	}
}
