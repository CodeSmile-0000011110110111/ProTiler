// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using System;
using System.Runtime.InteropServices;
using Unity.Serialization.Json;
using UnityEngine;
using LayerSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.Tests.Editor.ProTiler.Data
{
	public class Tile3DLayerTests
	{
		private static void SetAllTilesWithIncrementingIndex(ref Tile3DLayer tiles, int width, int height)
		{
			for (var i = 0; i < width * height; i++)
				tiles[i] = new Tile3D((short)(i + 1));
		}

		private static Tile3DCoord[] CreateTileCoordsWithIncrementingIndex(int width, int height)
		{
			var coordDataCount = width * height;
			var tileCoordDatas = new Tile3DCoord[coordDataCount];
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var coord = new GridCoord(x, 0, y);
					var index = Grid3DUtility.ToIndex2D(x, y, width);
					tileCoordDatas[index] = new Tile3DCoord(coord, new Tile3D((short)(index + 1)));
				}
			}
			return tileCoordDatas;
		}

		private static void AssertThatAllTilesHaveIncrementingIndex(int width, int height, Tile3DLayer tiles)
		{
			for (var i = 0; i < width * height; i++)
				Assert.That(tiles[i].Index, Is.EqualTo((short)(i + 1)));
		}

		private Tile3DLayer CreateLayer(int width, int height) => new(new LayerSize(width, height));

		[Test] public void AssertThatSizeDidNotChangeUnintentionally()
		{
			var sizeInBytes = Marshal.SizeOf(typeof(Tile3DLayer));

			Debug.Log($"Size of Tile3DLayer type: {sizeInBytes} bytes");

			Assert.That(sizeInBytes == 8);
		}

		[Test] public void AssertThatJsonDidNotChangeUnintentionally()
		{
			var tiles = CreateLayer(1, 2);

			var json = JsonSerialization.ToJson(tiles);
			Debug.Log($"ToJson() => {json.Length} bytes:");
			Debug.Log(json);

			Assert.That(json.Length, Is.EqualTo(161));
		}

		[Test] public void AssertThatMinifiedJsonDidNotChangeUnintentionally()
		{
			var tiles = CreateLayer(1, 2);

			var json = JsonSerialization.ToJson(tiles,
				new JsonSerializationParameters { Minified = true, Simplified = true });
			Debug.Log($"ToJson() (minified) => {json.Length} bytes:");
			Debug.Log(json);

			Assert.That(json, Is.EqualTo("{m_Tiles=[{Index=0 Flags=0} {Index=0 Flags=0}]}"));
			Assert.That(json.Length, Is.EqualTo(47));
		}

		[TestCase(-1, 0)] [TestCase(0, -1)]
		public void NegativeSizeThrows(int width, int height) => Assert.Throws<ArgumentException>(() =>
		{
			new Tile3DLayer(new LayerSize(width, height));
		});

		[TestCase(0, 0)]
		public void ZeroSizedLayerIsAllowed(int width, int height)
		{
			var tiles = CreateLayer(width, height);

			Assert.That(tiles.Count, Is.EqualTo(0));
		}

		[TestCase(0, 0)] [TestCase(1, 0)] [TestCase(0, 1)]
		public void ZeroSizedLayerIsConsideredUninitialized(int width, int height)
		{
			var tiles = CreateLayer(width, height);

			Assert.That(tiles.IsInitialized == false);
		}

		[TestCase(1, 1)]
		public void NonZeroSizedLayerIsConsideredInitialized(int width, int height)
		{
			var tiles = CreateLayer(width, height);

			Assert.That(tiles.IsInitialized);
		}

		[TestCase(5, 9)]
		public void LayerIsInitiallyEmpty(int width, int height)
		{
			var tiles = CreateLayer(width, height);

			Assert.That(tiles.Count, Is.EqualTo(0));
		}

		[TestCase(0, 1)] [TestCase(1, 1)] [TestCase(13, 17)]
		public void LayerCapacityMatchesSize(int width, int height)
		{
			var tiles = CreateLayer(width, height);

			Assert.That(tiles.Capacity, Is.EqualTo(width * height));
		}

		[TestCase(0, 0)] [TestCase(2, 2)]
		public void ResizeLayerMatchesCapacity(int width, int height)
		{
			var tiles = CreateLayer(width, height);

			var newSize = 8;
			tiles.Resize(new LayerSize(newSize, newSize));

			Assert.That(tiles.Capacity, Is.EqualTo(newSize * newSize));
		}

		[TestCase(0, 0)] [TestCase(0, 1)] [TestCase(1, 0)]
		public void ResizeLayerWithZeroSizeMakesLayerUninitialized(int width, int height)
		{
			var tiles = CreateLayer(3, 3);

			tiles.Resize(new LayerSize(width, height));

			Assert.That(tiles.IsInitialized == false);
		}

		[TestCase(4, 4)]
		public void ResizeLayerWithNegativeSizeThrows(int width, int height)
		{
			var tiles = CreateLayer(width, height);

			Assert.Throws<ArgumentException>(() => { tiles.Resize(new LayerSize(-1, 0)); });
			Assert.Throws<ArgumentException>(() => { tiles.Resize(new LayerSize(0, -1)); });
		}

		[TestCase(5, 3)]
		public void SetEmptyTileDataDoesNotChangeCount(int width, int height)
		{
			var tiles = CreateLayer(width, height);

			tiles[0] = new Tile3D();

			Assert.That(tiles.Count == 0);
		}

		[TestCase(3, 2)]
		public void SetNonEmptyTileDataIncrementsCount(int width, int height)
		{
			var tiles = CreateLayer(width, height);

			var tileIndex = (short)9;
			tiles[0] = new Tile3D(tileIndex);

			Assert.That(tiles.Count == 1);
			Assert.That(tiles[0].Index == tileIndex);
		}

		[TestCase(3, 3)]
		public void SetNonEmptyTileDataForEntireLayerHasCountEqualCapacity(int width, int height)
		{
			var tiles = CreateLayer(width, height);

			SetAllTilesWithIncrementingIndex(ref tiles, width, height);

			Assert.That(tiles.Count, Is.EqualTo(tiles.Capacity));
		}

		[TestCase(4, 7)]
		public void SetAllTilesReturnTheExpectedTile(int width, int height)
		{
			var tiles = CreateLayer(width, height);

			SetAllTilesWithIncrementingIndex(ref tiles, width, height);

			AssertThatAllTilesHaveIncrementingIndex(width, height, tiles);
		}

		[TestCase(7, 4)]
		public void SetAllTilesWithCoordData(int width, int height)
		{
			var tiles = CreateLayer(width, height);
			var tileCoords = CreateTileCoordsWithIncrementingIndex(width, height);

			tiles.SetTiles(tileCoords, width);

			Assert.That(tiles.Count == width * height);
			AssertThatAllTilesHaveIncrementingIndex(width, height, tiles);
		}
	}
}
