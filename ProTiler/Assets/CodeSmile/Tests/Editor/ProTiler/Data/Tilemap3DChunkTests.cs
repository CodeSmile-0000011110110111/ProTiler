// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Serialization.Json;
using UnityEngine;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.Tests.Editor.ProTiler.Data
{
	public class Tilemap3DChunkTests
	{
		private static Tilemap3DChunk CreateChunk(int width, int length) => new(new ChunkSize(width, length));

		[Test] public void AssertThatSizeDidNotChangeUnintentionally()
		{
			var sizeInBytes = Marshal.SizeOf(typeof(Tilemap3DChunk));
			Debug.Log($"Size of {nameof(Tilemap3DChunk)} type: {sizeInBytes} bytes");

			Assert.That(sizeInBytes == 32);
		}

		[Test] public void AssertThatJsonDidNotChangeUnintentionally()
		{
			var chunk = CreateChunk(3, 2);

			var json = JsonSerialization.ToJson(chunk);
			Debug.Log($"ToJson() => {json.Length} bytes:");
			Debug.Log(json);

			Assert.That(json.Length, Is.EqualTo(76));
		}

		[Test] public void AssertThatMinifiedJsonDidNotChangeUnintentionally()
		{
			var chunk = CreateChunk(3, 2);

			var json = JsonSerialization.ToJson(chunk,
				new JsonSerializationParameters { Minified = true, Simplified = true });
			Debug.Log($"ToJson() (minified) => {json.Length} bytes:");
			Debug.Log(json);

			Assert.That(json, Is.EqualTo("{m_Size={x=3 y=2} m_Layers={}}"));
			Assert.That(json.Length, Is.EqualTo(30));
		}

		[Test] public void NewTilemap3DChunkTileCountIsZero()
		{
			var chunk = CreateChunk(5, 5);

			Assert.That(chunk.TileCount, Is.EqualTo(0));
		}

		[Test] public void NewTilemap3DChunkLayerCountIsZero()
		{
			var chunk = CreateChunk(5, 5);

			Assert.That(chunk.LayerCount, Is.EqualTo(0));
		}

		[TestCase(3, 2)]
		[TestCase(4, 5)]
		public void NewTilemap3DChunkSizeMatches(int width, int length)
		{
			var chunk = CreateChunk(width, length);

			Assert.That(chunk.Size, Is.EqualTo(new ChunkSize(width, length)));
		}

		[TestCase(2, 0, 2)]
		[TestCase(4, 1, 5)]
		[TestCase(9, 7, 7)]
		[TestCase(3, 51, 3)]
		public void SetOneTileCreatesLayersAccordingToHeight(int width, int height, int length)
		{
			var chunk = CreateChunk(width, length);
			var tileCoords = Tile3DTestUtility.CreateOneTileCoord(width, height, length);

			chunk.SetLayerTiles(tileCoords);

			Assert.That(chunk.LayerCount, Is.EqualTo(height + 1));
		}

		[TestCase(3, 0, 7)]
		[TestCase(4, 1, 9)]
		[TestCase(6, 10, 5)]
		public void SetOneTileReturnsThatTile(int width, int height, int length)
		{
			var chunk = CreateChunk(width, length);
			var tileCoords = Tile3DTestUtility.CreateOneTileCoord(width, height, length);

			chunk.SetLayerTiles(tileCoords);

			var coord = tileCoords[0].Coord;
			var tileIndex = Grid3DUtility.ToIndex2D(coord.x, coord.z, width);
			Assert.That(chunk[height][tileIndex], Is.EqualTo(tileCoords[0].Tile));
		}

		[TestCase(4, 7)]
		public void SetAllTilesOnOneLayerReturnsAllTiles(int width, int length)
		{
			var chunk = CreateChunk(width, length);
			var tileCoords = Tile3DTestUtility.CreateTileCoordsWithIncrementingIndex(width, length);

			chunk.SetLayerTiles(tileCoords);

			Assert.That(chunk.LayerCount, Is.EqualTo(1));
			Assert.That(chunk.TileCount, Is.EqualTo(width * length));
			Tile3DTestUtility.AssertThatAllTilesHaveIncrementingIndex(width, length, chunk[0]);
		}

		[TestCase(1, 3, 1)]
		[TestCase(1, 12, 1)]
		[TestCase(3, 9, 3)]
		[TestCase(4, 11, 4)]
		[TestCase(5, 11, 5)]
		public void SetAllTilesAcrossLayersReturnsAllTiles(int width, int height, int length)
		{
			var chunk = CreateChunk(width, length);
			var tileCoords = Tile3DTestUtility.CreateTileCoordsWithIncrementingIndexAcrossLayers(width, height, length);

			chunk.SetLayerTiles(tileCoords);

			Assert.That(chunk.LayerCount, Is.EqualTo(height));
			Assert.That(chunk.TileCount, Is.EqualTo(width * length * height));
			for (var y = 0; y < height; y++)
				Tile3DTestUtility.AssertThatAllTilesHaveIncrementingIndex(width, length, chunk[y], y);
		}

		[TestCase(3, 4, 5)]
		public void GetTileFromEmptyChunkShouldReturnEmptyTile(int x, int y, int z)
		{
			var chunk = CreateChunk(x, z);

			var coords = Tile3DTestUtility.CreateOneTileCoord(x, y, z).ToCoordArray();
			var gotTileCoords = chunk.GetLayerTiles(coords) as IList<Tile3DCoord>;

			Assert.That(gotTileCoords.Count, Is.EqualTo(coords.Length));
			Assert.That(gotTileCoords[0].Tile.IsEmpty);
		}

		[TestCase(3, 4, 5)]
		public void SetAndGetTilesAcrossLayersAreEqual(int width, int height, int length)
		{
			var chunk = CreateChunk(width, length);
			var tileCoords = Tile3DTestUtility.CreateTileCoordsWithIncrementingIndexAcrossLayers(width, height, length);
			chunk.SetLayerTiles(tileCoords);

			var gotTileCoords = chunk.GetLayerTiles(tileCoords.ToCoordArray()) as IList<Tile3DCoord>;

			Assert.That(gotTileCoords.Count, Is.EqualTo(tileCoords.Length));
			for (var i = 0; i < gotTileCoords.Count; i++)
			{
				Assert.That(gotTileCoords[i].Coord, Is.EqualTo(tileCoords[i].Coord));
				Assert.That(gotTileCoords[i].Tile, Is.EqualTo(tileCoords[i].Tile));
			}
		}
	}
}
