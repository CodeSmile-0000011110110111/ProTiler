// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Binary;
using Unity.Serialization.Json;

namespace CodeSmile.ProTiler3.Serialization
{
	[FullCovered]
	internal static class UnitySerializer
	{
		internal static Byte[] ToBinary(object input)
		{
			if (input == null)
				return new Byte[0];

			using (var stream = CreateUnsafeStream())
			{
				unsafe
				{
					BinarySerialization.ToBinary(&stream, input);
				}
				return ToManagedBytes(stream);
			}
		}

		internal static T FromBinary<T>(Byte[] bytes) where T : new()
		{
			if (bytes == null || bytes.Length == 0)
				return new T();

			using (var stream = CreateUnsafeStream(bytes))
			{
				var reader = stream.AsReader();
				unsafe
				{
					return BinarySerialization.FromBinary<T>(&reader);
				}
			}
		}

		internal static String ToJson(object input, Boolean minified = true) =>
			JsonSerialization.ToJson(input, new JsonSerializationParameters { Minified = minified });

		internal static T FromJson<T>(String json) => JsonSerialization.FromJson<T>(json);

		private static UnsafeAppendBuffer CreateUnsafeStream() =>
			new(256, 8, Allocator.Temp);

		private static UnsafeAppendBuffer CreateUnsafeStream(Byte[] bytes)
		{
			unsafe
			{
				fixed (Byte* p = bytes)
				{
					var stream = new UnsafeAppendBuffer(p, bytes.Length);
					stream.Add(p, bytes.Length);
					return stream;
				}
			}
		}

		private static Byte[] ToManagedBytes(in UnsafeAppendBuffer stream)
		{
			var reader = stream.AsReader();
			var bytes = new Byte[reader.Size];
			// TODO: it bothers me that I couldn't find a 'native' way to copy/convert the UnsafeAppendBuffer bytes
			// for now, Marshal.Copy works and is reasonably fast (definitely faster than iterating over bytes)
			unsafe { Marshal.Copy((IntPtr)reader.Ptr, bytes, 0, reader.Size); }
			return bytes;
		}
	}
}
