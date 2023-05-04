// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Runtime.CompilerServices;
using UnityEngine;

namespace CodeSmile.ProTiler.Utilities
{
	public static class Grid3DUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToIndex2D(int x, int y, int width) => y * width + x;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToIndex2D(Vector3Int coord, int width) => coord.z * width + coord.x;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int ToCoord(int index2d, int width, int y = 0) => new(index2d % width, y, index2d / width);

		/// <summary>
		/// Converts a world position to cell coordinates.
		///
		/// Note: a simple int-cast won't do because ((int)-0.1234f) will be 0 where it should be -1!
		/// FloorToInt() rounds towards infinity to give the correct result whereas casting to int rounds towards zero).
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <param name="cellSize"></param>
		/// <returns></returns>
		public static Vector3Int ToCoord(Vector3 worldPosition, Vector3Int cellSize) => new(
			Mathf.FloorToInt(worldPosition.x * (1f / cellSize.x)),
			Mathf.FloorToInt(worldPosition.y * (1f / cellSize.y)),
			Mathf.FloorToInt(worldPosition.z * (1f / cellSize.z)));
	}
}
