// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections.LowLevel.Unsafe;

namespace CodeSmile.Serialization
{
	public interface IBinarySerializable
	{
		unsafe void Serialize(UnsafeAppendBuffer* writer);
		unsafe void Deserialize(UnsafeAppendBuffer.Reader* reader, Byte dataVersion);
	}
}
