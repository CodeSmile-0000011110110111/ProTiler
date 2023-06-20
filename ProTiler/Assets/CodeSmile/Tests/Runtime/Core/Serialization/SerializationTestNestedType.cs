// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.Tests.Runtime.Serialization
{
	public struct SerializationTestNestedType : IEquatable<SerializationTestNestedType>
	{
		public SerializationTestAllSimpleTypes m_SerializationTestAllSimpleTypes;

		public bool Equals(SerializationTestNestedType other) => Equals(m_SerializationTestAllSimpleTypes, other.m_SerializationTestAllSimpleTypes);
		public override bool Equals(object obj) => obj is SerializationTestNestedType other && Equals(other);
		public override int GetHashCode() => (m_SerializationTestAllSimpleTypes != null ? m_SerializationTestAllSimpleTypes.GetHashCode() : 0);
		public static bool operator ==(SerializationTestNestedType left, SerializationTestNestedType right) => left.Equals(right);
		public static bool operator !=(SerializationTestNestedType left, SerializationTestNestedType right) => !left.Equals(right);
	}
}
