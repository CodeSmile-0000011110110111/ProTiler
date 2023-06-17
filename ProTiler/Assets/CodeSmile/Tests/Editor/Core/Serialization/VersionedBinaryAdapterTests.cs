// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Core.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Serialization.Binary;

namespace CodeSmile.Tests.Editor.Core.Serialization
{
	public class VersionedBinaryAdapterTests
	{
		private const Byte CurrentAdapterVersion = 3;
		private const Byte PreviousDataVersionDefaultValue = Byte.MaxValue;

		[Test] public void VersionedBinaryAdapter_Ctor_VersionPropertyMatchesVersion()
		{
			var adapter = new TestVersionedBinaryAdapter(CurrentAdapterVersion);

			Assert.That(adapter.BinaryAdapterVersion, Is.EqualTo(CurrentAdapterVersion));
		}

		[Test] public void VersionedBinaryAdapter_Serialize_FirstByteIsVersion()
		{
			var byteValue = (Byte)(CurrentAdapterVersion * 2);
			var data = new TestStruct { byteValue = byteValue };

			var adapters = new List<IBinaryAdapter> { new TestVersionedBinaryAdapter(CurrentAdapterVersion) };
			var bytes = Serialize.ToBinary(data, adapters);

			Assert.NotNull(bytes);
			Assert.That(bytes[0], Is.EqualTo(CurrentAdapterVersion));
			Assert.That(bytes[1], Is.EqualTo(byteValue));
		}

		[Test] public void VersionedBinaryAdapter_DeserializePreviousVersion_ReturnsDefaultValue()
		{
			var previousVersion = (Byte)(CurrentAdapterVersion - 1);
			var adapters = new List<IBinaryAdapter> { new TestVersionedBinaryAdapter(previousVersion) };
			var data = Serialize.FromBinary<TestStruct>(new[] { previousVersion }, adapters);

			Assert.That(data.byteValue, Is.EqualTo(PreviousDataVersionDefaultValue));
		}

		[Test] public void VersionedBinaryAdapter_DeserializeIncompatibleVersion_ThrowsSerializationException()
		{
			var incompatibleVersion = (Byte)(CurrentAdapterVersion - 2);
			var adapters = new List<IBinaryAdapter> { new TestVersionedBinaryAdapter(incompatibleVersion) };

			Assert.Throws<SerializationVersionException>(() =>
			{
				Serialize.FromBinary<TestStruct>(new[] { incompatibleVersion }, adapters);
			});
		}

		[Test] public void VersionedBinaryAdapter_DeserializeFutureVersion_ThrowsSerializationException()
		{
			var futureVersion = (Byte)(CurrentAdapterVersion + 1);
			var adapters = new List<IBinaryAdapter> { new TestVersionedBinaryAdapter(futureVersion) };

			Assert.Throws<SerializationVersionException>(() =>
			{
				Serialize.FromBinary<TestStruct>(new[] { futureVersion }, adapters);
			});
		}

		public struct TestStruct
		{
			public Byte byteValue;
		}

		public class TestVersionedBinaryAdapter : VersionedBinaryAdapterBase, IBinaryAdapter<TestStruct>
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
				var serializedDataVersion = ReadAdapterVersion(reader);

				if (serializedDataVersion == CurrentAdapterVersion)
					return new TestStruct { byteValue = reader->ReadNext<Byte>() };

				// handle previous version
				// uses default value because "byteValue" field was supposedly added in a later version
				if (serializedDataVersion == CurrentAdapterVersion - 1)
					return new TestStruct { byteValue = PreviousDataVersionDefaultValue };

				if (serializedDataVersion > CurrentAdapterVersion)
				{
					throw new SerializationVersionException(
						GetFutureVersionExceptionMessage(serializedDataVersion, CurrentAdapterVersion));
				}

				throw new SerializationVersionException(
					GetLegacyVersionExceptionMessage(serializedDataVersion, CurrentAdapterVersion));
			}
		}
	}
}
