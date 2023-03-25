// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tile;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
public class TileDataTests
{
	[Test]
	public void TileDataCtorTests()
	{
		var tileData = new TileData(1, TileFlags.AllDirections | TileFlags.AllFlips);
		Assert.IsTrue(tileData.IsValid);
		Assert.AreEqual(1, tileData.TileSetIndex);
		Assert.AreEqual(TileFlags.AllDirections | TileFlags.AllFlips, tileData.Flags);

		tileData = new TileData(2);
		Assert.AreEqual(TileFlags.DirectionNorth, tileData.Flags);

		tileData = new TileData();
		Assert.AreEqual(0, tileData.TileSetIndex);
		Assert.AreEqual(TileFlags.None, tileData.Flags);

		tileData = new TileData(Const.InvalidTileSetIndex);
		Assert.IsTrue(tileData.IsInvalid);

		Assert.IsNotNull(tileData.ToString());
		Assert.IsNotEmpty(tileData.ToString());
	}

	[Test]
	public void TileDataEqualsTests()
	{
		var tileData1 = new TileData(1, TileFlags.AllDirections);
		var tileData2 = new TileData(2, TileFlags.AllFlips);
		Assert.AreNotSame(tileData1, tileData1);
		Assert.AreNotSame(tileData2, tileData2);
		Assert.AreNotSame(tileData1, tileData2);

		Assert.IsTrue(tileData1.Equals(tileData1));
		Assert.IsTrue(tileData2.Equals(tileData2));
		Assert.IsFalse(tileData1.Equals(tileData2));
		Assert.IsFalse(tileData2.Equals(tileData1));
		Assert.IsTrue(tileData1.Equals((object)tileData1));
		Assert.IsTrue(tileData2.Equals((object)tileData2));
		Assert.IsFalse(tileData1.Equals((object)tileData2));
		Assert.IsFalse(tileData2.Equals((object)tileData1));
		Assert.IsTrue(tileData1 == tileData1);
		Assert.IsTrue(tileData2 == tileData2);
		Assert.IsTrue(tileData1 != tileData2);
		Assert.IsTrue(tileData2 != tileData1);
		Assert.IsTrue(tileData1 == new TileData(tileData1));
		Assert.IsTrue(tileData2 == new TileData(tileData2));

		Assert.AreEqual(tileData1, tileData1);
		Assert.AreEqual(tileData2, tileData2);
		Assert.AreNotEqual(tileData1, tileData2);

		Assert.AreEqual(tileData1, new TileData(1, TileFlags.AllDirections));
		Assert.AreEqual(tileData2, new TileData(2, TileFlags.AllFlips));
		Assert.IsTrue(tileData1.Equals(new TileData(1, TileFlags.AllDirections)));
		Assert.IsTrue(tileData2.Equals(new TileData(2, TileFlags.AllFlips)));

		Assert.AreEqual(tileData1.GetHashCode(), new TileData(1, TileFlags.AllDirections).GetHashCode());
		Assert.AreEqual(tileData2.GetHashCode(), new TileData(2, TileFlags.AllFlips).GetHashCode());
	}

	[Test]
	public void TileDataIndexTests()
	{
		var tileData = new TileData(1);
		Assert.AreEqual(1, tileData.TileSetIndex);
		tileData.TileSetIndex = Const.InvalidTileSetIndex;
		Assert.AreEqual(Const.InvalidTileSetIndex, tileData.TileSetIndex);
		tileData.TileSetIndex = int.MaxValue;
		Assert.AreEqual(int.MaxValue, tileData.TileSetIndex);
	}
	
	[Test]
	public void TileDataSetFlagsTests()
	{
		var tileData = new TileData(1);
		var flags = tileData.SetFlags(TileFlags.AllFlips);
		Assert.AreEqual(TileFlags.AllFlips | TileFlags.DirectionNorth, flags);
		Assert.AreEqual(TileFlags.AllFlips | TileFlags.DirectionNorth, tileData.Flags);
		flags = tileData.SetFlags(TileFlags.AllDirections);
		Assert.AreEqual(TileFlags.AllFlips | TileFlags.AllDirections, flags);
		Assert.AreEqual(TileFlags.AllFlips | TileFlags.AllDirections, tileData.Flags);
	}

	[Test]
	public void TileDataClearFlagsTests()
	{
		var tileData = new TileData(1, TileFlags.AllDirections | TileFlags.AllFlips);
		Assert.AreEqual(TileFlags.AllDirections | TileFlags.AllFlips, tileData.Flags);
		var flags = tileData.ClearFlags(TileFlags.AllFlips);
		Assert.AreEqual(TileFlags.AllDirections, flags);
		Assert.AreEqual(TileFlags.AllDirections, tileData.Flags);
		flags = tileData.ClearFlags(TileFlags.DirectionNorth | TileFlags.DirectionSouth);
		Assert.AreEqual(TileFlags.DirectionEast | TileFlags.DirectionWest, flags);
		Assert.AreEqual(TileFlags.DirectionEast | TileFlags.DirectionWest, tileData.Flags);
		flags = tileData.ClearFlags(TileFlags.DirectionEast | TileFlags.DirectionWest);
		Assert.AreEqual(TileFlags.None, flags);
		Assert.AreEqual(TileFlags.None, tileData.Flags);
	}

	[Test]
	public void TileDataRotateTests()
	{
		var tileData = new TileData(0);
		Assert.AreEqual(TileFlags.DirectionNorth, tileData.Flags);

		tileData.Flags = TileFlags.None;
		tileData.Rotate(0);
		tileData.Rotate(0);
		Assert.AreEqual(TileFlags.DirectionSouth, tileData.Flags);
		tileData.Rotate(0);
		tileData.Rotate(0);
		Assert.AreEqual(TileFlags.DirectionNorth, tileData.Flags);
		tileData.Rotate(-1);
		tileData.Rotate(-1);
		Assert.AreEqual(TileFlags.DirectionSouth, tileData.Flags);
		tileData.Rotate(-1);
		tileData.Rotate(-1);
		Assert.AreEqual(TileFlags.DirectionNorth, tileData.Flags);
		tileData.Rotate(-1);
		Assert.AreEqual(TileFlags.DirectionWest, tileData.Flags);
		tileData.Rotate(0);
		tileData.Rotate(0);
		Assert.AreEqual(TileFlags.DirectionEast, tileData.Flags);
	}

	[Test]
	public void TileDataFlipTests()
	{
		var tileData = new TileData(0);
		tileData.Flags = TileFlags.None;

		tileData.Flip(0);
		Assert.AreEqual(TileFlags.FlipVertical, tileData.Flags);
		tileData.Flip(0);
		Assert.AreEqual(TileFlags.FlipVertical | TileFlags.FlipHorizontal, tileData.Flags);
		tileData.Flip(0);
		Assert.AreEqual(TileFlags.FlipHorizontal, tileData.Flags);
		tileData.Flip(0);
		Assert.AreEqual(TileFlags.None, tileData.Flags);

		tileData.Flip(-1);
		Assert.AreEqual(TileFlags.FlipHorizontal, tileData.Flags);
		tileData.Flip(-1);
		Assert.AreEqual(TileFlags.FlipVertical | TileFlags.FlipHorizontal, tileData.Flags);
		tileData.Flip(-1);
		Assert.AreEqual(TileFlags.FlipVertical, tileData.Flags);
		tileData.Flip(-1);
		Assert.AreEqual(TileFlags.None, tileData.Flags);
	}
}