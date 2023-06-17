// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Core.Serialization;
using NUnit.Framework;

namespace CodeSmile.Tests.Runtime.Core.Serialization
{
	public class UnitySerializationTests
	{
		public static void UnitySerialization_SerializeAndDeserializeAllSimpleTypes_AreEqual_Impl()
		{
			var original = new SerializationTestAllSimpleTypes();

			var deserialized = Serialize.FromBinary<SerializationTestAllSimpleTypes>(Serialize.ToBinary(original));

			Assert.That(deserialized, Is.EqualTo(original));
		}

		public static void UnitySerialization_SerializeAndDeserializeNestedType_AreEqual_Impl()
		{
			var original = new SerializationTestNestedType();

			var deserialized = Serialize.FromBinary<SerializationTestNestedType>(Serialize.ToBinary(original));

			Assert.That(deserialized, Is.EqualTo(original));
		}

		[Test] public void UnitySerialization_SerializeAndDeserializeAllSimpleTypes_AreEqual() =>
			UnitySerialization_SerializeAndDeserializeAllSimpleTypes_AreEqual_Impl();

		[Test] public void UnitySerialization_SerializeAndDeserializeNestedType_AreEqual() =>
			UnitySerialization_SerializeAndDeserializeNestedType_AreEqual_Impl();
	}
}
