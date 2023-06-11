// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Runtime.CodeDesign.Model;
using CodeSmile.ProTiler.Runtime.CodeDesign.Serialization.Unity.Serialization;
using ChunkCoord = Unity.Mathematics.int2;
using ChunkSize = Unity.Mathematics.int3;
using CellSize = Unity.Mathematics.float3;
using CellGap = Unity.Mathematics.float3;
using GridCoord = Unity.Mathematics.int3;
using WorldPos = Unity.Mathematics.float3;

namespace CodeSmile.ProTiler.Runtime.CodeDesign.UserCode
{
	public class MyTilemap : TilemapBase
	{
		public MyTilemap()
		{
			var chunkStream = new TilemapChunkBufferedFileStream<ChunkDataLinear<TileData>, TileData>();
			var linearMapData = new TilemapDataLinear<TileData>(chunkStream);
			linearMaps.Add(linearMapData);

			linearMapData.SerializeChunk(0);
			linearMapData.UnloadChunk(0);

			var sparseMapData = new TilemapDataSparse<MySparseBits>();
			sparseMaps.Add(sparseMapData);
		}

		public TileData GetTileData() => default; // TODO ...
		public MySparseBits GetBits(GridCoord coord) => default; // TODO ...
	}
}
