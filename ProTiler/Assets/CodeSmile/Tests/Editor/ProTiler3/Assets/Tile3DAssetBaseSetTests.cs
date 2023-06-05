// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler3.Editor.Creation;
using CodeSmile.ProTiler3.Runtime;
using CodeSmile.ProTiler3.Runtime.Assets;
using NUnit.Framework;
using System;

namespace CodeSmile.Tests.Editor.ProTiler3.Assets
{
	public class Tile3DAssetBaseSetTests
	{
		[Test] public void MissingTileIsLoaded()
		{
			var tileSet = new Tile3DAssetBaseSet();

			Assert.That(tileSet.MissingTileAsset != null);
			Assert.That(tileSet.MissingTileAsset ==
			            Tile3DAssetBaseSet.LoadTile3DAssetResource(Paths.ResourcesMissingTileAsset));
		}

		[Test] public void EmptyTileIsLoaded()
		{
			var tileSet = new Tile3DAssetBaseSet();

			Assert.That(tileSet.EmptyTileAsset != null);
			Assert.That(tileSet.EmptyTileAsset == Tile3DAssetBaseSet.LoadTile3DAssetResource(Paths.ResourcesEmptyTileAsset));
		}

		[Test] public void LoadNonExistingTileResourceThrows() =>
			Assert.Throws<ArgumentNullException>(() => Tile3DAssetBaseSet.LoadTile3DAssetResource("this path is invalid"));

		[Test] public void AddAndContainsTileAsset()
		{
			var tileSet = new Tile3DAssetBaseSet();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset);

			Assert.That(tileSet.Contains(tileAsset));
		}

		[Test] public void AddTileAssetWithIndex()
		{
			var tileSet = new Tile3DAssetBaseSet();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset, out var index);

			Assert.That(index > 0);
			Assert.That(tileSet.Contains(tileAsset));
		}

		[Test] public void RemoveAndDoesNotContainTileAsset()
		{
			var tileSet = new Tile3DAssetBaseSet();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset);
			tileSet.Remove(tileAsset);

			Assert.That(tileSet.Contains(tileAsset) == false);
		}
	}
}
