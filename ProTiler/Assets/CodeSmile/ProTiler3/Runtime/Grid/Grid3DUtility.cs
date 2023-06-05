// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.Runtime.CompilerServices;
using WorldPos = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using Math = Unity.Mathematics.math;

namespace CodeSmile.ProTiler.Grid
{
	[FullCovered]
	internal static class Grid3DUtility
	{
		/// <summary>
		///     Converts grid coord to world position.
		/// </summary>
		/// <param name="coord"></param>
		/// <param name="cellSize"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorldPos ToWorldPos(GridCoord coord, CellSize cellSize) =>
			new WorldPos(coord.x * cellSize.x, coord.y * cellSize.y, coord.z * cellSize.z) + cellSize * .5f;

		/// <summary>
		///     Converts x/z coordinate to a 2D array index.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="z"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Int32 ToIndex2D(Int32 x, Int32 z, Int32 width) => z * width + x;

		/// <summary>
		///     Converts grid coordinate to a 2D array index.
		/// </summary>
		/// <param name="coord"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Int32 ToIndex2D(GridCoord coord, Int32 width) => coord.z * width + coord.x;

		/// <summary>
		///     Converts 2D array index to grid coordinate.
		/// </summary>
		/// <param name="index2d"></param>
		/// <param name="width"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static GridCoord ToGridCoord(Int32 index2d, Int32 width, Int32 y = 0) =>
			new(index2d % width, y, index2d / width);

		/// <summary>
		///     Converts a world position to cell coordinates.
		///     Note: a simple int-cast won't do because ((int)-0.1234f) will be 0 where it should be -1!
		///     FloorToInt() rounds towards infinity to give the correct result whereas casting to int rounds towards zero).
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <param name="cellSize"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static GridCoord ToGridCoord(WorldPos worldPosition, CellSize cellSize) => new(
			(Int32)Math.floor((Double)(worldPosition.x * (1f / cellSize.x))),
			(Int32)Math.floor((Double)(worldPosition.y * (1f / cellSize.y))),
			(Int32)Math.floor((Double)(worldPosition.z * (1f / cellSize.z))));
	}
}
