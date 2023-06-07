// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;
using Unity.Serialization.Binary;
using Unity.Serialization.Json;

namespace CodeSmile.Tests.Editor.ProTiler
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

		public static String ToJson<T>(T data, List<IJsonAdapter> adapters = null)
		{
			var parameters = new JsonSerializationParameters { UserDefinedAdapters = adapters };
			return JsonSerialization.ToJson(data, parameters);
		}

		private static Stream ToStream(this string value, Encoding encoding = null)
			=> new MemoryStream((encoding ?? Encoding.ASCII).GetBytes(value ?? string.Empty));
		public static T FromJson<T>(String json, List<IJsonAdapter> adapters = null)
		{
			/*var stream = json.ToStream();
			using (var reader = new SerializedObjectReader(stream))
			{
				NodeType node = reader.Step(out SerializedValueView current);

				switch (node)
				{
					case NodeType.BeginObject:
					case NodeType.EndObject:
						break;
					case NodeType.Primitive:
						var value = current.AsInt64();
						break;
				}
			}*/

			var parameters = new JsonSerializationParameters { UserDefinedAdapters = adapters };
			return JsonSerialization.FromJson<T>(json, parameters);
		}
	}
}
