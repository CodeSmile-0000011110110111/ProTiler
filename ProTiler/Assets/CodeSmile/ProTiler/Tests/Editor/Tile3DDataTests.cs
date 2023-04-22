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
		public void CreateEmptyTileWithNewKeyword()
		{
			var tile = new Tile3DData();

			Assert.IsTrue(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(Tile3DFlags.None, tile.Flags);
		}

		[Test]
		public void CreateEmptyTile()
		{
			var tile = Tile3DData.New();

			Assert.IsTrue(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(0, tile.Index);
		}

		[Test]
		public void CreateNonEmptyTileWithFlags()
		{
			var tileIndex = 3;
			var flags = Tile3DFlags.DirectionNorth | Tile3DFlags.FlipVertical;

			var tile = Tile3DData.New(tileIndex, flags);

			Assert.IsFalse(tile.IsEmpty);
			Assert.AreEqual(tileIndex, tile.Index);
			Assert.AreEqual(flags, tile.Flags);
		}

		[Test]
		public void CreateTileWithDefaultFlags()
		{
			var tileIndex = 5;

			var tile = Tile3DData.New(tileIndex);

			Assert.IsFalse(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(tileIndex, tile.Index);
			Assert.AreEqual(Tile3DFlags.DirectionNorth, tile.Flags);
		}

		[Test]
		public void CreateInvalidTile()
		{
			var tile = Tile3DData.New(-1);

			Assert.IsTrue(tile.IsEmpty);
			Assert.IsFalse(tile.IsValid);
		}

		[Test]
		public void GetDirectionDefault()
		{
			var tile = Tile3DData.New();

			Assert.AreEqual(Tile3DFlags.DirectionNorth, tile.Direction);
		}

		[Test]
		public void GetDirectionCustom()
		{
			var tile = Tile3DData.New(1, Tile3DFlags.AllDirections);

			Assert.AreEqual(Tile3DFlags.AllDirections, tile.Direction);
		}

		[Test]
		public void TilesWithDifferentIndexAndSameFlagsAreNotEqual()
		{
			var tile1 = new Tile3DData { Index = 11, Flags = Tile3DFlags.DirectionSouth };
			var tile2 = new Tile3DData { Index = 17, Flags = Tile3DFlags.DirectionSouth };

			Assert.IsFalse(tile1 == tile2);
			Assert.IsTrue(tile1 != tile2);
			Assert.IsFalse(tile1.Equals(tile2));
			Assert.IsFalse(tile1.Equals((object)tile2));
		}

		[Test]
		public void TilesWithSameIndexAndDifferentFlagsAreNotEqual()
		{
			var tile1 = new Tile3DData { Index = 1, Flags = Tile3DFlags.DirectionEast };
			var tile2 = new Tile3DData { Index = 1, Flags = Tile3DFlags.DirectionSouth };

			Assert.IsFalse(tile1 == tile2);
			Assert.IsTrue(tile1 != tile2);
			Assert.IsFalse(tile1.Equals(tile2));
			Assert.IsFalse(tile1.Equals((object)tile2));
		}

		[Test]
		public void TilesWithSameIndexAndFlagsAreEqual()
		{
			var tile1 = new Tile3DData { Index = 7, Flags = Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal };
			var tile2 = new Tile3DData { Index = 7, Flags = Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal };

			Assert.IsTrue(tile1 == tile2);
			Assert.IsFalse(tile1 != tile2);
			Assert.IsTrue(tile1.Equals(tile2));
			Assert.IsTrue(tile1.Equals((object)tile2));
		}

		[Test]
		public void TilesWithDifferentIndexHaveNonEqualHashcodes()
		{
			var tile1 = new Tile3DData { Index = 2 };
			var tile2 = new Tile3DData { Index = 13 };

			Assert.IsFalse(tile1.GetHashCode() == tile2.GetHashCode());
		}

		[Test]
		public void TilesWithSameIndexAndDifferentFlagsHaveNonEqualHashcodes()
		{
			var tile1 = new Tile3DData { Index = 2, Flags = Tile3DFlags.FlipHorizontal };
			var tile2 = new Tile3DData { Index = 13, Flags = Tile3DFlags.FlipVertical };

			Assert.IsFalse(tile1.GetHashCode() == tile2.GetHashCode());
		}

		[Test]
		public void TilesWithSameIndexAndFlagsHaveEqualHashcodes()
		{
			var tile1 = new Tile3DData { Index = 2, Flags = Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal };
			var tile2 = new Tile3DData { Index = 2, Flags = Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal };

			Assert.IsTrue(tile1.GetHashCode() == tile2.GetHashCode());
		}
	}
}
