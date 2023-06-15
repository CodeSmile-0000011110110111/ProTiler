// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using NUnit.Framework;
using Unity.Mathematics;

namespace CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization
{
	public class DataMapSerializationTests
	{
		public struct TestData
		{
			public byte byteValue;
			public int3 int3Value;
			public float4 float4Value;
		}

		[Test] public void LinearDataMap_SerializeAndDeserialize_AreEqual()
		{
			var map = new LinearDataMap<TestData>();



		}
	}
}
