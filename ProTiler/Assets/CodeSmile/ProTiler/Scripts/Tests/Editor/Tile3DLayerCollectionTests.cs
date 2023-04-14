// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Collections;
using NUnit.Framework;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Tile3DLayerCollectionTests
	{
		[Test]
		public void SetTileTests()
		{
			/*
			var width = 10;
			var height = 20;
			var layer = new Tile3DDataCollection(width, height);
			Assert.NotNull(layer);
			Assert.AreEqual(width * height, layer.Capacity);
			Assert.AreEqual(0, layer.Count);

			Assert.Fail("reimplement");
			*/

			/*
			var coord = new Vector2Int(3, 7);
			var tileData = new Tile3DData { Tile = ScriptableObject.CreateInstance<Tile3D>(), PrefabSetIndex = 1,};
			layer[coord.x, coord.y] = tileData;
			Assert.AreEqual(1, layer.Count);
			Assert.AreEqual(tileData.Tile, layer[coord.x,coord.y].Tile);
			Assert.AreNotEqual(tileData.Tile, layer[coord.x + 1, coord.y - 1].Tile);

			Object.DestroyImmediate(layer[coord.x, coord.y].Tile);
		*/
		}
	}
}
