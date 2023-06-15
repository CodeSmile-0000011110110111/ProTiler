// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Runtime.CodeDesign;
using CodeSmile.ProTiler.Runtime.CodeDesign.v4.GridMap;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Tests.Editor.ProTiler.IntegrationTests
{
	public class VoxelMapBehaviourTest : VoxelMapBehaviour
	{
		public GridMapBinarySerialization BinarySerialization => m_GridMapBinarySerialization;

		private void Awake() => m_GridMapBinarySerialization = new GridMapBinarySerializationTest();
	}

	public class GridMapBinarySerializationTest : GridMapBinarySerialization
	{
		public Byte[] SerializedGridMap => m_SerializedGridMap;
	}

	public class v4DesignSerializationTests
	{
		[Test] [CreateEmptyScene] [CreateGameObject("Test", typeof(VoxelMapBehaviourTest))]
		public void v4DesignSerializationTestsSimplePasses()
		{
			var voxelMapBehaviour = Object.FindAnyObjectByType<VoxelMapBehaviourTest>();
			var serialization = voxelMapBehaviour.BinarySerialization as GridMapBinarySerializationTest;

			voxelMapBehaviour.SerializeMap();
			Debug.Log(voxelMapBehaviour.GridMap);
			var bytes = serialization.SerializedGridMap;
			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");

			voxelMapBehaviour.DeserializeMap();
			Debug.Log(voxelMapBehaviour.GridMap);
			bytes = serialization.SerializedGridMap;
			Debug.Log($"{bytes.Length} Bytes: {bytes.AsString()}");

			Assert.NotNull(serialization.SerializedGridMap);
			Assert.That(serialization.SerializedGridMap.Length, Is.GreaterThan(0));
		}
	}
}
