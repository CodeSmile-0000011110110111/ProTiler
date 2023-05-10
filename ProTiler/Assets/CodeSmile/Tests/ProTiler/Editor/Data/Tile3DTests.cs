// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using NUnit.Framework;
using System.Runtime.InteropServices;
using Unity.Serialization.Json;
using UnityEngine;

namespace CodeSmile.Tests.ProTiler.Editor.Data
{
	public class Tile3DTests
	{
		[Test] public void AssertThatTile3DSizeDidNotChangeWithoutIntent()
		{
			var sizeInBytes = Marshal.SizeOf(typeof(Tile3D));
			Debug.Log($"Size of Tile3D: {Marshal.SizeOf(typeof(Tile3D))} bytes");

			Assert.That(sizeInBytes == 4);
		}

		[Test] public void AssertThatJsonSerializedStringDidNotChangeWithoutIntent()
		{
			var tile = new Tile3D(short.MaxValue, (Tile3DFlags)short.MaxValue);

			var json = JsonSerialization.ToJson(tile);
			Debug.Log(json);
			Debug.Log($"({json.Length} bytes)");

			Assert.That(json, Is.EqualTo("{\n    \"Index\": 32767,\n    \"Flags\": 32767\n}"));
			Assert.That(json.Length == 42);
		}

		[Test] public void AssertThatJsonMinifiedSerializedStringDidNotChangeWithoutIntent()
		{
			var tile = new Tile3D(short.MaxValue, (Tile3DFlags)short.MaxValue);

			var json = JsonSerialization.ToJson(tile,
				new JsonSerializationParameters { Minified = true, Simplified = true });
			Debug.Log(json);
			Debug.Log($"({json.Length} bytes)");

			Assert.That(json, Is.EqualTo("{Index=32767 Flags=32767}"));
			Assert.That(json.Length == 25);
		}

		[Test] public void TileCreatedWithNewKeywordIsEmpty() => Assert.That(new Tile3D().IsEmpty, Is.True);
		[Test] public void TileCreatedWithNewKeywordIsValid() => Assert.That(new Tile3D().IsValid, Is.True);

		[Test] public void TileCreatedWithNewKeywordHasNoFlags() =>
			Assert.That(new Tile3D().Flags, Is.EqualTo(Tile3DFlags.None));

		[Test] public void NewTileIsEmpty() => Assert.That(new Tile3D().IsEmpty, Is.True);
		[Test] public void NewTileIsValid() => Assert.That(new Tile3D().IsValid, Is.True);

		[Test] public void NewTileHasDefaultDirectionNorth() =>
			Assert.That(new Tile3D().Direction == Tile3DFlags.DirectionNorth);

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.EmptyIndexes))]
		public void NewTileWithNegativeOrZeroIndexIsEmpty(int index) =>
			Assert.That(new Tile3D((short)index).IsEmpty, Is.True);

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.NonEmptyIndexes))]
		public void NewTileWithNonZeroIndexIsNotEmpty(int index) =>
			Assert.That(new Tile3D((short)index).IsEmpty, Is.False);

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.ValidIndexes))]
		public void NewTileWithZeroOrPositiveIndexIsValid(int index) =>
			Assert.That(new Tile3D((short)index).IsValid, Is.True);

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.InvalidIndexes))]
		public void NewTileWithNegativeIndexIsNotValid(int index) =>
			Assert.That(new Tile3D((short)index).IsValid, Is.False);

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.InvalidIndexes))]
		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.ValidIndexes))]
		public void NewTileWithIndexReturnsIndex(int index) => Assert.That(new Tile3D((short)index).Index == index);

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.ValidIndexesWithFlags))]
		public void NewTileWithIndexAndFlagsReturnsBothUnaltered(int index, Tile3DFlags flags)
		{
			Assert.That(new Tile3D((short)index, flags).Index == index);
			Assert.That(new Tile3D((short)index, flags).Flags == flags);
		}

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.DirectionFlags))]
		public void NewTileWithDirectionFlagsReturnsDirectionUnaltered(Tile3DFlags flags) =>
			Assert.That(new Tile3D(123, flags).Direction == flags);

		[TestCase(Tile3DFlags.None)]
		public void NewTileWithDirectionNoneReturnsDirectionNorth(Tile3DFlags flags) =>
			Assert.That(new Tile3D(234, flags).Direction == Tile3DFlags.DirectionNorth);

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.NonEqualTilePairs))]
		public void TilesWithDifferentIndexAndSameFlagsAreNotEqual(Tile3D tile1, Tile3D tile2)
		{
			Assert.That(tile1 == tile2, Is.False);
			Assert.That(tile1 != tile2, Is.True);
			Assert.That(tile1.Equals(tile2), Is.False);
			Assert.That(tile2.Equals(tile1), Is.False);
			Assert.That(tile1.Equals((object)tile2), Is.False);
			Assert.That(tile2.Equals((object)tile1), Is.False);
		}

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.EqualTilePairs))]
		public void TilesWithSameIndexAndFlagsAreEqual(Tile3D tile1, Tile3D tile2)
		{
			Assert.That(tile1 == tile2, Is.True);
			Assert.That(tile1 != tile2, Is.False);
			Assert.That(tile1.Equals(tile2), Is.True);
			Assert.That(tile2.Equals(tile1), Is.True);
			Assert.That(tile1.Equals((object)tile2), Is.True);
			Assert.That(tile2.Equals((object)tile1), Is.True);
		}

		[Test] public void TilesWithDifferentIndexHaveNonEqualHashcodes() =>
			Assert.That(new Tile3D(2).GetHashCode() != new Tile3D(13).GetHashCode());

		[Test] public void TilesWithSameIndexAndDifferentFlagsHaveNonEqualHashcodes() => Assert.That(
			new Tile3D(2, Tile3DFlags.FlipHorizontal).GetHashCode() !=
			new Tile3D(2, Tile3DFlags.FlipVertical).GetHashCode());

		[Test] public void TilesWithSameIndexAndFlagsHaveEqualHashcodes() => Assert.That(
			new Tile3D(2, Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal).GetHashCode() ==
			new Tile3D(2, Tile3DFlags.DirectionWest | Tile3DFlags.FlipHorizontal).GetHashCode());
	}
}
