// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tests.Runtime.Core.Serialization;
using NUnit.Framework;

namespace CodeSmile.Tests.Editor.ProTiler.UnitTests.Serialization
{
	public class UnityEditorSerializationTests
	{
		[Test] public void UnitySerialization_SerializeAndDeserializeAllSimpleTypes_AreEqual() =>
			UnitySerializationTests.UnitySerialization_SerializeAndDeserializeAllSimpleTypes_AreEqual_Impl();

		[Test] public void UnitySerialization_SerializeAndDeserializeNestedType_AreEqual() =>
			UnitySerializationTests.UnitySerialization_SerializeAndDeserializeNestedType_AreEqual_Impl();
	}
}
