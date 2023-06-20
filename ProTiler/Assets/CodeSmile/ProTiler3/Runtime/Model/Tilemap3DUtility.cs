// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ChunkKey = System.Int64;
using GridCoord = Unity.Mathematics.int3;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int2;
using LayerCoord = Unity.Mathematics.int3;
using Math = Unity.Mathematics.math;

namespace CodeSmile.ProTiler3.Model
{
	[FullCovered]
	internal static class Tilemap3DUtility
	{
		internal static IEnumerable<GridCoord> GetChunkGridCoords(ChunkCoord chunkCoord, ChunkSize chunkSize,
			Int32 height = 0)
		{
			var coords = new GridCoord[chunkSize.x * chunkSize.y];
			var index = 0;
			for (var z = 0; z < chunkSize.y; z++)
			{
				for (var x = 0; x < chunkSize.x; x++)
				{
					coords[index] = LayerToGridCoord(new LayerCoord(x, height, z), chunkCoord, chunkSize);
					index++;
				}
			}
			return coords;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ChunkSize ClampChunkSize(ChunkSize chunkSize)
		{
			chunkSize.x = Math.max(Tilemap3D.MinChunkSize.x, chunkSize.x);
			chunkSize.y = Math.max(Tilemap3D.MinChunkSize.y, chunkSize.y);
			return chunkSize;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ChunkKey GetChunkKey(GridCoord coord, ChunkSize chunkSize)
		{
			var chunkCoord = GridToChunkCoord(coord, chunkSize);
			return GetChunkKey(chunkCoord);
		}

		/// <summary>
		///     Note: negative grid coordinates result in negative chunk coordinates - but offset by 1. There may
		///     be a generic way to calculate this but the straightforward solution using ternary works just fine.
		///     Examples for ChunkSize(2,2):
		///     Grid(-1,0,-1) => Chunk(-1,-1)
		///     Grid(-2,0,-2) => Chunk(-1,-1)
		///     Grid(-3,0,-3) => Chunk(-2,-2)
		///     Grid(-4,0,-4) => Chunk(-2,-2)
		/// </summary>
		/// <param name="gridCoord"></param>
		/// <param name="chunkSize"></param>
		/// <returns></returns>
		// TODO: these +1/-1 can probably be refactored to a more generic algorithm
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ChunkCoord GridToChunkCoord(GridCoord gridCoord, ChunkSize chunkSize)
		{
			return new ChunkCoord(
				gridCoord.x < 0 ? -(Math.abs(gridCoord.x + 1) / chunkSize.x + 1) : gridCoord.x / chunkSize.x,
				gridCoord.z < 0 ? -(Math.abs(gridCoord.z + 1) / chunkSize.y + 1) : gridCoord.z / chunkSize.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ChunkKey GetChunkKey(ChunkCoord chunkCoord) => HashUtility.GetHash(chunkCoord.x, chunkCoord.y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static GridCoord LayerToGridCoord(LayerCoord layerCoord, ChunkCoord chunkCoord, ChunkSize chunkSize) =>
			new(chunkCoord.x * chunkSize.x + layerCoord.x,
				layerCoord.y,
				chunkCoord.y * chunkSize.y + layerCoord.z);



		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static LayerCoord GridToLayerCoord(GridCoord gridCoord, ChunkSize chunkSize) => new(
			Math.abs(gridCoord.x) % chunkSize.x,
			Math.max(0, gridCoord.y),
			Math.abs(gridCoord.z) % chunkSize.y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static GridCoord ChunkToGridCoord(ChunkCoord chunkCoord, ChunkSize chunkSize) =>
			new(chunkCoord.x * chunkSize.x, 0, chunkCoord.y * chunkSize.y);

		[ExcludeFromCodeCoverage] static Tilemap3DUtility() {}
	}
}
