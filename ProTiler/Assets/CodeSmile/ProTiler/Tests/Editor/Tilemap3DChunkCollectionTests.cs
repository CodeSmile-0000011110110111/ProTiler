﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Collections;
using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using System;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Tilemap3DChunkCollectionTests
	{
		[Test]
		public void CreateChunk()
		{
			var chunkSize = new Vector2Int(7, 4);
			var chunks = new Tilemap3DChunkCollection(chunkSize);
			Assert.AreEqual(chunkSize, chunks.Size);
			Assert.AreEqual(0, chunks.Count);
			Assert.AreEqual(0, chunks.TileCount); // should be all empty tiles
		}

		[Test]
		public void CreateIllegalChunkSize()
		{
			var min = Tilemap3DChunkCollection.MinChunkSize;
			var chunks = new Tilemap3DChunkCollection(new Vector2Int(1, 0));
			Assert.AreEqual(new Vector2Int(min, min), chunks.Size);
			chunks = new Tilemap3DChunkCollection(new Vector2Int(0, 1));
			Assert.AreEqual(new Vector2Int(min, min), chunks.Size);
			chunks = new Tilemap3DChunkCollection(new Vector2Int(2, 1));
			Assert.AreEqual(new Vector2Int(min, min), chunks.Size);
			chunks = new Tilemap3DChunkCollection(new Vector2Int(-1, -1));
			Assert.AreEqual(new Vector2Int(min, min), chunks.Size);
		}

		[Test]
		public void ChangeChunkSize()
		{
			var chunkSize = new Vector2Int(7, 4);
			var chunks = new Tilemap3DChunkCollection(chunkSize);
			Assert.AreEqual(chunkSize, chunks.Size);

			var newSize = new Vector2Int(6, 9);
			chunks.ChangeChunkSize(newSize);
			Assert.AreEqual(newSize, chunks.Size);

			chunks.ChangeChunkSize(newSize);
			Assert.AreEqual(newSize, chunks.Size);
		}

		[Test]
		public void SetAndGetTiles()
		{
			var width = 3;
			var height = 4;
			var layers = 2;

			var tileCount = width * height * layers;
			var tileCoordDatas = new Tile3DCoordData[tileCount];
			var coords = new Vector3Int[tileCount];
			var arrayIndex = 0;
			for (var layerY = 0; layerY < layers; layerY++)
			{
				for (var y = 0; y < height; y++)
				{
					for (var x = 0; x < width; x++)
					{
						var coord = new Vector3Int(x, layerY, y);
						var index = Grid3DUtility.ToIndex2D(x, y, width);
						var tileData = Tile3DData.New(index + 1, (Tile3DFlags)(1 << index % 6));
						var coordData = Tile3DCoordData.New(coord, tileData);
						tileCoordDatas[arrayIndex] = coordData;
						coords[arrayIndex] = coord;
						arrayIndex++;
					}
				}
			}

			var getTileCoordDatas = new Tile3DCoordData[tileCount];
			var chunkSize = new Vector2Int(width, height);
			var chunks = new Tilemap3DChunkCollection(chunkSize);
			chunks.SetTiles(tileCoordDatas);
			Assert.AreEqual(1, chunks.Count);
			Assert.AreEqual(tileCount, chunks.TileCount);
			chunks.GetTiles(coords, ref getTileCoordDatas);
			for (var i = 0; i < tileCount; i++)
				Assert.IsTrue(getTileCoordDatas[i].TileData == tileCoordDatas[i].TileData);

			chunkSize = new Vector2Int(width - 1, height - 1);
			chunks = new Tilemap3DChunkCollection(chunkSize);
			chunks.SetTiles(tileCoordDatas);
			Assert.AreEqual(tileCount, chunks.TileCount);
			for (var i = 0; i < tileCount; i++)
				Assert.IsTrue(getTileCoordDatas[i].TileData == tileCoordDatas[i].TileData);

			chunkSize = new Vector2Int(Tilemap3DChunkCollection.MinChunkSize, Tilemap3DChunkCollection.MinChunkSize);
			chunks = new Tilemap3DChunkCollection(chunkSize);
			Assert.Throws<IndexOutOfRangeException>(() => { chunks.SetTiles(tileCoordDatas); });
		}

		[Test]
		public void GetTilesInvalidParams()
		{
			var width = 3;
			var height = 4;

			var chunkSize = new Vector2Int(width, height);
			var chunks = new Tilemap3DChunkCollection(chunkSize);

			var coords = new Vector3Int[0];
			var tileCoordDatas = new Tile3DCoordData[0];
			chunks.GetTiles(coords, ref tileCoordDatas);
			chunks.GetTiles(null, ref tileCoordDatas);
			tileCoordDatas = null;
			chunks.GetTiles(coords, ref tileCoordDatas);
		}

		[Test]
		public void GetTilesOutsideBounds()
		{
			var width = 3;
			var height = 4;
			var chunkSize = new Vector2Int(width, height);
			var chunks = new Tilemap3DChunkCollection(chunkSize);

			var coords = new Vector3Int[] { new(100, 0, 100) };
			var tileCoordDatas = new Tile3DCoordData[10];
			chunks.GetTiles(coords, ref tileCoordDatas);
			Assert.AreEqual(0, tileCoordDatas[0].TileData.Index);

			coords = new Vector3Int[] { new(3, 100, 2) };
			chunks.SetTiles(new[]
			{
				Tile3DCoordData.New(new Vector3Int(3, 0, 2), Tile3DData.New(13)),
			});
			chunks.GetTiles(coords, ref tileCoordDatas);
			Assert.AreEqual(0, tileCoordDatas[0].TileData.Index);
		}

		[Test]
		public void ToChunkCoord()
		{
			var chunks = new Tilemap3DChunkCollection(2, 2);
			Assert.AreEqual(new Vector3Int(0, 0, 0), chunks.ToChunkCoord(new Vector3Int(0, 0, 0)));
			Assert.AreEqual(new Vector3Int(0, 1, 0), chunks.ToChunkCoord(new Vector3Int(1, 1, 1)));
			Assert.AreEqual(new Vector3Int(1, -1, 1), chunks.ToChunkCoord(new Vector3Int(2, -1, 2)));
			Assert.AreEqual(new Vector3Int(1, -2, 1), chunks.ToChunkCoord(new Vector3Int(3, -2, 3)));
			Assert.AreEqual(new Vector3Int(3, 0, 2), chunks.ToChunkCoord(new Vector3Int(6, 0, 5)));
			Assert.AreEqual(new Vector3Int(0, 0, 0), chunks.ToChunkCoord(new Vector3Int(-1, 0, -1)));
			Assert.AreEqual(new Vector3Int(-1, 1, -1), chunks.ToChunkCoord(new Vector3Int(-2, 1, -2)));
			Assert.AreEqual(new Vector3Int(-1, -5, -2), chunks.ToChunkCoord(new Vector3Int(-3, -5, -4)));

			chunks = new Tilemap3DChunkCollection(3, 7);
			Assert.AreEqual(new Vector3Int(0, 0, 0), chunks.ToChunkCoord(new Vector3Int(0, 0, 0)));
			Assert.AreEqual(new Vector3Int(2, 1, 0), chunks.ToChunkCoord(new Vector3Int(6, 1, 2)));
			Assert.AreEqual(new Vector3Int(2, 1, 0), chunks.ToChunkCoord(new Vector3Int(7, 1, 4)));
			Assert.AreEqual(new Vector3Int(3, 2, 2), chunks.ToChunkCoord(new Vector3Int(9, 2, 14)));
			Assert.AreEqual(new Vector3Int(0, 1, 0), chunks.ToChunkCoord(new Vector3Int(-2, 1, -6)));
			Assert.AreEqual(new Vector3Int(-1, 1, -1), chunks.ToChunkCoord(new Vector3Int(-3, 1, -7)));
		}

		[Test]
		public void ToLayerCoord()
		{
			var chunks = new Tilemap3DChunkCollection(4, 5);
			var chunkCoord = new Vector3Int(0, 0, 0);
			Assert.AreEqual(new Vector3Int(0, 0, 0), chunks.ToLayerCoord(chunkCoord, new Vector3Int(0, 0, 0)));
			Assert.AreEqual(new Vector3Int(5, 5, -5), chunks.ToLayerCoord(chunkCoord, new Vector3Int(5, 5, -5)));

			chunkCoord = new Vector3Int(3, 1, 2);
			Assert.AreEqual(new Vector3Int(3, 4, -7), chunks.ToLayerCoord(chunkCoord, new Vector3Int(6, 5, -5)));
		}
	}
}