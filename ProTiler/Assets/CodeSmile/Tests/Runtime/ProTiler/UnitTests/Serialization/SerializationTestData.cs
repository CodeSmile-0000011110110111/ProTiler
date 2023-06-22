// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Serialization;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Serialization.Binary;

namespace CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization
{
	public struct SerializationTestData : IEquatable<SerializationTestData>, IBinarySerializable
	{
		public const Byte DataVersion = 9;
		public int3 Coord;
		public UInt16 Index;

		public static Boolean operator ==(SerializationTestData left, SerializationTestData right) =>
			left.Equals(right);

		public static Boolean operator !=(SerializationTestData left, SerializationTestData right) =>
			!left.Equals(right);

		public unsafe void Serialize(UnsafeAppendBuffer* writer)
		{
			writer->Add(Coord);
			writer->Add(Index);
		}

		public unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte serializedDataVersion)
		{
			switch (serializedDataVersion)
			{
				case DataVersion:
					Coord = reader->ReadNext<int3>();
					Index = reader->ReadNext<UInt16>();
					break;
				case DataVersion - 1:
					Coord = reader->ReadNext<int3>();
					Index = (UInt16)reader->ReadNext<UInt32>(); // for example in case we previously saved a whole int
					break;
				default:
					throw new SerializationVersionException($"unhandled version {serializedDataVersion}");
			}
		}

		public Boolean Equals(SerializationTestData other) => Coord.Equals(other.Coord) && Index == other.Index;

		public IBinaryAdapter GetBinaryAdapter(Byte adapterVersion) => throw new NotImplementedException();
		public override String ToString() => $"{nameof(SerializationTestData)}({Index}, {Coord})";
		public override Boolean Equals(Object obj) => obj is SerializationTestData other && Equals(other);
		public override Int32 GetHashCode() => HashCode.Combine(Coord, Index);
	}

	public struct DataVersionOld : IBinarySerializable
	{
		public Byte RemainsUnchanged0;
		public Int16 WillChangeTypeInVersion1;
		public Byte RemainsUnchanged1;
		public Int64 WillBeRemovedInVersion1;
		public Byte RemainsUnchanged2;

		public unsafe void Serialize(UnsafeAppendBuffer* writer)
		{
			writer->Add(RemainsUnchanged0);
			writer->Add(WillChangeTypeInVersion1);
			writer->Add(RemainsUnchanged1);
			writer->Add(WillBeRemovedInVersion1);
			writer->Add(RemainsUnchanged2);
		}

		public unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte serializedDataVersion) =>
			throw new NotImplementedException();
	}

	public struct DataVersionCurrent : IBinarySerializable, IEquatable<DataVersionCurrent>
	{
		public const Double NewFieldInitialValue = 1.2345;

		public Byte RemainsUnchanged0;
		public Int64 WillChangeTypeInVersion1;
		public Byte RemainsUnchanged1;
		public Double NewFieldWithNonDefaultValue;
		public Byte RemainsUnchanged2;

		public unsafe void Serialize(UnsafeAppendBuffer* writer)
		{
			writer->Add(RemainsUnchanged0);
			writer->Add(WillChangeTypeInVersion1);
			writer->Add(RemainsUnchanged1);
			writer->Add(NewFieldWithNonDefaultValue);
			writer->Add(RemainsUnchanged2);
		}

		public unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte serializedDataVersion)
		{
			switch (serializedDataVersion)
			{
				case 1:
					RemainsUnchanged0 = reader->ReadNext<Byte>();
					WillChangeTypeInVersion1 = reader->ReadNext<Int64>();
					RemainsUnchanged1 = reader->ReadNext<Byte>();
					NewFieldWithNonDefaultValue = reader->ReadNext<Double>();
					RemainsUnchanged2 = reader->ReadNext<Byte>();
					break;
				case 0:
					RemainsUnchanged0 = reader->ReadNext<Byte>();
					WillChangeTypeInVersion1 = reader->ReadNext<Int16>();
					RemainsUnchanged1 = reader->ReadNext<Byte>();
					reader->ReadNext<Int64>(); // skip bytes for: WillBeRemovedInVersion1
					RemainsUnchanged2 = reader->ReadNext<Byte>();

					// could also be a value computed from the other fields
					NewFieldWithNonDefaultValue = NewFieldInitialValue;
					break;

				default:
					throw new SerializationVersionException($"unhandled data version {serializedDataVersion}");
			}
		}

		public bool Equals(DataVersionCurrent other) => RemainsUnchanged0 == other.RemainsUnchanged0 && WillChangeTypeInVersion1 == other.WillChangeTypeInVersion1 && RemainsUnchanged1 == other.RemainsUnchanged1 && NewFieldWithNonDefaultValue.Equals(other.NewFieldWithNonDefaultValue) && RemainsUnchanged2 == other.RemainsUnchanged2;
		public override bool Equals(object obj) => obj is DataVersionCurrent other && Equals(other);
		public override int GetHashCode() => HashCode.Combine(RemainsUnchanged0, WillChangeTypeInVersion1, RemainsUnchanged1, NewFieldWithNonDefaultValue, RemainsUnchanged2);
		public static bool operator ==(DataVersionCurrent left, DataVersionCurrent right) => left.Equals(right);
		public static bool operator !=(DataVersionCurrent left, DataVersionCurrent right) => !left.Equals(right);
	}
}
