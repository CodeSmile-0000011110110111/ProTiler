// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.Runtime.CompilerServices;
using Unity.Burst;

namespace CodeSmile
{
	[FullCovered]
	public static class HashUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int64 GetHash(Int32 a, Int32 b)
		{
			var hash = (Int64)a;
			hash = hash + 0xabcd1234 + (hash << 15);
			hash = hash + 0x0987efab ^ hash >> 11;
			hash ^= b;
			hash = hash + 0x46ac12fd + (hash << 7);
			hash = hash + 0xbe9730af ^ hash << 11;
			return hash;
		}
	}
}
