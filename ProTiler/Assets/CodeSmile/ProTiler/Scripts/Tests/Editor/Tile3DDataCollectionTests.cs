// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Collections;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Tile3DDataCollectionTests
	{
		[Test]
		public void CreateCollectionTests()
		{
			var width = 10;
			var height = 20;
			var tiles = new Tile3DDataCollection(new Vector2Int(width, height));
			Assert.AreEqual(width, tiles.Width);
			Assert.AreEqual(height, tiles.Height);
			Assert.AreEqual(width * height, tiles.Capacity);
			Assert.AreEqual(0, tiles.Count);
		}

		[Test]
		public void SetTileTests()
		{
			var width = 3;
			var height = 7;
			var tiles = new Tile3DDataCollection(new Vector2Int(width, height));
			tiles[0] = new Tile3DData();
			Assert.AreEqual(0, tiles.Count);

			var tile = tiles[0];
			tile.TileIndex = 1;
			tiles[0] = tile;
			Assert.AreEqual(1, tiles.Count);

			tiles[1] = tile;
			Assert.AreEqual(2, tiles.Count);

			tiles[width * height - 1] = tile;
			Assert.AreEqual(3, tiles.Count);
		}
	}
}
