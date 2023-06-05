// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using GridCoord = Unity.Mathematics.int3;

namespace CodeSmile.Tests.Editor.ProTiler3.Utility
{
	internal static class Tile3DTestUtility
	{
		internal static void SetAllTilesWithIncrementingIndex(ref Tile3DLayer tiles, Int32 width, Int32 length)
		{
			for (var i = 0; i < width * length; i++)
				tiles[i] = new Tile3D(i + 1);
		}

		internal static Tile3DCoord[] CreateTileCoordsWithIncrementingIndex(Int32 width, Int32 length, Int32 height = 0)
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

		internal static Tile3DCoord[] CreateTileCoordsWithIncrementingIndexAcrossLayers(Int32 width, Int32 height,
			Int32 length)
		{
			var tileCoordsLayers = new List<Tile3DCoord>();
			for (var y = 0; y < height; y++)
				tileCoordsLayers.AddRange(CreateTileCoordsWithIncrementingIndex(width, length, y));
			return tileCoordsLayers.ToArray();
		}

		internal static Tile3DCoord[] CreateOneTileCoord(Int32 x, Int32 y, Int32 z) => new[]
		{
			new Tile3DCoord(new GridCoord(x - 1, y, z - 1),
				new Tile3D(x + y + z)),
		};

		internal static void AssertThatAllTilesHaveIncrementingIndex(Int32 width, Int32 length, Tile3DLayer tiles,
			Int32 height = 0)
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

		/// <summary>
		///     Returns the grid coordinates of a Tile3DCoord collection.
		/// </summary>
		/// <param name="tileCoords"></param>
		/// <returns></returns>
		internal static GridCoord[] ToCoordArray(this Tile3DCoord[] tileCoords)
		{
			var coords = new GridCoord[tileCoords.Length];
			for (var i = 0; i < tileCoords.Length; i++)
				coords[i] = tileCoords[i].Coord;
			return coords;
		}

		private static Int32 MakeDummyTileIndex(Int32 tileIndex, Int32 height)
		{
			var dummyIndex = tileIndex + 1 << height;
			Assert.That(tileIndex + 1 << height, Is.EqualTo(dummyIndex), "dummy index overflow");
			return dummyIndex;
		}
	}
}
