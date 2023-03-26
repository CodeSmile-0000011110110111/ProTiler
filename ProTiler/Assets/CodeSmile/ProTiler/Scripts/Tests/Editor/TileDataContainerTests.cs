// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using CodeSmile.ProTiler.Extensions;
using NUnit.Framework;
using UnityEngine.Tilemaps;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using TileData = CodeSmile.ProTiler.Data.TileData;
using TileFlags = CodeSmile.ProTiler.Data.TileFlags;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.ProTiler.Tests.Editor
{
	public class TileDataContainerTests
	{
		[Test]
		public void GetSetTileTests()
		{
			var tdc = new TileDataContainer();
			var coord1 = new GridCoord();
			var coord2 = new GridCoord(-1, 0, 1);
			var tile1 = new TileData(1);
			var tile2 = new TileData(2);
			tdc.SetTile(coord1, tile1);

			Assert.AreEqual(1, tdc.Count);
			tdc.SetTile(coord1, tile2);
			Assert.AreEqual(1, tdc.Count);

			Assert.IsTrue(tdc.Contains(coord1));
			Assert.IsFalse(tdc.Contains(coord2));
			Assert.AreEqual(2, tdc.GetTile(coord1).TileSetIndex);
			Assert.AreEqual(TileData.InvalidTileData, tdc.GetTile(coord2));

			tdc.ClearAllTiles();

			var tile3 = new TileData(3);
			var tile4 = new TileData(4);
			Assert.IsFalse(tdc.Contains(coord1));
			Assert.IsFalse(tdc.Contains(coord2));
			tdc.SetTile(coord1, tile3);
			tdc.SetTile(coord2, tile4);
			Assert.AreEqual(2, tdc.Count);
			Assert.IsTrue(tdc.Contains(coord1));
			Assert.IsTrue(tdc.Contains(coord2));
			Assert.AreEqual(3, tdc.GetTile(coord1).TileSetIndex);
			Assert.AreEqual(4, tdc.GetTile(coord2).TileSetIndex);

			tdc.SetTile(coord1, TileData.InvalidTileData);
			Assert.IsTrue(tdc.Contains(coord1));
			Assert.AreEqual(2, tdc.Count);
			tdc.ClearTile(coord1);
			Assert.IsFalse(tdc.Contains(coord1));
			Assert.AreEqual(1, tdc.Count);
		}

		[Test]
		public void FlagsTests()
		{
			var tdc = new TileDataContainer();
			var coord1 = new GridCoord();
			var coord2 = new GridCoord(-1, 0, 1);
			tdc.SetTile(coord1, new TileData(1));

			tdc.SetTileFlags(coord1, TileFlags.AllDirections);
			Assert.AreEqual(TileFlags.AllDirections, tdc.GetTile(coord1).Flags);
			tdc.ClearTileFlags(coord1, TileFlags.AllDirections);
			Assert.AreEqual(TileFlags.None, tdc.GetTile(coord1).Flags);
			tdc.RotateTile(coord1, -1);
			Assert.AreEqual(TileFlags.DirectionWest, tdc.GetTile(coord1).Flags);
			tdc.FlipTile(coord1, 1);
			Assert.AreEqual(TileFlags.DirectionWest | TileFlags.FlipVertical, tdc.GetTile(coord1).Flags);

			// coord2 does not exist
			tdc.SetTileFlags(coord2, TileFlags.AllDirections);
			tdc.ClearTileFlags(coord2, TileFlags.AllDirections);
			tdc.RotateTile(coord2, 1);
			tdc.FlipTile(coord2, -1);
			Assert.AreEqual(1, tdc.Count);
		}

		[Test]
		public void SetTileIndexesTests()
		{
			var tdc = new TileDataContainer();
			var coord1 = new GridCoord(-1, 0, -1);
			var coord2 = new GridCoord(2, 0, 2);
			tdc.SetTileIndexes(new GridRect(coord1.ToCoord2(), coord2.ToCoord2()), 1);
			Assert.AreEqual(4, tdc.Count);
			tdc.SetTileIndexes(new GridRect(coord1.ToCoord2(), coord2.ToCoord2()), TileData.InvalidTileSetIndex);
			Assert.AreEqual(0, tdc.Count);
		}
	}
}