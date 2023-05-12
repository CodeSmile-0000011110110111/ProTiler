// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.Tests.Editor.ProTiler.Data
{
	public class Tilemap3DTests
	{
		private static readonly object[] IllegalChunkSizes =
		{
			new object[] { new ChunkSize(1, 0) },
			new object[] { new ChunkSize(0, 1) },
			new object[] { new ChunkSize(2, 1) },
			new object[] { new ChunkSize(-1, 1) },
			new object[] { new ChunkSize(1, -1) },
		};


		private static Tilemap3DChunk CreateChunk(int width, int length) => new(new ChunkSize(width, length));

		[Test]
		public void EnsureDefaultCtorUsesMinChunkSize()
		{
			var tilemap = new Tilemap3D();

			Assert.That(tilemap.ChunkCount, Is.EqualTo(0));
			Assert.That(tilemap.ChunkSize, Is.EqualTo(Tilemap3D.MinChunkSize));
		}

		[TestCaseSource(nameof(IllegalChunkSizes))]
		public void IllegalChunkSizesAreClampedToMinChunkSize(ChunkSize illegalChunkSize)
		{
			var tilemap = CreateTilemap(illegalChunkSize);

			Assert.That(tilemap.ChunkCount, Is.EqualTo(0));
			Assert.That(tilemap.ChunkSize, Is.EqualTo(Tilemap3D.MinChunkSize));
		}

		[TestCase(6, 3, 11, 3)]
		public void SetAndGetTilesAcrossLayersAreEqual(int width, int height, int length, int chunkDivider)
		{
			var chunkSize = new ChunkSize(width / chunkDivider, length / chunkDivider);
			var tilemap = CreateTilemap(chunkSize);

			var tileCoords = Tile3DTestUtility.CreateTileCoordsWithIncrementingIndexAcrossLayers(width, height, length);
			tilemap.SetTiles(tileCoords);

			var gotTileCoords = tilemap.GetTiles(tileCoords.ToCoordArray());

			Assert.That(gotTileCoords.Length, Is.EqualTo(tileCoords.Length));
			for (var i = 0; i < gotTileCoords.Length; i++)
			{
				Assert.That(gotTileCoords[i].Coord, Is.EqualTo(tileCoords[i].Coord));
				Assert.That(gotTileCoords[i].Tile, Is.EqualTo(tileCoords[i].Tile));
			}
		}

		private Tilemap3D CreateTilemap(ChunkSize chunkSize) => new(chunkSize);
	}
}
