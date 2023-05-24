// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Binary;
using Unity.Serialization.Json;

namespace CodeSmile.ProTiler
{
	[FullCovered]
	internal static class UnitySerialization
	{
		[Pure] internal static Byte[] ToBinary(object input)
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

		[Pure] internal static T FromBinary<T>(Byte[] bytes) where T : new()
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

		[Pure] internal static String ToJson(object input, Boolean minified = true) =>
			JsonSerialization.ToJson(input, new JsonSerializationParameters { Minified = minified });

		[Pure] internal static T FromJson<T>(String json) => JsonSerialization.FromJson<T>(json);

		[Pure] private static UnsafeAppendBuffer CreateUnsafeStream() =>
			new(65536, 8, Allocator.Temp);

		[Pure] private static UnsafeAppendBuffer CreateUnsafeStream(Byte[] bytes)
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

		[Pure] private static Byte[] ToManagedBytes(in UnsafeAppendBuffer stream)
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
