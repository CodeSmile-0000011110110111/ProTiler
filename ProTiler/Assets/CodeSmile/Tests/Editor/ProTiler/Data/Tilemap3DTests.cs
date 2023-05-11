﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.Tests.Editor.ProTiler.Data
{
	public class Tilemap3DTests
	{
		private static readonly object[] IllegalChunkSizes =
		{
			new object[] { new Vector2Int(1, 0) },
			new object[] { new Vector2Int(0, 1) },
			new object[] { new Vector2Int(2, 1) },
			new object[] { new Vector2Int(-1, 1) },
			new object[] { new Vector2Int(1, -1) },
		};

		/*
		[Test]
		public void CreateChunk()
		{
			var chunkSize = new Vector2Int(7, 4);
			var chunks = new Tilemap3D(chunkSize);
			Assert.That(chunks.Size, Is.EqualTo(chunkSize));
			Assert.That(chunks.Count == 0);
			Assert.That(chunks.TileCount == 0);
		}

		[TestCaseSource(nameof(IllegalChunkSizes))]
		public void CreateIllegalChunkSize(Vector2Int chunkSize)
		{
			var min = Tilemap3D.MinChunkSize;
			var chunks = new Tilemap3D(chunkSize);
			Assert.That(chunks.Size, Is.EqualTo(new Vector2Int(min, min)));
		}

		[Test]
		public void ChangeChunkSize()
		{
			var chunkSize = new Vector2Int(7, 4);
			var chunks = new Tilemap3D(chunkSize);
			Assert.That(chunks.Size, Is.EqualTo(chunkSize));

			var newSize = new Vector2Int(6, 9);
			chunks.ChangeChunkSize(newSize);
			Assert.That(chunks.Size, Is.EqualTo(newSize));
		}

		[Test]
		public void ChangeChunkSizeToSameSize()
		{
			var chunkSize = new Vector2Int(4, 5);
			var chunks = new Tilemap3D(chunkSize);

			chunks.ChangeChunkSize(chunkSize);
			Assert.That(chunks.Size, Is.EqualTo(chunkSize));
		}

		[Test]
		public void SetAndGetTiles()
		{
			// FIXME: this test does too many things!
			var width = 3;
			var height = 4;
			var layers = 2;

			var tileCount = width * height * layers;
			var tileCoordDatas = new Tile3DCoord[tileCount];
			var coords = new Vector3Int[tileCount];
			var arrayIndex = 0;
			for (var layerY = 0; layerY < layers; layerY++)
			{
				for (var y = 0; y < height; y++)
				{
					for (var x = 0; x < width; x++)
					{
						var coord = new Vector3Int(x, layerY, y);
						var index = (short)Grid3DUtility.ToIndex2D(x, y, width);
						var tileData = new Tile3D((short)(index + 1), (Tile3DFlags)(1 << index % 6));
						var coordData = new Tile3DCoord(coord, tileData);
						tileCoordDatas[arrayIndex] = coordData;
						coords[arrayIndex] = coord;
						arrayIndex++;
					}
				}
			}

			var getTileCoordDatas = new Tile3DCoord[tileCount];
			var chunkSize = new Vector2Int(width, height);
			var chunks = new Tilemap3D(chunkSize);
			chunks.SetTiles(tileCoordDatas);
			Assert.That(chunks.Count == 1);
			Assert.That(chunks.TileCount == tileCount);

			chunks.GetTiles(coords, ref getTileCoordDatas);
			for (var i = 0; i < tileCount; i++)
				Assert.That(getTileCoordDatas[i].Tile == tileCoordDatas[i].Tile);

			chunkSize = new Vector2Int(width - 1, height - 1);
			chunks = new Tilemap3D(chunkSize);
			chunks.SetTiles(tileCoordDatas);
			Assert.That(chunks.TileCount == tileCount);

			for (var i = 0; i < tileCount; i++)
				Assert.That(getTileCoordDatas[i].Tile == tileCoordDatas[i].Tile);

			chunkSize = new Vector2Int(Tilemap3D.MinChunkSize, Tilemap3D.MinChunkSize);
			chunks = new Tilemap3D(chunkSize);
			Assert.Throws<IndexOutOfRangeException>(() => { chunks.SetTiles(tileCoordDatas); });
		}

		[Test]
		public void GetTilesInvalidParamsDoesNotThrow()
		{
			var width = 3;
			var height = 4;
			var chunkSize = new Vector2Int(width, height);
			var chunks = new Tilemap3D(chunkSize);
			var coords = new Vector3Int[0];
			var tileCoordDatas = new Tile3DCoord[0];

			chunks.GetTiles(coords, ref tileCoordDatas);

			chunks.GetTiles(null, ref tileCoordDatas);

			tileCoordDatas = null;
			chunks.GetTiles(coords, ref tileCoordDatas);
		}

		[Test]
		public void GetTilesOutsideBoundsDoesNotThrow()
		{
			var width = 3;
			var height = 4;
			var chunkSize = new Vector2Int(width, height);
			var chunks = new Tilemap3D(chunkSize);

			var coords = new Vector3Int[] { new(100, 0, 100) };
			var tileCoordDatas = new Tile3DCoord[10];
			chunks.GetTiles(coords, ref tileCoordDatas);
			Assert.That(tileCoordDatas[0].Tile.Index == 0);

			coords = new Vector3Int[] { new(3, 100, 2) };
			chunks.SetTiles(new[]
			{
				new Tile3DCoord(new Vector3Int(3, 0, 2), new Tile3D(13)),
			});
			chunks.GetTiles(coords, ref tileCoordDatas);
			Assert.That(tileCoordDatas[0].Tile.Index == 0);
		}

		[Test]
		public void ToChunkCoord()
		{
			var chunks = new Tilemap3D(new Vector2Int(2, 2));
			Assert.AreEqual(new Vector3Int(0, 0, 0), chunks.ToChunkCoord(new Vector3Int(0, 0, 0)));
			Assert.AreEqual(new Vector3Int(0, 1, 0), chunks.ToChunkCoord(new Vector3Int(1, 1, 1)));
			Assert.AreEqual(new Vector3Int(1, -1, 1), chunks.ToChunkCoord(new Vector3Int(2, -1, 2)));
			Assert.AreEqual(new Vector3Int(1, -2, 1), chunks.ToChunkCoord(new Vector3Int(3, -2, 3)));
			Assert.AreEqual(new Vector3Int(3, 0, 2), chunks.ToChunkCoord(new Vector3Int(6, 0, 5)));
			Assert.AreEqual(new Vector3Int(0, 0, 0), chunks.ToChunkCoord(new Vector3Int(-1, 0, -1)));
			Assert.AreEqual(new Vector3Int(-1, 1, -1), chunks.ToChunkCoord(new Vector3Int(-2, 1, -2)));
			Assert.AreEqual(new Vector3Int(-1, -5, -2), chunks.ToChunkCoord(new Vector3Int(-3, -5, -4)));

			chunks = new Tilemap3D(new Vector2Int(3, 7));
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
			var chunks = new Tilemap3D(new Vector2Int(4, 5));
			var chunkCoord = new Vector3Int(0, 0, 0);
			Assert.That(chunks.ToLayerCoord(chunkCoord, new Vector3Int(0, 0, 0)),
				Is.EqualTo(new Vector3Int(0, 0, 0)));
			Assert.That(chunks.ToLayerCoord(chunkCoord, new Vector3Int(5, 5, -5)),
				Is.EqualTo(new Vector3Int(5, 5, -5)));

			chunkCoord = new Vector3Int(3, 1, 2);
			Assert.That(chunks.ToLayerCoord(chunkCoord, new Vector3Int(6, 5, -5)),
				Is.EqualTo(new Vector3Int(3, 4, -7)));
		}
	*/
	}
}