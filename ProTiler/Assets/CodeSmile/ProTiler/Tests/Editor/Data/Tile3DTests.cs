// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;

namespace CodeSmile.ProTiler.Tests.Editor.Data
{
	public class Tile3DTests
	{
		[Test]
		public void CreateEmptyTileWithNewKeyword()
		{
			var tile = new Tile3D();

			Assert.That(tile.IsEmpty, Is.True);
			Assert.That(tile.IsValid, Is.True);
			Assert.That(tile.Flags == Tile3DFlags.None);
		}

		[Test]
		public void CreateEmptyTile()
		{
			var tile = Tile3D.New();

			Assert.That(tile.IsEmpty, Is.True);
			Assert.That(tile.IsValid, Is.True);
			Assert.That(tile.Index == 0);
		}

		[Test]
		public void CreateNonEmptyTileWithFlags()
		{
			var tileIndex = 3;
			var flags = Tile3DFlags.DirectionNorth | Tile3DFlags.FlipVertical;

			var tile = Tile3D.New(tileIndex, flags);

			Assert.That(tile.IsEmpty, Is.False);
			Assert.That(tile.Index == tileIndex);
			Assert.That(tile.Flags == flags);
		}

		[Test]
		public void CreateTileWithDefaultFlags()
		{
			var tileIndex = 5;

			var tile = Tile3D.New(tileIndex);

			Assert.That(tile.IsEmpty, Is.False);
			Assert.That(tile.IsValid, Is.True);
			Assert.That(tile.Index == tileIndex);
			Assert.That(tile.Flags == Tile3DFlags.DirectionNorth);
		}

		[Test]
		public void CreateInvalidTile()
		{
			var tile = Tile3D.New(-1);

			Assert.That(tile.IsEmpty, Is.True);
			Assert.That(tile.IsValid, Is.False);
		}

		[Test]
		public void GetDirectionDefault()
		{
			var tile = Tile3D.New();

			Assert.That(tile.Direction == Tile3DFlags.DirectionNorth);
		}

		[Test]
		public void GetDirectionCustom()
		{
			var tile = Tile3D.New(1, Tile3DFlags.AllDirections);

			Assert.That(tile.Direction == Tile3DFlags.AllDirections);
		}

		[Test]
		public void TilesWithDifferentIndexAndSameFlagsAreNotEqual()
		{
			var tile1 = new Tile3D { Index = 11, Flags = Tile3DFlags.DirectionSouth };
			var tile2 = new Tile3D { Index = 17, Flags = Tile3DFlags.DirectionSouth };

			Assert.That(tile1 == tile2, Is.False);
			Assert.That(tile1 != tile2, Is.True);
			Assert.That(tile1.Equals(tile2), Is.False);
			Assert.That(tile1.Equals((object)tile2), Is.False);
		}

		[Test]
		public void TilesWithSameIndexAndDifferentFlagsAreNotEqual()
		{
			var tile1 = new Tile3D { Index = 1, Flags = Tile3DFlags.DirectionEast };
			var tile2 = new Tile3D { Index = 1, Flags = Tile3DFlags.DirectionSouth };

			Assert.That(tile1 == tile2, Is.False);
			Assert.That(tile1 != tile2, Is.True);
			Assert.That(tile1.Equals(tile2), Is.False);
			Assert.That(tile1.Equals((object)tile2), Is.False);
		}

		[Test]
		public void TilesWithSameIndexAndFlagsAreEqual()
		{
			var tile1 = new Tile3D { Index = 7, Flags = Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal };
			var tile2 = new Tile3D { Index = 7, Flags = Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal };

			Assert.That(tile1 == tile2, Is.True);
			Assert.That(tile1 != tile2, Is.False);
			Assert.That(tile1.Equals(tile2), Is.True);
			Assert.That(tile1.Equals((object)tile2), Is.True);
		}

		[Test]
		public void TilesWithDifferentIndexHaveNonEqualHashcodes()
		{
			var tile1 = new Tile3D { Index = 2 };
			var tile2 = new Tile3D { Index = 13 };

			Assert.That(tile1.GetHashCode() != tile2.GetHashCode());
		}

		[Test]
		public void TilesWithSameIndexAndDifferentFlagsHaveNonEqualHashcodes()
		{
			var tile1 = new Tile3D { Index = 2, Flags = Tile3DFlags.FlipHorizontal };
			var tile2 = new Tile3D { Index = 13, Flags = Tile3DFlags.FlipVertical };

			Assert.That(tile1.GetHashCode() != tile2.GetHashCode());
		}

		[Test]
		public void TilesWithSameIndexAndFlagsHaveEqualHashcodes()
		{
			var tile1 = new Tile3D { Index = 2, Flags = Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal };
			var tile2 = new Tile3D { Index = 2, Flags = Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal };

			Assert.That(tile1.GetHashCode() == tile2.GetHashCode());
		}
	}
}
