// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using System.Collections.Generic;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.Tests.Editor.ProTiler.Data
{
	internal static class Tile3DTestUtility
	{
		public static void SetAllTilesWithIncrementingIndex(ref Tile3DLayer tiles, int width, int length)
		{
			for (var i = 0; i < width * length; i++)
				tiles[i] = new Tile3D(i + 1);
		}

		public static Tile3DCoord[] CreateTileCoordsWithIncrementingIndex(int width, int length, int height = 0)
		{
			var coordDataCount = width * length;
			var tileCoords = new Tile3DCoord[coordDataCount];
			for (var x = 0; x < width; x++)
			{
				for (var z = 0; z < length; z++)
				{
					var coord = new GridCoord(x, height, z);
					var tileIndex = Grid3DUtility.ToIndex2D(x, z, width);
					var dummyTileIndex = MakeDummyTileIndex(tileIndex, height);
					//Debug.Log($"{coord} dummyTileIndex: {dummyTileIndex}");
					tileCoords[tileIndex] = new Tile3DCoord(coord, new Tile3D(dummyTileIndex));
				}
			}
			return tileCoords;
		}

		public static Tile3DCoord[] CreateTileCoordsWithIncrementingIndexAcrossLayers(int width, int height, int length)
		{
			var tileCoordsLayers = new List<Tile3DCoord>();
			for (var y = 0; y < height; y++)
				tileCoordsLayers.AddRange(CreateTileCoordsWithIncrementingIndex(width, length, y));
			return tileCoordsLayers.ToArray();
		}

		public static Tile3DCoord[] CreateOneTileCoord(int x, int y, int z) => new[]
		{
			new Tile3DCoord(new GridCoord(x - 1, y, z - 1),
				new Tile3D(x + y + z)),
		};

		public static void AssertThatAllTilesHaveIncrementingIndex(int width, int length, Tile3DLayer tiles,
			int height = 0)
		{
			for (var x = 0; x < width; x++)
			{
				for (var z = 0; z < length; z++)
				{
					var coord = new GridCoord(x, height, z);
					var tileIndex = Grid3DUtility.ToIndex2D(x, z, width);
					var expected = MakeDummyTileIndex(tileIndex, height);
					//Debug.Log($"{coord} expected Index: {expected}");
					Assert.That(tiles[tileIndex].Index, Is.EqualTo(expected));
				}
			}
		}

		private static int MakeDummyTileIndex(int tileIndex, int height)
		{
			var dummyIndex = tileIndex + 1 << height;
			Assert.That(tileIndex + 1 << height, Is.EqualTo(dummyIndex), "dummy index overflow");
			return dummyIndex;
		}
	}
}
