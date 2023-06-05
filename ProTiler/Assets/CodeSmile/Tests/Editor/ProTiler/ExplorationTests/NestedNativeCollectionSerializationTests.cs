// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Binary;
using UnityEngine;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int2;
using ChunkKey = System.Int64;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;
using ListCount = System.UInt16;

namespace CodeSmile.Tests.Editor.ProTiler
{
	public class NestedNativeCollectionSerializationTests
	{
		[Test] public void CanSerializeLinearTileDataStruct()
		{
			var linearData = new LinearTileData(1, TileFlags.DirectionWest);

			var bytes = BinarySerializer.Serialize(linearData);

			Assert.NotNull(bytes);
			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
			Assert.That(bytes.Length, Is.EqualTo(sizeof(UInt32)));
		}

		[Test] public void CanSerializeAndDeserializeLinearTileDataStruct()
		{
			var linearData = new LinearTileData(123, TileFlags.DirectionEast | TileFlags.FlipBoth);

			var bytes = BinarySerializer.Serialize(linearData);
			var deserializedData = BinarySerializer.Deserialize<LinearTileData>(bytes);

			Assert.That(deserializedData, Is.EqualTo(linearData));
		}

		[Test] public void CanSerializeAndDeserializeNativeListOfLinearTileDataStruct()
		{
			var linearData = new LinearTileData(2, TileFlags.DirectionSouth);
			var list = new NativeList<LinearTileData>(1, Allocator.Temp);
			list.Add(linearData);

			var adapters = new List<IBinaryAdapter> { new BinaryAdapters.NativeListMax65k<LinearTileData>() };
			var bytes = BinarySerializer.Serialize(list, adapters);
			var deserialList = BinarySerializer.Deserialize<NativeList<LinearTileData>>(bytes, adapters);

			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
			Assert.That(deserialList.Length, Is.EqualTo(1));
			Assert.That(deserialList[0], Is.EqualTo(linearData));
		}

		[Test] public void CanSerializeAndDeserializeUnsafeListOfLinearTileDataStruct()
		{
			var linearData = new LinearTileData(2, TileFlags.DirectionSouth);
			var list = new UnsafeList<LinearTileData>(1, Allocator.Temp);
			list.Add(linearData);

			var adapters = new List<IBinaryAdapter> { new BinaryAdapters.UnsafeListMax65k<LinearTileData>() };
			var bytes = BinarySerializer.Serialize(list, adapters);
			var deserialList = BinarySerializer.Deserialize<UnsafeList<LinearTileData>>(bytes, adapters);

			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
			Assert.That(deserialList.Length, Is.EqualTo(1));
			Assert.That(deserialList[0], Is.EqualTo(linearData));
		}

		[Test] public void CanSerializeAndDeserializeNativeListOfUnsafeListOfLinearTileDataStruct()
		{
			var linearData1 = new LinearTileData(2, TileFlags.DirectionSouth);
			var linearData2 = new LinearTileData(3, TileFlags.DirectionWest);
			var list = new NativeList<UnsafeList<LinearTileData>>(2, Allocator.Temp);
			var list0 = new UnsafeList<LinearTileData>(1, Allocator.Temp);
			var list1 = new UnsafeList<LinearTileData>(1, Allocator.Temp);
			list0.Add(linearData1);
			list1.Add(linearData2);
			list.Add(list0);
			list.Add(list1);

			Assert.That(list0[0], Is.EqualTo(linearData1));
			Assert.That(list1[0], Is.EqualTo(linearData2));

			var adapters = new List<IBinaryAdapter>
			{
				new BinaryAdapters.NativeListMax65k<UnsafeList<LinearTileData>>(),
				new BinaryAdapters.UnsafeListMax65k<LinearTileData>(),
			};
			var bytes = BinarySerializer.Serialize(list, adapters);
			var deserialList = BinarySerializer.Deserialize<NativeList<UnsafeList<LinearTileData>>>(bytes, adapters);

			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");
			Assert.That(deserialList.Length, Is.EqualTo(2));
			var deserialList0 = deserialList[0];
			var deserialList1 = deserialList[1];
			Assert.That(deserialList0[0], Is.EqualTo(linearData1));
			Assert.That(deserialList1[0], Is.EqualTo(linearData2));
		}

		public static class BinaryAdapters
		{
			private static class Collection
			{
				public static Int32 GetListCount<TList, TListType>(TList list)
					where TList:unmanaged, INativeList<TListType> where TListType : unmanaged =>
					list.Length <= ListCount.MaxValue
						? list.Length
						: throw new ArgumentOutOfRangeException(
							$"List length {list.Length} exceeds {ListCount.MaxValue}");

				public static unsafe void SerializeCount<TList, TListType>(
					in BinarySerializationContext<TList> context, in TList list)
					where TList : unmanaged, INativeList<TListType> where TListType : unmanaged =>
					context.Writer->Add((ListCount)GetListCount<TList, TListType>(list));

				public static void SerializeValues<TList, TListType>(
					in BinarySerializationContext<TList> context, in TList list)
					where TList : unmanaged, INativeList<TListType> where TListType : unmanaged
				{
					var listCount = list.Length;
					for (var i = 0; i < listCount; i++)
						context.SerializeValue(list[i]);
				}

				public static unsafe Int32 DeserializeCount<TList>(in BinaryDeserializationContext<TList> context)
					where TList : unmanaged => context.Reader->ReadNext<ListCount>();

				public static void DeserializeValues<TList, TListType>(in BinaryDeserializationContext<TList> context,
					TList list, Int32 itemCount)
					where TList : unmanaged, INativeList<TListType> where TListType : unmanaged
				{
					for (var i = 0; i < itemCount; i++)
						list[i] = context.DeserializeValue<TListType>();
				}

				public static NativeList<T> CreateResizedNativeList<T>(Int32 itemCount, Allocator allocator)
					where T : unmanaged
				{
					var list = new NativeList<T>(itemCount, allocator);
					list.Resize(itemCount, NativeArrayOptions.UninitializedMemory);
					return list;
				}

				public static UnsafeList<T> CreateResizedUnsafeList<T>(Int32 itemCount, Allocator allocator)
					where T : unmanaged
				{
					var list = new UnsafeList<T>(itemCount, allocator);
					list.Resize(itemCount);
					return list;
				}
			}

			public class NativeListMax65k<T> : IBinaryAdapter<NativeList<T>> where T : unmanaged
			{
				private readonly Allocator m_Allocator;

				public NativeListMax65k(Allocator allocator = Allocator.Temp) => m_Allocator = allocator;

				public void Serialize(in BinarySerializationContext<NativeList<T>> context, NativeList<T> list)
				{
					Collection.SerializeCount<NativeList<T>, T>(context, list);
					Collection.SerializeValues<NativeList<T>, T>(context, list);
				}

				public NativeList<T> Deserialize(in BinaryDeserializationContext<NativeList<T>> context)
				{
					var itemCount = Collection.DeserializeCount(context);
					var list = Collection.CreateResizedNativeList<T>(itemCount, m_Allocator);
					Collection.DeserializeValues<NativeList<T>, T>(context, list, itemCount);
					return list;
				}
			}

			public class UnsafeListMax65k<T> : IBinaryAdapter<UnsafeList<T>> where T : unmanaged
			{
				private readonly Allocator m_Allocator;

				public UnsafeListMax65k(Allocator allocator = Allocator.Temp) => m_Allocator = allocator;

				public void Serialize(in BinarySerializationContext<UnsafeList<T>> context, UnsafeList<T> list)
				{
					Collection.SerializeCount<UnsafeList<T>, T>(context, list);
					Collection.SerializeValues<UnsafeList<T>, T>(context, list);
				}

				public UnsafeList<T> Deserialize(in BinaryDeserializationContext<UnsafeList<T>> context)
				{
					var itemCount = Collection.DeserializeCount(context);
					var list = Collection.CreateResizedUnsafeList<T>(itemCount, m_Allocator);
					Collection.DeserializeValues<UnsafeList<T>, T>(context, list, itemCount);
					return list;
				}
			}
		}
	}
}
