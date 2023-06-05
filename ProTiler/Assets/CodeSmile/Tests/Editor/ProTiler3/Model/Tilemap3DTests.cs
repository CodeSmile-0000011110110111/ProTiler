// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Runtime.Model;
using CodeSmile.ProTiler3.Runtime.Serialization;
using CodeSmile.Tests.Editor.ProTiler3.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Serialization.Json;
using UnityEngine;
using ChunkSize = Unity.Mathematics.int2;
using GridCoord = Unity.Mathematics.int3;
using Object = System.Object;

namespace CodeSmile.Tests.Editor.ProTiler3.Model
{
	public class Tilemap3DTests
	{
		private static readonly Object[] IllegalChunkSizes =
		{
			new Object[] { new ChunkSize(1, 0) },
			new Object[] { new ChunkSize(0, 1) },
			new Object[] { new ChunkSize(2, 1) },
			new Object[] { new ChunkSize(-1, 1) },
			new Object[] { new ChunkSize(1, -1) },
		};

		private static Tilemap3D CreateTilemap(ChunkSize chunkSize) => new(chunkSize);

		[Test] public void EmptyTilemapMinifiedJsonDidNotChangeUnintentionally()
		{
			var tilemap = new Tilemap3D(Tilemap3D.MinChunkSize);

			var json = JsonSerialization.ToJson(tilemap,
				new JsonSerializationParameters { Simplified = true, Minified = true });
			Debug.Log($"ToJson() (minified) => {json.Length} bytes:");
			Debug.Log(json);

			Assert.That(json, Is.EqualTo("{m_ChunkSize={x=2 y=2} m_Chunks={m_Chunks=[]}}"));
			Assert.That(json.Length, Is.EqualTo(46));
		}

		[Test] public void NonEmptyTilemapMinifiedJsonDidNotChangeUnintentionally()
		{
			var tilemap = new Tilemap3D(Tilemap3D.MinChunkSize);
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
			var tileIndex = Int16.MaxValue;
			tilemap.SetTiles(new Tile3DCoord[] { new(coord, new Tile3D(tileIndex)) });

			var json = UnitySerializer.ToJson(tilemap);
			Debug.Log(json);
			var deserializedTilemap = UnitySerializer.FromJson<Tilemap3D>(json);
			var tiles = deserializedTilemap.GetExistingTiles(new[] { coord });

			Assert.That(deserializedTilemap.ChunkSize, Is.EqualTo(tilemap.ChunkSize));
			Assert.That(deserializedTilemap.ChunkCount, Is.EqualTo(tilemap.ChunkCount));
			Assert.That(deserializedTilemap.TileCount, Is.EqualTo(tilemap.TileCount));
			Assert.That(tiles.First().Coord, Is.EqualTo(coord));
			Assert.That(tiles.First().Tile.Index, Is.EqualTo(tileIndex));
		}

		[Test] public void DefaultCtorUsesDefaultChunkSize()
		{
			var tilemap = new Tilemap3D();

			Assert.That(tilemap.ChunkCount, Is.Zero);
			Assert.That(tilemap.ChunkSize, Is.EqualTo(Tilemap3D.DefaultChunkSize));
		}

		[TestCaseSource(nameof(IllegalChunkSizes))]
		public void IllegalChunkSizesAreClampedToMinChunkSize(ChunkSize illegalChunkSize)
		{
			var tilemap = CreateTilemap(illegalChunkSize);

			Assert.That(tilemap.ChunkCount, Is.Zero);
			Assert.That(tilemap.ChunkSize, Is.EqualTo(Tilemap3D.MinChunkSize));
		}

		[Test] public void GetExistingTilesOnEmptyMapReturnsEmptyEnumerable()
		{
			var tilemap = CreateTilemap(new ChunkSize(2, 2));

			var tiles = tilemap.GetExistingTiles(new[] { new GridCoord() });

			Assert.That(tiles != null);
			Assert.That(tiles.Count(), Is.Zero);
		}

		[Test] public void GetTilesWithEmptyListReturnsEmptyDict()
		{
			var tilemap = CreateTilemap(new ChunkSize(2, 2));

			var tiles = tilemap.GetTiles(new GridCoord[0]);

			Assert.That(tiles != null);
			Assert.That(tiles.Count, Is.Zero);
		}

		[Test] public void GetTilesOnEmptyMapReturnsEmptyTiles()
		{
			var tilemap = CreateTilemap(new ChunkSize(2, 2));

			var tiles = tilemap.GetTiles(new[] { new GridCoord() });

			Assert.That(tiles.Count, Is.EqualTo(1));
		}

		[Test] public void GetTilesOnNonEmptyMapFillsInEmptyTiles()
		{
			var tilemap = CreateTilemap(new ChunkSize(2, 2));
			var zeroCoord = new Tile3DCoord(GridCoord.zero, new Tile3D(1));
			var oneCoord = new Tile3DCoord(new GridCoord(1, 1, 1), new Tile3D(2));
			var twoCoord = new Tile3DCoord(new GridCoord(2, 2, 2), new Tile3D(22));
			var existingCoords = new[] { zeroCoord, oneCoord, twoCoord };
			tilemap.SetTiles(existingCoords);

			var coords = new List<GridCoord>();
			for (var x = 0; x < 3; x++)
				for (var y = 0; y < 3; y++)
					for (var z = 0; z < 3; z++)
						coords.Add(new GridCoord(x, y, z));

			var tiles = tilemap.GetTiles(coords);

			Assert.That(tiles.Count, Is.EqualTo(coords.Count));
			Assert.That(tiles[zeroCoord.Coord], Is.EqualTo(zeroCoord));
			Assert.That(tiles[oneCoord.Coord], Is.EqualTo(oneCoord));
			Assert.That(tiles[twoCoord.Coord], Is.EqualTo(twoCoord));
			Assert.That(tiles[new GridCoord(0, 1, 2)], Is.EqualTo(new Tile3DCoord(new GridCoord(0, 1, 2))));
			Assert.That(tiles[new GridCoord(1, 2, 0)], Is.EqualTo(new Tile3DCoord(new GridCoord(1, 2, 0))));
			Assert.That(tiles[new GridCoord(2, 0, 1)], Is.EqualTo(new Tile3DCoord(new GridCoord(2, 0, 1))));
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
		public void SetAndGetCornerTileReturnsTile(Int32 width, Int32 height, Int32 length, Int32 chunkX, Int32 chunkY)
		{
			var tilemap = CreateTilemap(new ChunkSize(chunkX, chunkY));

			var coord = new GridCoord(width, height, length);
			var tileCoords = new Tile3DCoord[] { new(coord, new Tile3D(12345)) };
			Debug.Log($"set: {tileCoords[0]}");
			tilemap.SetTiles(tileCoords);

			var gotTileCoords = tilemap.GetExistingTiles(new[] { coord }) as IList<Tile3DCoord>;

			Debug.Log($"got: {gotTileCoords[0]}");
			Assert.That(gotTileCoords[0].Coord, Is.EqualTo(tileCoords[0].Coord));
			Assert.That(gotTileCoords[0].Tile, Is.EqualTo(tileCoords[0].Tile));
		}

		[TestCase(0, 0, 0, 1)]
		[TestCase(0, 0, 1, 2)]
		[TestCase(1, 1, 11, 12)]
		[TestCase(2, 2, 12, 13)]
		[TestCase(-3, -3, 13, 14)]
		public void SetTileReturnsCorrectLayerCount(Int32 chunkX, Int32 chunkY, Int32 height, Int32 layerCount)
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
		public void SetAndGetTilesAcrossLayersAreEqual(Int32 width, Int32 height, Int32 length, Int32 chunkX,
			Int32 chunkY)
		{
			var tilemap = CreateTilemap(new ChunkSize(chunkX, chunkY));

			var tileCoords = Tile3DTestUtility.CreateTileCoordsWithIncrementingIndexAcrossLayers(width, height, length);
			tilemap.SetTiles(tileCoords);

			var coords = tileCoords.ToCoordArray();
			Assert.That(coords.Length, Is.EqualTo(tileCoords.Length));

			var gotTileCoords = tilemap.GetExistingTiles(coords) as IList<Tile3DCoord>;

			Assert.That(gotTileCoords.Count, Is.EqualTo(tileCoords.Length));
			for (var i = 0; i < gotTileCoords.Count; i++)
				// order of tiles likely differs
				Assert.That(tileCoords.Contains(gotTileCoords[i]));
		}
	}
}
