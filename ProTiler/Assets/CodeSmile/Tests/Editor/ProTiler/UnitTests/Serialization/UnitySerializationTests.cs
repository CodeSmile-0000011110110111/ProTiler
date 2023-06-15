// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Serialization;
using NUnit.Framework;
using System;
using Object = System.Object;

namespace CodeSmile.Tests.Editor.ProTiler.UnitTests.Serialization
{
	public class UnitySerializationTests
	{
		[Test] public void UnitySerialization_SerializeAndDeserializeAllSimpleTypes_AreEqual()
		{
			var original = new AllSimpleTypes();

			var deserialized = Serialize.FromBinary<AllSimpleTypes>(Serialize.ToBinary(original));

			Assert.That(deserialized, Is.EqualTo(original));
		}

		[Test] public void UnitySerialization_SerializeAndDeserializeNestedType_AreEqual()
		{
			var original = new NestedType();

			var deserialized = Serialize.FromBinary<NestedType>(Serialize.ToBinary(original));

			Assert.That(deserialized, Is.EqualTo(original));
		}

		public struct NestedType : IEquatable<NestedType>
		{
			public AllSimpleTypes allSimpleTypes;

			public bool Equals(NestedType other) => Equals(allSimpleTypes, other.allSimpleTypes);
			public override bool Equals(object obj) => obj is NestedType other && Equals(other);
			public override int GetHashCode() => (allSimpleTypes != null ? allSimpleTypes.GetHashCode() : 0);
			public static bool operator ==(NestedType left, NestedType right) => left.Equals(right);
			public static bool operator !=(NestedType left, NestedType right) => !left.Equals(right);
		}


		public class AllSimpleTypes : IEquatable<AllSimpleTypes>
		{
			public Boolean boolValue = true;
			public Decimal decimalValueMin = Decimal.MinValue;
			public Decimal decimalValueMax = Decimal.MinValue;
			public SByte sbyteValueMin = SByte.MinValue;
			public SByte sbyteValueMax = SByte.MaxValue;
			public Byte byteValueMin = Byte.MinValue;
			public Byte byteValueMax = Byte.MaxValue;
			public Int16 shortValueMin = Int16.MinValue;
			public Int16 shortValueMax = Int16.MaxValue;
			public UInt16 ushortValueMin = UInt16.MinValue;
			public UInt16 ushortValueMax = UInt16.MaxValue;
			public Int32 intValueMin = Int32.MinValue;
			public Int32 intValueMax = Int32.MaxValue;
			public UInt32 uintValueMin = UInt32.MinValue;
			public UInt32 uintValueMax = UInt32.MaxValue;
			public Int64 longValueMin = Int64.MinValue;
			public Int64 longValueMax = Int64.MaxValue;
			public UInt64 ulongValueMin = UInt64.MinValue;
			public UInt64 ulongValueMax = UInt64.MaxValue;
			public Char charValueMin = Char.MinValue;
			public Char charValueMax = Char.MaxValue;
			public Single floatValueMin = Single.MinValue;
			public Single floatValueMax = Single.MaxValue;
			public Double doubleValueMin = Double.MinValue;
			public Double doubleValueMax = Double.MaxValue;

			public static Boolean operator ==(AllSimpleTypes left, AllSimpleTypes right) => Equals(left, right);
			public static Boolean operator !=(AllSimpleTypes left, AllSimpleTypes right) => !Equals(left, right);

			public Boolean Equals(AllSimpleTypes other)
			{
				if (ReferenceEquals(null, other))
					return false;
				if (ReferenceEquals(this, other))
					return true;

				return boolValue == other.boolValue && decimalValueMin == other.decimalValueMin &&
				       decimalValueMax == other.decimalValueMax && sbyteValueMin == other.sbyteValueMin &&
				       sbyteValueMax == other.sbyteValueMax && byteValueMin == other.byteValueMin &&
				       byteValueMax == other.byteValueMax && shortValueMin == other.shortValueMin &&
				       shortValueMax == other.shortValueMax && ushortValueMin == other.ushortValueMin &&
				       ushortValueMax == other.ushortValueMax && intValueMin == other.intValueMin &&
				       intValueMax == other.intValueMax && uintValueMin == other.uintValueMin &&
				       uintValueMax == other.uintValueMax && longValueMin == other.longValueMin &&
				       longValueMax == other.longValueMax && ulongValueMin == other.ulongValueMin &&
				       ulongValueMax == other.ulongValueMax && charValueMin == other.charValueMin &&
				       charValueMax == other.charValueMax && floatValueMin.Equals(other.floatValueMin) &&
				       floatValueMax.Equals(other.floatValueMax) && doubleValueMin.Equals(other.doubleValueMin) &&
				       doubleValueMax.Equals(other.doubleValueMax);
			}

			public override Boolean Equals(Object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;
				if (ReferenceEquals(this, obj))
					return true;
				if (obj.GetType() != GetType())
					return false;

				return Equals((AllSimpleTypes)obj);
			}

			public override Int32 GetHashCode()
			{
				var hashCode = new HashCode();
				hashCode.Add(boolValue);
				hashCode.Add(decimalValueMin);
				hashCode.Add(decimalValueMax);
				hashCode.Add(sbyteValueMin);
				hashCode.Add(sbyteValueMax);
				hashCode.Add(byteValueMin);
				hashCode.Add(byteValueMax);
				hashCode.Add(shortValueMin);
				hashCode.Add(shortValueMax);
				hashCode.Add(ushortValueMin);
				hashCode.Add(ushortValueMax);
				hashCode.Add(intValueMin);
				hashCode.Add(intValueMax);
				hashCode.Add(uintValueMin);
				hashCode.Add(uintValueMax);
				hashCode.Add(longValueMin);
				hashCode.Add(longValueMax);
				hashCode.Add(ulongValueMin);
				hashCode.Add(ulongValueMax);
				hashCode.Add(charValueMin);
				hashCode.Add(charValueMax);
				hashCode.Add(floatValueMin);
				hashCode.Add(floatValueMax);
				hashCode.Add(doubleValueMin);
				hashCode.Add(doubleValueMax);
				return hashCode.ToHashCode();
			}
		}
	}
}
