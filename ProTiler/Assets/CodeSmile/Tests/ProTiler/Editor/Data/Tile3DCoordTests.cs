// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.ProTiler.Editor.Data
{
	public class Tile3DCoordTests
	{
		private readonly Vector3Int m_TestCoord = new(1, 2, 3);
		private readonly Tile3D m_TestTile = new Tile3D(123, Tile3DFlags.DirectionEast | Tile3DFlags.FlipVertical);

		[Test] public void TileCreatedWithNewKeywordHasDefaultValues()
		{
			var tileCoord = new Tile3DCoord();

			Assert.That(tileCoord.Coord, Is.EqualTo(new Vector3Int()));
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
			Assert.That(tile1.Equals((object)tile2), Is.False);
			Assert.That(tile2.Equals((object)tile1), Is.False);
		}

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.EqualTileCoordPairs))]
		public void TilesWithSameIndexAndFlagsAreEqual(Tile3DCoord tile1, Tile3DCoord tile2)
		{
			Assert.That(tile1 == tile2, Is.True);
			Assert.That(tile1 != tile2, Is.False);
			Assert.That(tile1.Equals(tile2), Is.True);
			Assert.That(tile2.Equals(tile1), Is.True);
			Assert.That(tile1.Equals((object)tile2), Is.True);
			Assert.That(tile2.Equals((object)tile1), Is.True);
		}

		[Test] public void TilesWithDifferentCoordsHaveNonEqualHashcodes() => Assert.That(
			new Tile3DCoord(Vector3Int.left, new Tile3D()).GetHashCode() !=
			new Tile3DCoord(Vector3Int.right, new Tile3D()).GetHashCode());

		[Test] public void TilesWithSameCoordsHaveEqualHashcodes() => Assert.That(
			new Tile3DCoord(Vector3Int.one, new Tile3D()).GetHashCode() ==
			new Tile3DCoord(Vector3Int.one, new Tile3D()).GetHashCode());
	}
}
