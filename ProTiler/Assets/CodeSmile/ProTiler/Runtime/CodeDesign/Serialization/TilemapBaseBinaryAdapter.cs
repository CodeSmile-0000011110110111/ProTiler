// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using System.IO;
using Unity.Serialization.Binary;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Serialization
{
	public class TilemapBaseBinaryAdapter<T> : IBinaryAdapter<T> where T : TilemapBase, new()
	{
			public unsafe void Serialize(in BinarySerializationContext<T> context, T value) =>
				value.Serialize(new BinaryWriter(context.Writer));

			public unsafe T Deserialize(in BinaryDeserializationContext<T> context) =>
				new T().Deserialize(new BinaryReader(context.Reader)) as T;
	}
}
