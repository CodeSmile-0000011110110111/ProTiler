// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Model;
using NUnit.Framework;
using System;
using System.Runtime.InteropServices;
using Unity.Serialization.Json;
using UnityEngine;

namespace CodeSmile.Tests.Editor.ProTiler.Model
{
	public class Tile3DTests
	{
		[Test] public void SizeDidNotChangeUnintentionally()
		{
			var sizeInBytes = Marshal.SizeOf(typeof(Tile3D));
			Debug.Log($"Size of {nameof(Tile3D)}: {sizeInBytes} bytes");

			Assert.That(sizeInBytes, Is.EqualTo(4));
		}

		[Test] public void JsonDidNotChangeUnintentionally()
		{
			var tile = new Tile3D(ushort.MaxValue, (Tile3DFlags)byte.MaxValue);

			var json = JsonSerialization.ToJson(tile);
			Debug.Log(json);
			Debug.Log($"({json.Length} bytes)");

			Assert.That(json, Is.EqualTo("{\n    \"Index\": 65535,\n    \"Flags\": 255\n}"));
			Assert.That(json.Length, Is.EqualTo(40));
		}

		[Test] public void MinifiedJsonDidNotChangeUnintentionally()
		{
			var tile = new Tile3D(ushort.MaxValue, (Tile3DFlags)byte.MaxValue);

			var json = JsonSerialization.ToJson(tile,
				new JsonSerializationParameters { Minified = true, Simplified = true });
			Debug.Log(json);
			Debug.Log($"({json.Length} bytes)");

			Assert.That(json, Is.EqualTo("{Index=65535 Flags=255}"));
			Assert.That(json.Length, Is.EqualTo(23));
		}

		[Test] public void TileCreatedWithNewKeywordIsEmpty() => Assert.That(new Tile3D().IsEmpty, Is.True);

		[Test] public void TileCreatedWithNewKeywordHasNoFlags() =>
			Assert.That(new Tile3D().Flags, Is.EqualTo(Tile3DFlags.None));

		[Test] public void NewTileIsEmpty() => Assert.That(new Tile3D().IsEmpty, Is.True);

		[Test] public void NewTileWithNegativeIndexThrows() => Assert.Throws<ArgumentException>(
			() => { new Tile3D(-1); });

		[Test] public void NewTileWithInt16MaxValueIndexIsAllowed() => Assert.DoesNotThrow(
			() => { new Tile3D(ushort.MaxValue); });

		[Test] public void NewTileWithIndexGreaterThanInt16Throws() => Assert.Throws<ArgumentException>(
			() => { new Tile3D(ushort.MaxValue + 1); });

		[Test] public void NewTileHasDefaultDirectionNorth() =>
			Assert.That(new Tile3D().Direction == Tile3DFlags.DirectionNorth);

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.EmptyIndexes))]
		public void NewTileWithZeroIndexIsEmpty(int index) => Assert.That(new Tile3D(index).IsEmpty, Is.True);

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.NonEmptyIndexes))]
		public void NewTileWithNonZeroIndexIsNotEmpty(int index) => Assert.That(new Tile3D(index).IsEmpty, Is.False);

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.ValidIndexes))]
		public void NewTileWithIndexReturnsIndex(int index) => Assert.That(new Tile3D(index).Index == index);

		[TestCaseSource(typeof(Tile3DTestCaseSource), nameof(Tile3DTestCaseSource.ValidIndexesWithFlags))]
		public void NewTileWithIndexAndFlagsReturnsBothUnaltered(int index, Tile3DFlags flags)
		{
			Assert.That(new Tile3D(index, flags).Index == index);
			Assert.That(new Tile3D(index, flags).Flags == flags);
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
