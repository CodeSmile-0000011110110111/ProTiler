// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Data;
using NUnit.Framework;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Tile3DDataTests
	{
		[Test]
		public void CreateTileTests()
		{
			var tile = new Tile3DData();
			Assert.IsTrue(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(0, tile.TileIndex);
			Assert.AreEqual(Tile3DFlags.None, tile.Flags);

			var flags = Tile3DFlags.DirectionNorth | Tile3DFlags.FlipVertical;
			tile = new Tile3DData { TileIndex = 1, Flags = flags };
			Assert.IsFalse(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(1, tile.TileIndex);
			Assert.AreEqual(flags, tile.Flags);

			tile = Tile3DData.New(2);
			Assert.IsFalse(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(2, tile.TileIndex);
			Assert.AreEqual(Tile3DFlags.DirectionNorth, tile.Flags);

			tile = Tile3DData.New(3, flags);
			Assert.IsFalse(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(3, tile.TileIndex);
			Assert.AreEqual(flags, tile.Flags);

			tile = Tile3DData.New();
			Assert.IsTrue(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(0, tile.TileIndex);
			Assert.AreEqual(Tile3DFlags.DirectionNorth, tile.Flags);

			tile = Tile3DData.New(-1);
			Assert.IsFalse(tile.IsEmpty);
			Assert.IsFalse(tile.IsValid);
		}

		[Test]
		public void DirectionTests()
		{
			var tile = new Tile3DData();
			Assert.AreEqual(Tile3DFlags.DirectionNorth, tile.Direction);

			tile = Tile3DData.New();
			Assert.AreEqual(Tile3DFlags.DirectionNorth, tile.Direction);

			tile = Tile3DData.New(1, Tile3DFlags.AllDirections);
			Assert.AreEqual(Tile3DFlags.AllDirections, tile.Direction);
		}
	}
}
