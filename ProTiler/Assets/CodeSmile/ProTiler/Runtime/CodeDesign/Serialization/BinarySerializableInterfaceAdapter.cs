// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Core.Serialization;
using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using System;
using Unity.Serialization.Binary;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Serialization
{
	public class BinarySerializableInterfaceAdapter<T> : VersionedBinaryAdapterBase, IBinaryAdapter<IBinarySerializable>
		where T : unmanaged, IBinarySerializable
	{
		public BinarySerializableInterfaceAdapter(Byte adapterVersion)
			: base(adapterVersion) {}

		public unsafe void Serialize(in BinarySerializationContext<IBinarySerializable> context,
			IBinarySerializable value) => value.Serialize(context.Writer);

		public unsafe IBinarySerializable Deserialize(in BinaryDeserializationContext<IBinarySerializable> context)
		{
			var data = new T();
			data.Deserialize(context.Reader, AdapterVersion);
			return data; // TODO: avoid boxing! Task: https://www.pivotaltracker.com/story/show/185427726
		}
	}
}
