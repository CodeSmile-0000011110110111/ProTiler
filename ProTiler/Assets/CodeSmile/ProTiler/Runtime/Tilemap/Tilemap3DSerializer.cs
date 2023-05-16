// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Serialization.Binary;
using Unity.Serialization.Json;

namespace CodeSmile.ProTiler.Tilemap
{
	[FullCovered]
	internal static class Tilemap3DSerializer
	{
		private enum BinaryCopyStrategy
		{
			AtomicByte,
			MarshalCopy,
		}

		[Pure] internal static Byte[] ToBinary(Tilemap3D tilemap)
		{
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
			using (var stream = CreateUnsafeStream(bytes))
			{
				var reader = stream.AsReader();
				unsafe
				{
					return BinarySerialization.FromBinary<Tilemap3D>(&reader);
				}
			}
		}

		[Pure] internal static String ToJson(Tilemap3D tilemap, Boolean minified = true) => JsonSerialization.ToJson(tilemap,
			new JsonSerializationParameters { Minified = minified });

		[Pure] internal static Tilemap3D FromJson(String json) => JsonSerialization.FromJson<Tilemap3D>(json);

		[Pure] private static UnsafeAppendBuffer CreateUnsafeStream() => new(256, 8, Allocator.Persistent);

		[Pure] private static UnsafeAppendBuffer CreateUnsafeStream(Byte[] bytes)
		{
			unsafe
			{
				fixed (Byte* p = bytes)
					return new UnsafeAppendBuffer(p, bytes.Length);
			}
		}

		[Pure] private static Byte[] ToManagedBytes(in UnsafeAppendBuffer stream,
			BinaryCopyStrategy copyStrategy = BinaryCopyStrategy.AtomicByte)
		{
			var reader = stream.AsReader();
			var bytes = new Byte[reader.Size];

			switch (copyStrategy)
			{
				case BinaryCopyStrategy.AtomicByte:
				{
					var index = 0;
					while (reader.EndOfBuffer == false)
					{
						bytes[index] = reader.ReadNext<Byte>();
						index++;
					}
					break;
				}
				case BinaryCopyStrategy.MarshalCopy:
				{
					unsafe
					{
						Marshal.Copy((IntPtr)reader.Ptr, bytes, 0, reader.Size);
					}
					break;
				}
				default: throw new ArgumentOutOfRangeException(nameof(copyStrategy), copyStrategy, null);
			}

			return bytes;
		}
	}
}
