// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Core.Runtime.Extensions.NativeCollections;
using System;
using Unity.Collections;
using Unity.Serialization.Binary;

namespace CodeSmile.Core.Serialization.BinaryAdapters
{
	public class NativeListBinaryAdapter<T> : IBinaryAdapter<NativeList<T>> where T : unmanaged
	{
		private readonly Allocator m_Allocator;

		public NativeListBinaryAdapter(Allocator allocator) => m_Allocator = allocator;

		public unsafe void Serialize(in BinarySerializationContext<NativeList<T>> context, NativeList<T> list)
		{
			var itemCount = list.Length;
			context.Writer->Add(itemCount);

			for (var i = 0; i < itemCount; i++)
				context.SerializeValue(list[i]);
		}

		public unsafe NativeList<T> Deserialize(in BinaryDeserializationContext<NativeList<T>> context)
		{
			var itemCount = context.Reader->ReadNext<Int32>();

			var list = NativeListExt.NewWithLength<T>(itemCount, m_Allocator, NativeArrayOptions.UninitializedMemory);
			for (var i = 0; i < itemCount; i++)
				list[i] = context.DeserializeValue<T>();

			return list;
		}
	}
}
