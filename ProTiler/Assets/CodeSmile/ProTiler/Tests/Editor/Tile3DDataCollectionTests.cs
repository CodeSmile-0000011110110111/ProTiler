﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Collections;
using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Tile3DDataCollectionTests
	{
		[Test]
		public void CreateCollection()
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
		public void SetTile()
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

		[Test]
		public void SetTilesWithCoordData()
		{
			var width = 4;
			var height = 8;
			var tiles = new Tile3DDataCollection(new Vector2Int(width, height));

			var coordDataCount = width*height;
			Tile3DCoordData[] tileCoordDatas = new Tile3DCoordData[coordDataCount];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var coord = new Vector3Int(x, 0, y);
					var index = Grid3DUtility.ToIndex(x, y, width);
					var flags = x % 2 == 0 ? Tile3DFlags.DirectionWest:Tile3DFlags.DirectionEast;
					flags |= y % 2 == 0 ? Tile3DFlags.FlipHorizontal : Tile3DFlags.FlipVertical;

					var tileData = Tile3DData.New(index + 1, flags);
					var coordData = Tile3DCoordData.New(coord, tileData);
					tileCoordDatas[index] = coordData;
				}
			}

			tiles.SetTiles(tileCoordDatas);

			Assert.AreEqual(tiles.Capacity, tiles.Count);
			Assert.AreEqual(coordDataCount, tiles.Capacity);
			Assert.AreEqual(coordDataCount, tiles.Count);

			var prevFlags = Tile3DFlags.None;
			for (int i = 0; i < tiles.Count; i++)
			{
				Assert.AreEqual(i + 1, tiles[i].TileIndex);
				Assert.AreNotEqual(Tile3DFlags.None, tiles[i].Flags);
				Assert.AreNotEqual(Tile3DFlags.DirectionNorth, tiles[i].Flags);
				Assert.AreNotEqual(prevFlags, tiles[i].Flags);
				prevFlags = tiles[i].Flags;
				//Debug.Log($"[{i}] = {tiles[i].Flags}");
			}
		}
	}
}