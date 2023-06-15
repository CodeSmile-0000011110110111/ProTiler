// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Serialization;
using Unity.Serialization.Binary;

namespace CodeSmile.Tests.Runtime.Core.Serialization
{
	public class VersionedBinaryAdapterTests
	{
		private const Byte CurrentSerializedVersion = 3;
		private const Byte PreviousSerializedVersionDefaultValue = byte.MaxValue;

		[Test] public void VersionedBinaryAdapter_Ctor_VersionPropertyMatchesVersion()
		{
			var adapter = new TestVersionedBinaryAdapter(CurrentSerializedVersion);

			Assert.That(adapter.BinaryAdapterVersion, Is.EqualTo(CurrentSerializedVersion));
		}

		[Test] public void VersionedBinaryAdapter_Serialize_FirstByteIsVersion()
		{
			var byteValue = (Byte)(CurrentSerializedVersion * 2);
			var data = new TestStruct { byteValue = byteValue };

			var adapters = new List<IBinaryAdapter> { new TestVersionedBinaryAdapter(CurrentSerializedVersion) };
			var bytes = Serialize.ToBinary(data, adapters);

			Assert.NotNull(bytes);
			Assert.That(bytes[0], Is.EqualTo(CurrentSerializedVersion));
			Assert.That(bytes[1], Is.EqualTo(byteValue));
		}

		[Test] public void VersionedBinaryAdapter_DeserializePreviousVersion_ReturnsDefaultValue()
		{
			var previousVersion = (Byte)(CurrentSerializedVersion - 1);
			var adapters = new List<IBinaryAdapter> { new TestVersionedBinaryAdapter(previousVersion) };
			var data = Serialize.FromBinary<TestStruct>(new[] { previousVersion }, adapters);

			Assert.That(data.byteValue, Is.EqualTo(PreviousSerializedVersionDefaultValue));
		}

		[Test] public void VersionedBinaryAdapter_DeserializeIncompatibleVersion_ThrowsSerializationException()
		{
			var incompatibleVersion = (Byte)(CurrentSerializedVersion - 2);
			var adapters = new List<IBinaryAdapter> { new TestVersionedBinaryAdapter(incompatibleVersion) };

			Assert.Throws<SerializationException>(() =>
			{
				Serialize.FromBinary<TestStruct>(new[] { incompatibleVersion }, adapters);
			});
		}

		[Test] public void VersionedBinaryAdapter_DeserializeFutureVersion_ThrowsSerializationException()
		{
			var futureVersion = (Byte)(CurrentSerializedVersion + 1);
			var adapters = new List<IBinaryAdapter> { new TestVersionedBinaryAdapter(futureVersion) };

			Assert.Throws<SerializationException>(() =>
			{
				Serialize.FromBinary<TestStruct>(new[] { futureVersion }, adapters);
			});
		}

		public struct TestStruct
		{
			public Byte byteValue;
		}

		public class TestVersionedBinaryAdapter : VersionedBinaryAdapter, IBinaryAdapter<TestStruct>
		{
			public Byte BinaryAdapterVersion => AdapterVersion;

			public TestVersionedBinaryAdapter(Byte adapterVersion)
				: base(adapterVersion) {}

			public unsafe void Serialize(in BinarySerializationContext<TestStruct> context, TestStruct value)
			{
				var writer = context.Writer;
				WriteAdapterVersion(writer);

				writer->Add(value.byteValue);
			}

			public unsafe TestStruct Deserialize(in BinaryDeserializationContext<TestStruct> context)
			{
				var reader = context.Reader;
				var dataVersion = ReadSerializedVersion(reader);

				if (dataVersion == CurrentSerializedVersion)
					return new TestStruct { byteValue = reader->ReadNext<Byte>() };

				// handle previous version
				// uses default value because "byteValue" field was supposedly added in a later version
				if (dataVersion == CurrentSerializedVersion - 1)
					return new TestStruct { byteValue = PreviousSerializedVersionDefaultValue };

				if (dataVersion > CurrentSerializedVersion)
					throw new SerializationException($"can not read future version {dataVersion}");

				throw new SerializationException($"incompatible earlier version {dataVersion}");
			}
		}
	}
}
