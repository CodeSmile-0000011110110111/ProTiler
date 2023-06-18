// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Core.Serialization;
using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace CodeSmile.Tests.Runtime.ProTiler.UnitTests.Serialization
{
	public struct SerializationTestData : IEquatable<SerializationTestData>, IBinarySerializable
	{
		public int3 Coord;
		public UInt16 Index;

		public unsafe void Serialize(UnsafeAppendBuffer* writer)
		{
			writer->Add(Coord);
			writer->Add(Index);
		}

		public unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte dataVersion)
		{
			switch (dataVersion)
			{
				case 0:
					Coord = reader->ReadNext<int3>();
					Index = reader->ReadNext<UInt16>();
					break;
				default:
					throw new SerializationVersionException($"unhandled version {dataVersion}");
			}
		}

		public static Boolean operator ==(SerializationTestData left, SerializationTestData right) =>
			left.Equals(right);

		public static Boolean operator !=(SerializationTestData left, SerializationTestData right) =>
			!left.Equals(right);

		public Boolean Equals(SerializationTestData other) => Coord.Equals(other.Coord) && Index == other.Index;
		public override String ToString() => $"{nameof(SerializationTestData)}({Index}, {Coord})";
		public override Boolean Equals(Object obj) => obj is SerializationTestData other && Equals(other);
		public override Int32 GetHashCode() => HashCode.Combine(Coord, Index);
	}
}