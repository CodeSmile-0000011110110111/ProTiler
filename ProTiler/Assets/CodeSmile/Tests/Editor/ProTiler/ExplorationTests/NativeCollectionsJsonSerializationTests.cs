// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Serialization.Binary;
using Unity.Serialization.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Editor.ProTiler
{
	public class NativeCollectionsJsonSerializationTests
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

		[Test] public void CanSerializeAndDeserializeNativeListOfLinearTileDataStruct()
		{
			var linearData1 = new LinearTileData(123, 0);
			var linearData2 = new LinearTileData(0, (TileFlags)1);
			var list = new NativeList<LinearTileData>(1, Allocator.Temp);
			list.Add(linearData1);
			list.Add(linearData2);

			var adapters = new List<IJsonAdapter>
			{
				new JsonAdapters.NativeListAdapter<LinearTileData>(Allocator.Temp),
			};
			var json = Serialize.ToJson(list, adapters);
			Debug.Log($"Json with {json.Length} characters:\n{json}");
			var deserialList = Serialize.FromJson<NativeList<LinearTileData>>(json, adapters);

			Assert.That(deserialList.Length, Is.EqualTo(2));
			Assert.That(deserialList[0], Is.EqualTo(linearData1));
			Assert.That(deserialList[1], Is.EqualTo(linearData2));
		}
	}
}
