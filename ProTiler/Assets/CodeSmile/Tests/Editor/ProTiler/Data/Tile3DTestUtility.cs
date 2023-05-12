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
				tiles[i] = new Tile3D((short)(i + 1));
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
					var index = Grid3DUtility.ToIndex2D(x, z, width);
					tileCoords[index] = new Tile3DCoord(coord, new Tile3D((short)(index + 1)));
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
				new Tile3D((short)(x + y + z))),
		};

		public static void AssertThatAllTilesHaveIncrementingIndex(int width, int length, Tile3DLayer tiles)
		{
			for (var i = 0; i < width * length; i++)
				Assert.That(tiles[i].Index, Is.EqualTo((short)(i + 1)));
		}
	}
}
