// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Runtime.CompilerServices;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	public static class Grid3DUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToIndex2D(int x, int y, int width) => y * width + x;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToIndex2D(Vector3Int coord, int width) => coord.z * width + coord.x;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int ToCoord(int index2d, int width, int y = 0)
		{
			return new Vector3Int(index2d % width, y, index2d / width);
		}
	}
}
