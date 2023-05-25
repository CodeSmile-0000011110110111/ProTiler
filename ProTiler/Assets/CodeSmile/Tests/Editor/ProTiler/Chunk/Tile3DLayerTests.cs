// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Model;
using CodeSmile.Tests.Editor.ProTiler.Utility;
using NUnit.Framework;
using System;
using System.Runtime.InteropServices;
using Unity.Serialization.Json;
using UnityEngine;
using LayerSize = UnityEngine.Vector2Int;

namespace CodeSmile.Tests.Editor.ProTiler.Chunk
{
	public class Tile3DLayerTests
	{
		private static Tile3DLayer CreateLayer(int width, int height) => new(new LayerSize(width, height));

		[Test] public void SizeDidNotChangeUnintentionally()
		{
			var sizeInBytes = Marshal.SizeOf(typeof(Tile3DLayer));
			Debug.Log($"Size of {nameof(Tile3DLayer)} type: {sizeInBytes} bytes");

			Assert.That(sizeInBytes == 8);
		}

		[Test] public void JsonDidNotChangeUnintentionally()
		{
			var layer = CreateLayer(1, 2);

			var json = JsonSerialization.ToJson(layer);
			Debug.Log($"ToJson() => {json.Length} bytes:");
			Debug.Log(json);

			Assert.That(json.Length, Is.EqualTo(161));
		}

		[Test] public void MinifiedJsonDidNotChangeUnintentionally()
		{
			var layer = CreateLayer(1, 2);

			var json = JsonSerialization.ToJson(layer,
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
			var layer = CreateLayer(width, height);

			Assert.That(layer.TileCount, Is.EqualTo(0));
		}

		[TestCase(0, 0)] [TestCase(1, 0)] [TestCase(0, 1)]
		public void ZeroSizedLayerIsConsideredUninitialized(int width, int height)
		{
			var layer = CreateLayer(width, height);

			Assert.That(layer.IsInitialized == false);
		}

		[TestCase(1, 1)]
		public void NonZeroSizedLayerIsConsideredInitialized(int width, int height)
		{
			var layer = CreateLayer(width, height);

			Assert.That(layer.IsInitialized);
		}

		[TestCase(5, 9)]
		public void LayerIsInitiallyEmpty(int width, int height)
		{
			var layer = CreateLayer(width, height);

			Assert.That(layer.TileCount, Is.EqualTo(0));
		}

		[TestCase(0, 1)] [TestCase(1, 1)] [TestCase(13, 17)]
		public void LayerCapacityMatchesSize(int width, int height)
		{
			var layer = CreateLayer(width, height);

			Assert.That(layer.Capacity, Is.EqualTo(width * height));
		}

		[TestCase(0, 0)] [TestCase(2, 2)]
		public void ResizeLayerMatchesCapacity(int width, int height)
		{
			var layer = CreateLayer(width, height);

			var newSize = 8;
			layer.Resize(new LayerSize(newSize, newSize));

			Assert.That(layer.Capacity, Is.EqualTo(newSize * newSize));
		}

		[TestCase(0, 0)] [TestCase(0, 1)] [TestCase(1, 0)]
		public void ResizeLayerWithZeroSizeMakesLayerUninitialized(int width, int height)
		{
			var layer = CreateLayer(3, 3);

			layer.Resize(new LayerSize(width, height));

			Assert.That(layer.IsInitialized == false);
		}

		[TestCase(4, 4)]
		public void ResizeLayerWithNegativeSizeThrows(int width, int height)
		{
			var layer = CreateLayer(width, height);

			Assert.Throws<ArgumentException>(() => { layer.Resize(new LayerSize(-1, 0)); });
			Assert.Throws<ArgumentException>(() => { layer.Resize(new LayerSize(0, -1)); });
		}

		[TestCase(5, 3)]
		public void SetEmptyTileDataDoesNotChangeCount(int width, int height)
		{
			var layer = CreateLayer(width, height);

			layer[0] = new Tile3D();

			Assert.That(layer.TileCount == 0);
		}

		[TestCase(3, 2)]
		public void SetNonEmptyTileDataIncrementsCount(int width, int height)
		{
			var layer = CreateLayer(width, height);

			var tileIndex = 9;
			layer[0] = new Tile3D(tileIndex);

			Assert.That(layer.TileCount == 1);
			Assert.That(layer[0].Index == tileIndex);
		}

		[TestCase(3, 3)]
		public void SetNonEmptyTileDataForEntireLayerHasCountEqualCapacity(int width, int height)
		{
			var layer = CreateLayer(width, height);

			Tile3DTestUtility.SetAllTilesWithIncrementingIndex(ref layer, width, height);

			Assert.That(layer.TileCount, Is.EqualTo(layer.Capacity));
		}

		[TestCase(4, 7)]
		public void SetAllTilesReturnTheExpectedTile(int width, int height)
		{
			var layer = CreateLayer(width, height);

			Tile3DTestUtility.SetAllTilesWithIncrementingIndex(ref layer, width, height);

			Tile3DTestUtility.AssertThatAllTilesHaveIncrementingIndex(width, height, layer);
		}

		[TestCase(7, 4)]
		public void SetAllTilesWithCoordData(int width, int height)
		{
			var layer = CreateLayer(width, height);
			var tileCoords = Tile3DTestUtility.CreateTileCoordsWithIncrementingIndex(width, height);

			layer.SetTiles(tileCoords, width);

			Assert.That(layer.TileCount == width * height);
			Tile3DTestUtility.AssertThatAllTilesHaveIncrementingIndex(width, height, layer);
		}
	}
}
