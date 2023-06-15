// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Properties;
using Unity.Serialization.Json;
using UnityEngine;

namespace CodeSmile.Tests.Editor.ProTiler.ExplorationTests
{
	public class JsonSerializationTests
	{
		[Test] public void CanSerializeLinearTileDataStruct()
		{
			var linearData = new LinearTileData(123, (TileFlags)1);

			var json = Serialize.ToJson(linearData);
			Debug.Log($"Json with {json.Length} characters:\n{json}");

			Assert.NotNull(json);
			Assert.That(json.Length, Is.EqualTo(36));
		}

		[Test] public void CanSerializeAndDeserializeLinearTileDataStruct()
		{
			var linearData = new LinearTileData(1, TileFlags.DirectionEast | TileFlags.FlipVertical);

			var json = Serialize.ToJson(linearData);
			Debug.Log($"Json with {json.Length} characters:\n{json}");
			var deserializedData = Serialize.FromJson<LinearTileData>(json);

			Assert.NotNull(json);
			Assert.That(deserializedData, Is.EqualTo(linearData));
		}

		[Test] public void CanSerializeAndDeserializeStructWithNativeList()
		{
			var structWithList = new StructWithNativeList(3);
			structWithList.m_NativeList.Add(1);
			structWithList.m_NativeList.Add(2);
			structWithList.m_NativeList.Add(3);

			var allocator = Allocator.Temp;
			var adapters = new List<IJsonAdapter>
			{
				new StructWithNativeList.JsonAdapter(),
				new JsonAdapters.NativeListAdapter<Int32>(allocator),
				new JsonAdapters.NativeListAdapter<Int64>(allocator),
			};
			var json = Serialize.ToJson(structWithList, adapters);
			Debug.Log($"Json with {json.Length} characters:\n{json}");
			var deserialList = Serialize.FromJson<StructWithNativeList>(json, adapters);

			Assert.That(deserialList.m_NativeList.Length, Is.EqualTo(3));
			Assert.That(deserialList.m_NativeList[0], Is.EqualTo(1));
			Assert.That(deserialList.m_NativeList[1], Is.EqualTo(2));
			Assert.That(deserialList.m_NativeList[2], Is.EqualTo(3));
		}

		[Test] public void CanSerializeAndDeserializeNativeListOfLinearTileDataStruct()
		{
			var allocator = Allocator.Temp;
			var linearData1 = new LinearTileData(123, 0);
			var linearData2 = new LinearTileData(0, (TileFlags)1);
			var list = new NativeList<LinearTileData>(1, allocator);
			list.Add(linearData1);
			list.Add(linearData2);

			var adapters = new List<IJsonAdapter>
			{
				new JsonAdapters.NativeListAdapter<LinearTileData>(allocator),
			};
			var json = Serialize.ToJson(list, adapters);
			Debug.Log($"Json with {json.Length} characters:\n{json}");
			var deserialList = Serialize.FromJson<NativeList<LinearTileData>>(json, adapters);

			Assert.That(deserialList.Length, Is.EqualTo(2));
			Assert.That(deserialList[0], Is.EqualTo(linearData1));
			Assert.That(deserialList[1], Is.EqualTo(linearData2));
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct StructWithNativeList
		{
			[CreateProperty] public NativeList<Int64> m_NativeList;

			public StructWithNativeList(Int32 count) => m_NativeList = new NativeList<Int64>(count, Allocator.Temp);

			public class JsonAdapter : IJsonAdapter<StructWithNativeList>
			{
				public void Serialize(in JsonSerializationContext<StructWithNativeList> context,
					StructWithNativeList value)
				{
					var writer = context.Writer;
					using (writer.WriteObjectScope())
						context.SerializeValue(value.m_NativeList);
				}

				public StructWithNativeList Deserialize(in JsonDeserializationContext<StructWithNativeList> context)
				{
					var structWithList = new StructWithNativeList();
					var objectView = context.SerializedValue.AsObjectView();
					structWithList.m_NativeList = context.DeserializeValue<NativeList<Int64>>(objectView);
					return structWithList;
				}
			}
		}
	}
}
