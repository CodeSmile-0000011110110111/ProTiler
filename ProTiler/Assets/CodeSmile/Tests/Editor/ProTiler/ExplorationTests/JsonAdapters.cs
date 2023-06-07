// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization;
using Unity.Serialization.Binary;
using Unity.Serialization.Json;
using UnityEngine;

namespace CodeSmile.Tests.Editor.ProTiler
{
	public static class JsonAdapters
	{
		public class NativeListAdapter<T> : IJsonAdapter<NativeList<T>> where T : unmanaged
		{
			private readonly Allocator m_Allocator;

			public NativeListAdapter(Allocator allocator) => m_Allocator = allocator;

			public void Serialize(in JsonSerializationContext<NativeList<T>> context, NativeList<T> list)
			{
				var writer = context.Writer;
				var itemCount = list.Length;

				using (writer.WriteObjectScope())
				{
					writer.WriteKey(nameof(Type));
					writer.WriteValue(typeof(NativeList<T>).Name);
					writer.WriteKey(nameof(NativeList<T>.Length));
					writer.WriteValue(itemCount);

					writer.WriteKey(typeof(T).Name);
					using (writer.WriteArrayScope())
					{
						for (var i = 0; i < itemCount; i++)
							context.SerializeValue(list[i]);
					}
				}
			}

			public NativeList<T> Deserialize(in JsonDeserializationContext<NativeList<T>> context)
			{
				var typeName = context.SerializedValue[nameof(Type)].AsStringView().ToString();
				if (typeName != typeof(NativeList<T>).Name)
					throw new SerializationException(
						$"expected type '{typeof(NativeList<T>).Name}' but read '{typeName}'");

				var itemCount = context.SerializedValue[nameof(NativeList<T>.Length)].AsInt32();
				var list = CreateResizedNativeList(itemCount, m_Allocator);

				var array = context.SerializedValue[typeof(T).Name].AsArrayView();
				Debug.Log(array.ToString());

				var enumerator = array.GetEnumerator();
				Debug.Log(enumerator.Current);
				enumerator.MoveNext();
				Debug.Log(enumerator.Current);


				//for (var i = 0; i < itemCount; i++) list[i] = array.
				return list;
			}

			private NativeList<T> CreateResizedNativeList(Int32 itemCount, Allocator allocator)
			{
				var list = new NativeList<T>(itemCount, allocator);
				list.Resize(itemCount, NativeArrayOptions.UninitializedMemory);
				return list;
			}
		}

		public class UnsafeListAdapter<T> : IBinaryAdapter<UnsafeList<T>> where T : unmanaged
		{
			private readonly Allocator m_Allocator;

			public UnsafeListAdapter(Allocator allocator) => m_Allocator = allocator;

			public unsafe void Serialize(in BinarySerializationContext<UnsafeList<T>> context, UnsafeList<T> list)
			{
				context.Writer->Add(list.Length);
				var itemCount = list.Length;
				for (var i = 0; i < itemCount; i++)
					context.SerializeValue(list[i]);
			}

			public unsafe UnsafeList<T> Deserialize(in BinaryDeserializationContext<UnsafeList<T>> context)
			{
				var itemCount = context.Reader->ReadNext<Int32>();
				var list = CreateResizedUnsafeList(itemCount, m_Allocator);
				for (var i = 0; i < itemCount; i++)
					list[i] = context.DeserializeValue<T>();
				return list;
			}

			private UnsafeList<T> CreateResizedUnsafeList(Int32 itemCount, Allocator allocator)
			{
				var list = new UnsafeList<T>(itemCount, allocator);
				list.Resize(itemCount);
				return list;
			}
		}

		public class NativeParallelHashMapAdapter<TKey, TValue> : IBinaryAdapter<NativeParallelHashMap<TKey, TValue>>
			where TKey : unmanaged, IEquatable<TKey> where TValue : unmanaged
		{
			private readonly Allocator m_Allocator;

			public NativeParallelHashMapAdapter(Allocator allocator) => m_Allocator = allocator;

			public unsafe void Serialize(in BinarySerializationContext<NativeParallelHashMap<TKey, TValue>> context,
				NativeParallelHashMap<TKey, TValue> map)
			{
				context.Writer->Add(map.Count());
				foreach (var pair in map)
				{
					context.SerializeValue(pair.Key);
					context.SerializeValue(pair.Value);
				}
			}

			public unsafe NativeParallelHashMap<TKey, TValue> Deserialize(
				in BinaryDeserializationContext<NativeParallelHashMap<TKey, TValue>> context)
			{
				var itemCount = context.Reader->ReadNext<Int32>();
				var map = new NativeParallelHashMap<TKey, TValue>(itemCount, m_Allocator);
				for (var i = 0; i < itemCount; i++)
				{
					var key = context.DeserializeValue<TKey>();
					var value = context.DeserializeValue<TValue>();
					map[key] = value;
				}
				return map;
			}
		}

		public class UnsafeParallelHashMapAdapter<TKey, TValue> : IBinaryAdapter<UnsafeParallelHashMap<TKey, TValue>>
			where TKey : unmanaged, IEquatable<TKey> where TValue : unmanaged
		{
			private readonly Allocator m_Allocator;

			public UnsafeParallelHashMapAdapter(Allocator allocator) => m_Allocator = allocator;

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
}
