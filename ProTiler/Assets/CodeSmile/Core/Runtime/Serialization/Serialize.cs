// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;
using Unity.Serialization.Binary;

namespace CodeSmile.Serialization
{
	public static class Serialize
	{
		public static unsafe Byte[] ToBinary<T>(T data, List<IBinaryAdapter> adapters = null)
		{
			var stream = new UnsafeAppendBuffer(16, 8, Allocator.Temp);
			var parameters = new BinarySerializationParameters { UserDefinedAdapters = adapters };
			BinarySerialization.ToBinary(&stream, data, parameters);

			var bytes = stream.ToBytesNBC();
			stream.Dispose();

			return bytes;
		}

		public static unsafe T FromBinary<T>(Byte[] bytes, List<IBinaryAdapter> adapters = null)
		{
			fixed (Byte* ptr = bytes)
			{
				var reader = new UnsafeAppendBuffer.Reader(ptr, bytes.Length);
				var parameters = new BinarySerializationParameters { UserDefinedAdapters = adapters };
				return BinarySerialization.FromBinary<T>(&reader, parameters);
			}
		}
	}
}
