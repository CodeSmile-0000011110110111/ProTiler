// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ChunkKey = System.Int64;
using GridCoord = UnityEngine.Vector3Int;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;
using LayerCoord = UnityEngine.Vector3Int;
using Math = UnityEngine.Mathf;

namespace CodeSmile.ProTiler.Utility
{
	[FullCovered]
	internal static class Tilemap3DUtility
	{
		/// <summary>
		///     This is a required technical limitation to have at least 2x2 tiles per chunk.
		///     The hashes of chunks with size of less than 2x2 would not be unique.
		/// </summary>
		internal static readonly ChunkSize MinChunkSize = new(2, 2);

		internal static IEnumerable<GridCoord> GetAllChunkLayerCoords(ChunkCoord chunkCoord, ChunkSize chunkSize, int height = 0)
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

		internal static ChunkSize ClampChunkSize(ChunkSize chunkSize)
		{
			chunkSize.x = Math.Max(MinChunkSize.x, chunkSize.x);
			chunkSize.y = Math.Max(MinChunkSize.y, chunkSize.y);
			return chunkSize;
		}

		/*internal static long GetChunkKey(GridCoord worldCoord, ChunkSize chunkSize)
	{
		var chunkCoord = ToChunkCoord(worldCoord, chunkSize);
		return GetChunkKey(chunkCoord);
	}*/

		internal static ChunkKey GetChunkKey(ChunkCoord chunkCoord) => HashUtility.GetHash(chunkCoord.x, chunkCoord.y);

		internal static GridCoord LayerToGridCoord(LayerCoord layerCoord, ChunkCoord chunkCoord, ChunkSize chunkSize) =>
			new(chunkCoord.x * chunkSize.x + layerCoord.x,
				layerCoord.y,
				chunkCoord.y * chunkSize.y + layerCoord.z);

		/// <summary>
		/// Note: negative grid coordinates result in negative chunk coordinates - but offset by 1. There may
		/// be a generic way to calculate this but the straightforward solution using ternary works just fine.
		/// Examples for ChunkSize(2,2):
		/// Grid(-1,0,-1) => Chunk(-1,-1)
		/// Grid(-2,0,-2) => Chunk(-1,-1)
		/// Grid(-3,0,-3) => Chunk(-2,-2)
		/// Grid(-4,0,-4) => Chunk(-2,-2)
		/// </summary>
		/// <param name="gridCoord"></param>
		/// <param name="chunkSize"></param>
		/// <returns></returns>
		internal static ChunkCoord GridToChunkCoord(GridCoord gridCoord, ChunkSize chunkSize) => new(
			gridCoord.x < 0 ? (Math.Abs(gridCoord.x + 1) / chunkSize.x + 1) * -1 : gridCoord.x / chunkSize.x,
			gridCoord.z < 0 ? (Math.Abs(gridCoord.z + 1) / chunkSize.y + 1) * -1 : gridCoord.z / chunkSize.y);

		internal static LayerCoord GridToLayerCoord(GridCoord gridCoord, ChunkSize chunkSize) => new(
			Math.Abs(gridCoord.x) % chunkSize.x,
			Math.Max(0, gridCoord.y),
			Math.Abs(gridCoord.z) % chunkSize.y);

		[ExcludeFromCodeCoverage] static Tilemap3DUtility() {}
	}
}
