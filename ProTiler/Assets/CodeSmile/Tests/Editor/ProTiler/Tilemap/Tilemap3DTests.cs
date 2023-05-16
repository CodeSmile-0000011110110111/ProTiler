// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Tile;
using CodeSmile.ProTiler.Tilemap;
using CodeSmile.ProTiler.Utility;
using CodeSmile.Tests.Editor.ProTiler.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Serialization.Json;
using UnityEngine;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.Tests.Editor.ProTiler.Tilemap
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

		private static Tilemap3D CreateTilemap(ChunkSize chunkSize) => new(chunkSize);

		[Test] public void EmptyTilemapMinifiedJsonDidNotChangeUnintentionally()
		{
			var tilemap = new Tilemap3D();

			var json = JsonSerialization.ToJson(tilemap,
				new JsonSerializationParameters { Simplified = true, Minified = true });
			Debug.Log($"ToJson() (minified) => {json.Length} bytes:");
			Debug.Log(json);

			Assert.That(json, Is.EqualTo("{m_ChunkSize={x=2 y=2} m_Chunks={m_Chunks=[]}}"));
			Assert.That(json.Length, Is.EqualTo(46));
		}

		[Test] public void NonEmptyTilemapMinifiedJsonDidNotChangeUnintentionally()
		{
			var tilemap = new Tilemap3D();
			tilemap.SetTiles(new Tile3DCoord[] { new(new GridCoord(1, 1, 1), new Tile3D(123)) });

			var json = JsonSerialization.ToJson(tilemap,
				new JsonSerializationParameters { Simplified = true, Minified = true });
			Debug.Log($"ToJson() (minified) => {json.Length} bytes:");
			Debug.Log(json);

			Assert.That(json, Is.EqualTo("{m_ChunkSize={x=2 y=2} m_Chunks={m_Chunks=[{Key=805654745852585 " +
			                             "Value={m_Size={x=2 y=2} m_Layers={m_Layers=[{m_Tiles=[{Index=0 Flags=0} " +
			                             "{Index=0 Flags=0} {Index=0 Flags=0} {Index=0 Flags=0}]} " +
			                             "{m_Tiles=[{Index=0 Flags=0} {Index=0 Flags=0} {Index=0 Flags=0} " +
			                             "{Index=123 Flags=1}]}]}}}]}}"));
			Assert.That(json.Length, Is.EqualTo(284));
		}

		[Test] public void NonEmptyTilemapIsDeSerializedCorrectly()
		{
			var tilemap = new Tilemap3D();
			var coord = new GridCoord(1, 1, 1);
			var tileIndex = short.MaxValue;
			tilemap.SetTiles(new Tile3DCoord[] { new(coord, new Tile3D(tileIndex)) });

			var json = Tilemap3DSerializer.ToJson(tilemap);
			Debug.Log(json);
			var deserializedTilemap = Tilemap3DSerializer.FromJson(json);
			var tiles = deserializedTilemap.GetTiles(new[] { coord });

			Assert.That(deserializedTilemap.ChunkSize, Is.EqualTo(tilemap.ChunkSize));
			Assert.That(deserializedTilemap.ChunkCount, Is.EqualTo(tilemap.ChunkCount));
			Assert.That(deserializedTilemap.TileCount, Is.EqualTo(tilemap.TileCount));
			Assert.That(tiles.First().Coord, Is.EqualTo(coord));
			Assert.That(tiles.First().Tile.Index, Is.EqualTo(tileIndex));
		}

		[Test] public void DefaultCtorUsesMinChunkSize()
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

		[TestCase(0, 0, 0, 1)]
		[TestCase(0, 0, 1, 2)]
		[TestCase(1, 1, 11, 12)]
		[TestCase(2, 2, 12, 13)]
		[TestCase(-3, -3, 13, 14)]
		public void SetTileReturnsCorrectLayerCount(int chunkX, int chunkY, int height, int layerCount)
		{
			var chunkSize = new ChunkSize(3, 4);
			var tilemap = CreateTilemap(chunkSize);

			var coord = new GridCoord(chunkSize.x * chunkX, height, chunkSize.y * chunkY);
			var tileCoords = new Tile3DCoord[] { new(coord, new Tile3D(12345)) };
			tilemap.SetTiles(tileCoords);

			var chunkCoord = Tilemap3DUtility.GridToChunkCoord(coord, chunkSize);
			Assert.That(tilemap.GetLayerCount(chunkCoord), Is.EqualTo(layerCount));
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
	}
}
