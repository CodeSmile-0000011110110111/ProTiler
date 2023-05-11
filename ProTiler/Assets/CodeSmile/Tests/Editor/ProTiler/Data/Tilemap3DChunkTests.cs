// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using System.Runtime.InteropServices;
using Unity.Serialization.Json;
using UnityEngine;
using ChunkSize = UnityEngine.Vector2Int;

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

		[TestCase(2, 0, 2)]
		[TestCase(4, 1, 5)]
		[TestCase(9, 7, 7)]
		[TestCase(3, 51, 3)]
		public void SetOneTileCreatesLayersAccordingToHeight(int width, int height, int length)
		{
			var chunk = CreateChunk(width, length);
			var tileCoords = Tile3DTestUtility.CreateOneTileCoord(width, height, length);

			chunk.SetTiles(tileCoords);

			Assert.That(chunk.LayerCount, Is.EqualTo(height + 1));
		}

		[TestCase(3, 0, 7)]
		[TestCase(4, 1, 9)]
		[TestCase(6, 10, 5)]
		public void SetOneTileReturnsThatTile(int width, int height, int length)
		{
			var chunk = CreateChunk(width, length);
			var tileCoords = Tile3DTestUtility.CreateOneTileCoord(width, height, length);

			chunk.SetTiles(tileCoords);

			var coord = tileCoords[0].Coord;
			var tileIndex = Grid3DUtility.ToIndex2D(coord.x, coord.z, width);
			Assert.That(chunk[height][tileIndex], Is.EqualTo(tileCoords[0].Tile));
		}

		[TestCase(4, 7)]
		public void SetAllTiles(int width, int height)
		{
			var chunk = CreateChunk(width, height);
			var tileCoords = Tile3DTestUtility.CreateTileCoordsWithIncrementingIndex(width, height);

			chunk.SetTiles(tileCoords);

			Tile3DTestUtility.AssertThatAllTilesHaveIncrementingIndex(width, height, chunk[0]);

			Assert.That(chunk.LayerCount, Is.EqualTo(1));
		}
	}
}
