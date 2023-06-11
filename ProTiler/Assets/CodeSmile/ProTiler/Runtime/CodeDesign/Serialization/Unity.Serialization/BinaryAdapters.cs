// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using System;
using Unity.Serialization.Binary;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Serialization.Unity.Serialization
{
	internal sealed class ChunkDataLinearBinaryAdapter<TData> : IBinaryAdapter<ChunkDataLinear<TData>>
		where TData : unmanaged
	{
		public void Serialize(in BinarySerializationContext<ChunkDataLinear<TData>> context,
			ChunkDataLinear<TData> value) => throw new NotImplementedException();

		public ChunkDataLinear<TData> Deserialize(
			in BinaryDeserializationContext<ChunkDataLinear<TData>> context) =>
			throw new NotImplementedException();
	}
}
