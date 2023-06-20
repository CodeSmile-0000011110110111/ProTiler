// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;

namespace CodeSmile.ProTiler.CodeDesign._old.UserCode
{
	public interface ITileData {}

	public struct TileData : ITileData
	{
		[CreateProperty] public Int32 TileIndex;
		[CreateProperty] public Int32 TileFlags;
	}

	public struct MySparseBits
	{
		[CreateProperty] public BitField32 bits;
		[CreateProperty] public UnsafeBitArray bitArray;
	}
}
