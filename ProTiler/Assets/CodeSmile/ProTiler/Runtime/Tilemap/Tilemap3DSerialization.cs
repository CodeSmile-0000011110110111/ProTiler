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

namespace CodeSmile.ProTiler.Tilemap
{
	[FullCovered]
	internal static class Tilemap3DSerialization
	{
		[Pure] internal static Byte[] ToBinary(Tilemap3D tilemap)
		{
			if (tilemap == null)
				return new Byte[0];

			using (var stream = CreateUnsafeStream())
			{
				unsafe
				{
					BinarySerialization.ToBinary(&stream, tilemap);
				}
				return ToManagedBytes(stream);
			}
		}

		[Pure] internal static Tilemap3D FromBinary(Byte[] bytes)
		{
			if (bytes == null || bytes.Length == 0)
				return new Tilemap3D();

			using (var stream = CreateUnsafeStream(bytes))
			{
				var reader = stream.AsReader();
				unsafe
				{
					return BinarySerialization.FromBinary<Tilemap3D>(&reader);
				}
			}
		}

		[Pure] internal static String ToJson(Tilemap3D tilemap, Boolean minified = true) => JsonSerialization.ToJson(
			tilemap,
			new JsonSerializationParameters { Minified = minified });

		[Pure] internal static Tilemap3D FromJson(String json) => JsonSerialization.FromJson<Tilemap3D>(json);

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
			// for now, Marshal.Copy works and seems rather fast (definitely a lot better than iteration)
			unsafe { Marshal.Copy((IntPtr)reader.Ptr, bytes, 0, reader.Size); }
			return bytes;
		}
	}
}
