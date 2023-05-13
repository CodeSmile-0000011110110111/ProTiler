// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;
using LayerCoord = UnityEngine.Vector3Int;

internal static class Tilemap3DUtility
{
	/// <summary>
	///     This is a required technical limitation to have at least 2x2 tiles per chunk.
	///     The hashes of chunks with size of less than 2x2 would not be unique.
	/// </summary>
	internal static readonly ChunkSize MinChunkSize = new(2, 2);

	internal static ChunkSize ClampChunkSize(ChunkSize chunkSize)
	{
		chunkSize.x = Mathf.Max(MinChunkSize.x, chunkSize.x);
		chunkSize.y = Mathf.Max(MinChunkSize.y, chunkSize.y);
		return chunkSize;
	}

	/*internal static long GetChunkKey(GridCoord worldCoord, ChunkSize chunkSize)
	{
		var chunkCoord = ToChunkCoord(worldCoord, chunkSize);
		return GetChunkKey(chunkCoord);
	}*/

	internal static long GetChunkKey(ChunkCoord chunkCoord) => HashUtility.GetHash(chunkCoord.x, chunkCoord.y);

	internal static GridCoord LayerToGridCoord(LayerCoord layerCoord, ChunkCoord chunkCoord, ChunkSize chunkSize) =>
		new(chunkCoord.x * chunkSize.x + layerCoord.x, layerCoord.y, chunkCoord.y * chunkSize.y + layerCoord.z);

	internal static ChunkCoord GridToChunkCoord(GridCoord gridCoord, ChunkSize chunkSize) =>
		new(gridCoord.x / chunkSize.x, gridCoord.z / chunkSize.y);

	internal static LayerCoord GridToLayerCoord(GridCoord gridCoord, ChunkSize chunkSize) =>
		new(gridCoord.x % chunkSize.x, gridCoord.y, gridCoord.z % chunkSize.y);

	[ExcludeFromCodeCoverage] static Tilemap3DUtility() {}
}
