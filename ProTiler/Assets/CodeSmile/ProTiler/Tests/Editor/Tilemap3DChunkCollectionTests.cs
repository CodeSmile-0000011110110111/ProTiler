// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Collections;
using CodeSmile.ProTiler.Data;
using NUnit.Framework;
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
		public void ChangeChunkSize()
		{
			var chunkSize = new Vector2Int(7, 4);
			var chunks = new Tilemap3DChunkCollection(chunkSize);
			Assert.AreEqual(chunkSize, chunks.Size);

			var newSize = new Vector2Int(6, 9);
			chunks.ChangeChunkSize(newSize);
			Assert.AreEqual(newSize, chunks.Size);
		}

		[Test]
		public void SetTiles()
		{
			var width = 3;
			var height = 2;
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
						var flags = Tile3DFlags.DirectionSouth | Tile3DFlags.FlipVertical;
						var tileData = Tile3DData.New(index + 1, flags);
						var coordData = Tile3DCoordData.New(coord, tileData);
						tileCoordDatas[arrayIndex] = coordData;
						coords[arrayIndex] = coord;
						arrayIndex++;
					}
				}
			}

			var chunkSize = new Vector2Int(width, height);
			var chunks = new Tilemap3DChunkCollection(chunkSize);
			chunks.SetTiles(tileCoordDatas);
			Assert.AreEqual(1, chunks.Count);
			Assert.AreEqual(tileCount, chunks.TileCount);

			chunkSize = new Vector2Int(1, 1);
			chunks = new Tilemap3DChunkCollection(chunkSize);
			chunks.SetTiles(tileCoordDatas);
			Assert.AreEqual(width * height, chunks.Count);
			Assert.AreEqual(tileCount, chunks.TileCount);

			chunkSize = new Vector2Int(2, 1);
			chunks = new Tilemap3DChunkCollection(chunkSize);
			chunks.SetTiles(tileCoordDatas);
			Assert.AreEqual(tileCount, chunks.TileCount);

			chunkSize = new Vector2Int(1, 2);
			chunks = new Tilemap3DChunkCollection(chunkSize);
			chunks.SetTiles(tileCoordDatas);
			Assert.AreEqual(tileCount, chunks.TileCount);

			/*
			chunks.GetTileData(coords, ref var tileDatas);

			for (var index = 0; index < tileCount; index++)
			{
				var coord = Grid3DUtility.ToCoord(index, width, index % layers);
			}
			*/

			/*
			var prevFlags = Tile3DFlags.None;
			for (var i = 0; i < tiles.Count; i++)
			{
				Assert.AreEqual(i + 1, tiles[i].TileIndex);
				Assert.AreNotEqual(Tile3DFlags.None, tiles[i].Flags);
				Assert.AreNotEqual(Tile3DFlags.DirectionNorth, tiles[i].Flags);
				Assert.AreNotEqual(prevFlags, tiles[i].Flags);
				prevFlags = tiles[i].Flags;
				//Debug.Log($"[{i}] = {tiles[i].Flags}");
			}
		*/
		}
	}
}
