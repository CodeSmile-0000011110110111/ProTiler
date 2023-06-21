// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Serialization.Binary;

namespace CodeSmile.Serialization.BinaryAdapters
{
	public class GenericSerializableAdapter<T> : IBinaryAdapter<T> where T : unmanaged, IBinarySerializable
	{
		private byte m_AdapterVersion;
		public GenericSerializableAdapter(byte adapterVersion)
		{
			m_AdapterVersion = adapterVersion;
		}

		public unsafe void Serialize(in BinarySerializationContext<T> context, T value)
		{
			value.Serialize(context.Writer);
		}

		public unsafe T Deserialize(in BinaryDeserializationContext<T> context)
		{
			var instance = new T();
			instance.Deserialize(context.Reader, 0); // TODO: where does version come from?
			return instance;
		}
	}
}
