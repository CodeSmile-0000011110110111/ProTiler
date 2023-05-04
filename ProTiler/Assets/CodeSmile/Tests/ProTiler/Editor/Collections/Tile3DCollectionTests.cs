// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Collections;
using CodeSmile.ProTiler.Data;
using CodeSmile.ProTiler.Utilities;
using NUnit.Framework;
using System;
using UnityEngine;

namespace CodeSmile.Tests.ProTiler.Editor.Collections
{
	public class Tile3DCollectionTests
	{
		[Test] public void CreateCollection()
		{
			var width = 10;
			var height = 20;

			var tiles = new Tile3DCollection(new Vector2Int(width, height));

			Assert.That(tiles.Width == width);
			Assert.That(tiles.Height == height);
			Assert.That(tiles.Capacity == width * height);
			Assert.That(tiles.Count == 0);
		}

		[Test]
		public void SetAndGetEmptyTileData()
		{
			var width = 3;
			var height = 7;
			var tiles = new Tile3DCollection(new Vector2Int(width, height));

			tiles[0] = new Tile3D();

			Assert.That(tiles.Count == 0);
		}

		[Test]
		public void SetAndGetTileData()
		{
			var width = 3;
			var height = 7;
			var tiles = new Tile3DCollection(new Vector2Int(width, height));
			var tileIndex = 9;

			tiles[0] = Tile3D.New(tileIndex);

			Assert.That(tiles.Count == 1);
			Assert.That(tiles[0].Index == tileIndex);
		}

		[Test]
		public void SetAndGetTwoTileDatas()
		{
			var width = 3;
			var height = 7;
			var tiles = new Tile3DCollection(new Vector2Int(width, height));
			var tileIndex = 11;

			tiles[0] = Tile3D.New(tileIndex);
			tiles[1] = tiles[0];
			Assert.That(tiles.Count == 2);
			Assert.That(tiles[1].Index == tileIndex);
		}

		[Test]
		public void SetAndGetTileDataLastIndex()
		{
			var width = 3;
			var height = 7;
			var tiles = new Tile3DCollection(new Vector2Int(width, height));
			var tileIndex = 13;
			var lastIndex = width * height - 1;

			tiles[lastIndex] = Tile3D.New(tileIndex);

			Assert.That(tiles.Count == 1);
			Assert.That(tiles[lastIndex].Index == tileIndex);
		}

		[Test]
		public void SetAndGetTileByCoord()
		{
			var width = 5;
			var height = 9;
			var tiles = new Tile3DCollection(new Vector2Int(width, height));
			var tileIndex = 17;
			var tileData = Tile3D.New(tileIndex);

			var coordX = width - 1;
			var coordY = height - 1;
			tiles[coordX, coordY] = tileData;

			Assert.That(tiles[coordX, coordY].Index == tileIndex);
		}

		[Test]
		public void SetTileByCoordOutOfBoundsThrows()
		{
			var width = 5;
			var height = 9;
			var tiles = new Tile3DCollection(new Vector2Int(width, height));
			var tileIndex = 17;
			var tileData = Tile3D.New(tileIndex);

			Assert.Throws<IndexOutOfRangeException>(() => tiles[width, height] = tileData);
		}

		[Test]
		public void SetAllTilesWithCoordData()
		{
			var width = 4;
			var height = 8;
			var tiles = new Tile3DCollection(new Vector2Int(width, height));

			var coordDataCount = width * height;
			var tileCoordDatas = new Tile3DCoord[coordDataCount];
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var coord = new Vector3Int(x, 0, y);
					var index = Grid3DUtility.ToIndex2D(x, y, width);
					var flags = x % 2 == 0 ? Tile3DFlags.DirectionWest : Tile3DFlags.DirectionEast;
					flags |= y % 2 == 0 ? Tile3DFlags.FlipHorizontal : Tile3DFlags.FlipVertical;

					var tileData = Tile3D.New(index + 1, flags);
					var coordData = Tile3DCoord.New(coord, tileData);
					tileCoordDatas[index] = coordData;
				}
			}

			tiles.SetTiles(tileCoordDatas);

			Assert.That(tiles.Count == tiles.Capacity);
			Assert.That(tiles.Capacity == coordDataCount);
			Assert.That(tiles.Count == coordDataCount);

			var prevFlags = Tile3DFlags.None;
			for (var i = 0; i < tiles.Count; i++)
			{
				Assert.That(tiles[i].Index == i + 1);
				Assert.That(tiles[i].Flags != Tile3DFlags.None);
				Assert.That(tiles[i].Flags != Tile3DFlags.DirectionNorth);
				Assert.That(tiles[i].Flags != prevFlags);
				prevFlags = tiles[i].Flags;
			}
		}
	}
}
