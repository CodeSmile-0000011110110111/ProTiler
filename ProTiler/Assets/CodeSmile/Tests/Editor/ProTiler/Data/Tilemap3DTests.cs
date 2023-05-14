// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
			Assert.That(tilemap.ChunkSize, Is.EqualTo(Tilemap3DUtility.MinChunkSize));
		}

		[TestCaseSource(nameof(IllegalChunkSizes))]
		public void IllegalChunkSizesAreClampedToMinChunkSize(ChunkSize illegalChunkSize)
		{
			var tilemap = CreateTilemap(illegalChunkSize);

			Assert.That(tilemap.ChunkCount, Is.EqualTo(0));
			Assert.That(tilemap.ChunkSize, Is.EqualTo(Tilemap3DUtility.MinChunkSize));
		}

		[Test] public void GetTilesOnEmptyMapReturnsEmptyEnumerable()
		{
			var tilemap = CreateTilemap(new ChunkSize(2, 2));

			var tiles = tilemap.GetTiles(new[] { new GridCoord() });
			Assert.That(tiles != null);
			Assert.That(tiles.Count(), Is.EqualTo(0));
		}

		[TestCase(1, 0, 1, 2, 2)]
		[TestCase(2, 0, 1, 2, 2)]
		[TestCase(1, 0, 2, 2, 2)]
		[TestCase(2, 0, 2, 2, 2)]
		[TestCase(3, 0, 3, 2, 2)]
		[TestCase(4, 0, 4, 4, 4)]
		[TestCase(5, 0, 5, 4, 4)]
		[TestCase(5, 0, 5, 6, 6)]
		[TestCase(23, 0, 26, 6, 5)]
		[TestCase(24, 0, 25, 6, 5)]
		[TestCase(25, 0, 24, 6, 5)]
		[TestCase(23, 2, 26, 6, 5)]
		[TestCase(24, 3, 25, 6, 5)]
		[TestCase(25, 4, 24, 6, 5)]
		public void SetAndGetCornerTileReturnsTile(int width, int height, int length, int chunkX, int chunkY)
		{
			var tilemap = CreateTilemap(new ChunkSize(chunkX, chunkY));

			var coord = new GridCoord(width, height, length);
			var tileCoords = new Tile3DCoord[] { new(coord, new Tile3D(12345)) };
			Debug.Log($"set: {tileCoords[0]}");
			tilemap.SetTiles(tileCoords);

			var gotTileCoords = tilemap.GetTiles(new[] { coord }) as IList<Tile3DCoord>;

			Debug.Log($"got: {gotTileCoords[0]}");
			Assert.That(gotTileCoords[0].Coord, Is.EqualTo(tileCoords[0].Coord));
			Assert.That(gotTileCoords[0].Tile, Is.EqualTo(tileCoords[0].Tile));
		}

		[TestCase(1, 1, 1, 2, 2)]
		[TestCase(2, 1, 1, 2, 2)]
		[TestCase(1, 1, 2, 2, 2)]
		[TestCase(1, 2, 1, 2, 2)]
		[TestCase(2, 2, 2, 2, 2)]
		[TestCase(3, 3, 3, 2, 2)]
		[TestCase(4, 2, 4, 4, 4)]
		[TestCase(5, 2, 5, 4, 4)]
		[TestCase(16, 4, 17, 6, 5)]
		public void SetAndGetTilesAcrossLayersAreEqual(int width, int height, int length, int chunkX, int chunkY)
		{
			var tilemap = CreateTilemap(new ChunkSize(chunkX, chunkY));

			var tileCoords = Tile3DTestUtility.CreateTileCoordsWithIncrementingIndexAcrossLayers(width, height, length);
			tilemap.SetTiles(tileCoords);

			var coords = tileCoords.ToCoordArray();
			Assert.That(coords.Length, Is.EqualTo(tileCoords.Length));

			var gotTileCoords = tilemap.GetTiles(coords) as IList<Tile3DCoord>;

			Assert.That(gotTileCoords.Count, Is.EqualTo(tileCoords.Length));
			for (var i = 0; i < gotTileCoords.Count; i++)
				// order of tiles likely differs
				Assert.That(tileCoords.Contains(gotTileCoords[i]));
		}

		private Tilemap3D CreateTilemap(ChunkSize chunkSize) => new(chunkSize);
	}
}
