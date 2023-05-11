// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
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

		public static Tile3DCoord[] CreateTileCoordsWithIncrementingIndex(int width, int length)
		{
			var coordDataCount = width * length;
			var tileCoords = new Tile3DCoord[coordDataCount];
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < length; y++)
				{
					var coord = new GridCoord(x, 0, y);
					var index = Grid3DUtility.ToIndex2D(x, y, width);
					tileCoords[index] = new Tile3DCoord(coord, new Tile3D((short)(index + 1)));
				}
			}
			return tileCoords;
		}

		public static Tile3DCoord[] CreateOneTileCoord(int width, int height, int length) => new[]
		{
			new Tile3DCoord(new GridCoord(width - 1, height, length - 1),
				new Tile3D((short)(width + height + length))),
		};

		public static void AssertThatAllTilesHaveIncrementingIndex(int width, int length, Tile3DLayer tiles)
		{
			for (var i = 0; i < width * length; i++)
				Assert.That(tiles[i].Index, Is.EqualTo((short)(i + 1)));
		}
	}
}
