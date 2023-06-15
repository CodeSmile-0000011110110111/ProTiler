// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.Tests.Runtime.Core.Serialization
{
	public class SerializationTestAllSimpleTypes : IEquatable<SerializationTestAllSimpleTypes>
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

		public static Boolean operator ==(SerializationTestAllSimpleTypes left, SerializationTestAllSimpleTypes right) => Equals(left, right);
		public static Boolean operator !=(SerializationTestAllSimpleTypes left, SerializationTestAllSimpleTypes right) => !Equals(left, right);

		public Boolean Equals(SerializationTestAllSimpleTypes other)
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

			return Equals((SerializationTestAllSimpleTypes)obj);
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
