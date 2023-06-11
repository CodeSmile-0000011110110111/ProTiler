// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Binary;
using UnityEngine;
using Object = System.Object;

namespace CodeSmile.Tests.Editor.ProTiler
{
	public class BaseImplBinarySerializationTests
	{
		[Test] public void CanSerializeAndDeserializeAbstractBaseClassFromConcrete()
		{
			var impl = new TheImpl();
			var implAsBase = impl as TheBase;
			Assert.That(impl, Is.EqualTo(implAsBase));

			var covAdapters = new List<IBinaryAdapter> { new GenericCovarBinaryAdapter<TheImpl>() };
			var conAdapters = new List<IBinaryAdapter> { new GenericContraImplBinaryAdapter<TheImpl>() };

			var bytesCovarImpl = Serialize.ToBinary(impl, covAdapters);
			Debug.Log($"Cov-Serialize TheImpl: {bytesCovarImpl.Length} Bytes: {bytesCovarImpl.AsString()}");
			//var bytesCovarBase = Serialize.ToBinary(implAsBase, covAdapters);
			//Debug.Log($"Cov-Serialize TheBase: {bytesCovarBase.Length} Bytes: {bytesCovarBase.AsString()}");

			var bytesContraImpl = Serialize.ToBinary(impl, conAdapters);
			Debug.Log($"Con-Serialize TheImpl: {bytesContraImpl.Length} Bytes: {bytesContraImpl.AsString()}");
			// var bytesContraBase = Serialize.ToBinary(implAsBase, conAdapters);
			// Debug.Log($"Con-Serialize TheBase: {bytesContraBase.Length} Bytes: {bytesContraBase.AsString()}");

			var bytesImplNoAdapters = Serialize.ToBinary(impl);
			Debug.Log($"Def-Serialize TheImpl: {bytesImplNoAdapters.Length} Bytes: {bytesImplNoAdapters.AsString()}");
			// var bytesBaseNoAdapters = Serialize.ToBinary(implAsBase);
			// Debug.Log($"Def-Serialize TheBase: {bytesBaseNoAdapters.Length} Bytes: {bytesBaseNoAdapters.AsString()}");

			var implCovar = Serialize.FromBinary<TheImpl>(bytesCovarImpl, covAdapters);
			Assert.That(implCovar, Is.EqualTo(impl));
			// var baseCovar = Serialize.FromBinary<TheBase>(bytesCovarBase, covAdapters);
			// Assert.NotNull(baseCovar as TheImpl);
			// Assert.That(baseCovar, Is.EqualTo(impl));

			var implContra = Serialize.FromBinary<TheImpl>(bytesContraImpl, conAdapters);
			Assert.That(implContra, Is.EqualTo(impl));
			// var baseContra = Serialize.FromBinary<TheBase>(bytesContraBase, conAdapters);
			// Assert.NotNull(baseContra as TheImpl);
			// Assert.That(baseContra, Is.EqualTo(impl));

			var implNoAdapters = Serialize.FromBinary<TheImpl>(bytesImplNoAdapters);
			Assert.That(implNoAdapters, Is.EqualTo(impl));
			// var baseNoAdapters = Serialize.FromBinary<TheBase>(bytesBaseNoAdapters);
			// Assert.NotNull(baseNoAdapters as TheImpl);
			// Assert.That(baseNoAdapters, Is.EqualTo(impl));

			//var deserializedData = Serialize.FromBinary<LinearTileData>(bytes);
			//Assert.That(deserializedData, Is.EqualTo(impl));
		}

		public abstract class TheBase : IEquatable<TheBase>
		{
			public Byte baseField;

			public static Boolean operator ==(TheBase left, TheBase right) => Equals(left, right);
			public static Boolean operator !=(TheBase left, TheBase right) => !Equals(left, right);

			public TheBase() => baseField = 1;

			public TheBase(IBinaryReader reader)
			{
				throw new NotImplementedException();
			}

			public Boolean Equals(TheBase other)
			{
				if (ReferenceEquals(null, other))
					return false;
				if (ReferenceEquals(this, other))
					return true;

				return baseField == other.baseField;
			}

			public override String ToString() => "Base";

			public virtual void Serialize(IBinaryWriter writer) =>
				// serialize base class fields first
				writer.Add(baseField);

			public virtual TheBase Deserialize(IBinaryReader reader)
			{
				// deserialize base class fields first
				baseField = reader.ReadNext<Byte>();
				return this;
			}

			public override Boolean Equals(Object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;
				if (ReferenceEquals(this, obj))
					return true;
				if (obj.GetType() != GetType())
					return false;

				return Equals((TheBase)obj);
			}

			public override Int32 GetHashCode() => baseField.GetHashCode();
		}

		public class TheImpl : TheBase, IEquatable<TheImpl>
		{
			public Byte implField;

			public static Boolean operator ==(TheImpl left, TheImpl right) => Equals(left, right);
			public static Boolean operator !=(TheImpl left, TheImpl right) => !Equals(left, right);

			public TheImpl() => implField = 2;

			public TheImpl(IBinaryReader reader)
				: base(reader)
			{
				throw new NotImplementedException();
			}

			public Boolean Equals(TheImpl other)
			{
				if (ReferenceEquals(null, other))
					return false;
				if (ReferenceEquals(this, other))
					return true;

				return base.Equals(other) && implField == other.implField;
			}

			public override String ToString() => "Impl";

			public override void Serialize(IBinaryWriter writer)
			{
				base.Serialize(writer);
				writer.Add(implField);
			}

			public override TheBase Deserialize(IBinaryReader reader)
			{
				base.Deserialize(reader);
				implField = reader.ReadNext<Byte>();
				return this;
			}

			public override Boolean Equals(Object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;
				if (ReferenceEquals(this, obj))
					return true;
				if (obj.GetType() != GetType())
					return false;

				return Equals((TheImpl)obj);
			}

			public override Int32 GetHashCode() => HashCode.Combine(base.GetHashCode(), implField);
		}

		public interface IBinaryReader
		{
			public T ReadNext<T>() where T : unmanaged;
		}

		public interface IBinaryWriter
		{
			public void Add<T>(T value) where T : unmanaged;
		}

		public unsafe struct BinaryReader : IBinaryReader
		{
			private readonly UnsafeAppendBuffer.Reader* m_Reader;

			public BinaryReader(UnsafeAppendBuffer.Reader* reader) => m_Reader = reader;
			public T ReadNext<T>() where T : unmanaged => m_Reader->ReadNext<T>();
		}

		public unsafe struct BinaryWriter : IBinaryWriter
		{
			private readonly UnsafeAppendBuffer* m_Writer;
			public BinaryWriter(UnsafeAppendBuffer* writer) => m_Writer = writer;
			public void Add<T>(T value) where T : unmanaged => m_Writer->Add(value);
		}

		public class GenericCovarBinaryAdapter<T> : IBinaryAdapter<T> where T : TheBase, new()
		{
			public unsafe void Serialize(in BinarySerializationContext<T> context, T value) =>
				value.Serialize(new BinaryWriter(context.Writer));

			public unsafe T Deserialize(in BinaryDeserializationContext<T> context) =>
				new T().Deserialize(new BinaryReader(context.Reader)) as T;
		}

		public class GenericContraImplBinaryAdapter<T> : IContravariantBinaryAdapter<T> where T : TheBase, new()
		{
			public unsafe void Serialize(IBinarySerializationContext context, T value) =>
				value.Serialize(new BinaryWriter(context.Writer));

			public unsafe Object Deserialize(IBinaryDeserializationContext context) =>
				new T().Deserialize(new BinaryReader(context.Reader)) as T;
		}

		public class CovarBinaryAdapter : IBinaryAdapter<TheImpl>
		{
			public unsafe void Serialize(in BinarySerializationContext<TheImpl> context, TheImpl value) =>
				value.Serialize(new BinaryWriter(context.Writer));

			public unsafe TheImpl Deserialize(in BinaryDeserializationContext<TheImpl> context) =>
				new(new BinaryReader(context.Reader));
		}

		public class ContraImplBinaryAdapter : IContravariantBinaryAdapter<TheImpl>
		{
			public unsafe void Serialize(IBinarySerializationContext context, TheImpl value) =>
				value.Serialize(new BinaryWriter(context.Writer));

			public unsafe Object Deserialize(IBinaryDeserializationContext context) =>
				new TheImpl(new BinaryReader(context.Reader));
		}
	}
}
