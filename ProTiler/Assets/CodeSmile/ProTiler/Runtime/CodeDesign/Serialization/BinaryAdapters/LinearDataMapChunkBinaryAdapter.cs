// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Binary;
using UnityEngine;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.Model.Serialization
{
	public class LinearDataMapChunkBinaryAdapter<TData> : IBinaryAdapter<LinearDataMapChunk<TData>>
		where TData : unmanaged
	{
		public unsafe void Serialize(in BinarySerializationContext<LinearDataMapChunk<TData>> context,
			LinearDataMapChunk<TData> chunk)
		{
		}

		public unsafe LinearDataMapChunk<TData> Deserialize(
			in BinaryDeserializationContext<LinearDataMapChunk<TData>> context)
		{
			return new LinearDataMapChunk<TData>();
		}
	}
}
