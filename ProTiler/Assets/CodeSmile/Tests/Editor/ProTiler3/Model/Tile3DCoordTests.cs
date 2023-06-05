// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Model;
using NUnit.Framework;
using System;
using GridCoord = Unity.Mathematics.int3;

namespace CodeSmile.Tests.Editor.ProTiler3.Model
{
	public class Tile3DCoordTests
	{
		private readonly GridCoord m_TestCoord = new(1, 2, 3);
		private readonly Tile3D m_TestTile = new(123, Tile3DFlags.DirectionEast | Tile3DFlags.FlipVertical);

		[Test] public void TileCreatedWithNewKeywordHasDefaultValues()
		{
			var tileCoord = new Tile3DCoord();

			Assert.That(tileCoord.Coord, Is.EqualTo(new GridCoord()));
			Assert.That(tileCoord.Tile, Is.EqualTo(new Tile3D()));
		}

		[Test] public void NewTileReturnsParams()
		{
			var tileCoord = new Tile3DCoord(m_TestCoord, m_TestTile);

			Assert.That(tileCoord.Coord, Is.EqualTo(m_TestCoord));
			Assert.That(tileCoord.Tile, Is.EqualTo(m_TestTile));
		}

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.NonEqualTileCoordPairs))]
		public void TilesWithDifferentIndexAndSameFlagsAreNotEqual(Tile3DCoord tile1, Tile3DCoord tile2)
		{
			Assert.That(tile1 == tile2, Is.False);
			Assert.That(tile1 != tile2, Is.True);
			Assert.That(tile1.Equals(tile2), Is.False);
			Assert.That(tile2.Equals(tile1), Is.False);
			Assert.That(tile1.Equals((Object)tile2), Is.False);
			Assert.That(tile2.Equals((Object)tile1), Is.False);
		}

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.EqualTileCoordPairs))]
		public void TilesWithSameIndexAndFlagsAreEqual(Tile3DCoord tile1, Tile3DCoord tile2)
		{
			Assert.That(tile1 == tile2, Is.True);
			Assert.That(tile1 != tile2, Is.False);
			Assert.That(tile1.Equals(tile2), Is.True);
			Assert.That(tile2.Equals(tile1), Is.True);
			Assert.That(tile1.Equals((Object)tile2), Is.True);
			Assert.That(tile2.Equals((Object)tile1), Is.True);
		}

		[Test] public void TilesWithDifferentCoordsHaveNonEqualHashcodes() => Assert.That(
			new Tile3DCoord(new GridCoord(-1, 0, 0), new Tile3D()).GetHashCode() !=
			new Tile3DCoord(new GridCoord(1, 0, 0), new Tile3D()).GetHashCode());

		[Test] public void TilesWithSameCoordsHaveEqualHashcodes() => Assert.That(
			new Tile3DCoord(new GridCoord(1, 1, 1), new Tile3D()).GetHashCode() ==
			new Tile3DCoord(new GridCoord(1, 1, 1), new Tile3D()).GetHashCode());
	}
}
