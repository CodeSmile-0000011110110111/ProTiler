// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Runtime.CompilerServices;
using WorldPos = UnityEngine.Vector3;
using GridCoord = UnityEngine.Vector3Int;
using CellSize = UnityEngine.Vector3Int;
using Math = UnityEngine.Mathf;

namespace CodeSmile.ProTiler.Data
{
	public static class Grid3DUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToIndex2D(int x, int y, int width) => y * width + x;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToIndex2D(GridCoord coord, int width) => coord.z * width + coord.x;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static GridCoord ToCoord(int index2d, int width, int y = 0) => new(index2d % width, y, index2d / width);

		/// <summary>
		///     Converts a world position to cell coordinates.
		///     Note: a simple int-cast won't do because ((int)-0.1234f) will be 0 where it should be -1!
		///     FloorToInt() rounds towards infinity to give the correct result whereas casting to int rounds towards zero).
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <param name="cellSize"></param>
		/// <returns></returns>
		public static GridCoord ToCoord(WorldPos worldPosition, CellSize cellSize) => new(
			Math.FloorToInt(worldPosition.x * (1f / cellSize.x)),
			Math.FloorToInt(worldPosition.y * (1f / cellSize.y)),
			Math.FloorToInt(worldPosition.z * (1f / cellSize.z)));
	}
}
