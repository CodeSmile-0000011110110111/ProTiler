// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Serialization;
using System;
using Unity.Serialization.Binary;

namespace CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization
{
	public class SerializationTestDataBinaryAdapter : VersionedBinaryAdapterBase,
		IBinaryAdapter<SerializationTestData>
	{
		public SerializationTestDataBinaryAdapter(Byte adapterVersion)
			: base(adapterVersion) {}

		public unsafe void Serialize(in BinarySerializationContext<SerializationTestData> context,
			SerializationTestData data) => data.Serialize(context.Writer);

		public unsafe SerializationTestData Deserialize(
			in BinaryDeserializationContext<SerializationTestData> context)
		{
			var data = new SerializationTestData();
			data.Deserialize(context.Reader, AdapterVersion);
			return data;
		}
	}
}
