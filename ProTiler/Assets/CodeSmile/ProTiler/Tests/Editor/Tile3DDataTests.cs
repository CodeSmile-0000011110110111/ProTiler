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
		public void CreateTile()
		{
			var tile = new Tile3DData();
			Assert.IsTrue(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(0, tile.Index);
			Assert.AreEqual(Tile3DFlags.None, tile.Flags);

			var flags = Tile3DFlags.DirectionNorth | Tile3DFlags.FlipVertical;
			tile = Tile3DData.New(1, flags);
			Assert.IsFalse(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(1, tile.Index);
			Assert.AreEqual(flags, tile.Flags);

			tile = Tile3DData.New(2);
			Assert.IsFalse(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(2, tile.Index);
			Assert.AreEqual(Tile3DFlags.DirectionNorth, tile.Flags);

			tile = Tile3DData.New(3, flags);
			Assert.IsFalse(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(3, tile.Index);
			Assert.AreEqual(flags, tile.Flags);

			tile = Tile3DData.New();
			Assert.IsTrue(tile.IsEmpty);
			Assert.IsTrue(tile.IsValid);
			Assert.AreEqual(0, tile.Index);
			Assert.AreEqual(Tile3DFlags.DirectionNorth, tile.Flags);

			tile = Tile3DData.New(-1);
			Assert.IsFalse(tile.IsEmpty);
			Assert.IsFalse(tile.IsValid);
		}

		[Test]
		public void GetDirection()
		{
			var tile = new Tile3DData();
			Assert.AreEqual(Tile3DFlags.DirectionNorth, tile.Direction);

			tile = Tile3DData.New();
			Assert.AreEqual(Tile3DFlags.DirectionNorth, tile.Direction);

			tile = Tile3DData.New(1, Tile3DFlags.AllDirections);
			Assert.AreEqual(Tile3DFlags.AllDirections, tile.Direction);
		}


		[Test]
		public void Equality()
		{
			var tile1East = new Tile3DData { Index = 1, Flags = Tile3DFlags.DirectionEast };
			var tile1South = new Tile3DData { Index = 1, Flags = Tile3DFlags.DirectionSouth };
			var tile2a = new Tile3DData { Index = 2, Flags = Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal};
			var tile2b = new Tile3DData { Index = 2,Flags = Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal };

			Assert.IsFalse(tile1East == tile1South);
			Assert.IsTrue(tile1East != tile1South);
			Assert.IsFalse(tile1East.Equals(tile1South));
			Assert.IsFalse(tile1East.Equals((object)tile1South));

			Assert.IsTrue(tile2a == tile2b);
			Assert.IsFalse(tile2a != tile2b);
			Assert.IsTrue(tile2a.Equals(tile2b));
			Assert.IsTrue(tile2a.Equals((object)tile2b));

			Assert.IsTrue(tile2a.GetHashCode() == tile2b.GetHashCode());
			Assert.IsFalse(tile1East.GetHashCode() == tile2b.GetHashCode());
			Assert.IsFalse(tile1East.GetHashCode() == tile1South.GetHashCode());
		}
	}
}
