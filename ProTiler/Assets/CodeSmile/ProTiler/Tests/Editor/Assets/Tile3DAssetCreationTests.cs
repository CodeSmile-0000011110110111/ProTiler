// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.Tests.Utilities;
using NUnit.Framework;
using System;
using UnityEditor;

namespace CodeSmile.ProTiler.Tests.Editor.Assets
{
	public class Tile3DAssetCreationTests
	{
		[Test] public void CreateTile3DAssetInstance() =>
			Assert.That(Tile3DAssetCreation.CreateInstance<Tile3DAsset>() != null);

		[TestCase(TestPaths.TempTestAssets + "TestTile3DAsset.asset")]
		public void CreateTile3DAssetAtPath(string path)
		{
			var tileAsset = Tile3DAssetCreation.CreateAsset<Tile3DAsset>(path);

			Assert.That(tileAsset != null);
			Assert.That(AssetDatabase.LoadAssetAtPath<Tile3DAsset>(path) != null);
		}

		[Test] public void LoadEmptyTileInstance()
		{
			var empty = Tile3DAssetCreation.LoadEmptyTile();

			Assert.That(empty != null);
			Assert.That(empty.Prefab != null);
		}

		[Test] public void LoadMissingTileInstance()
		{
			var missing = Tile3DAssetCreation.LoadMissingTile();

			Assert.That(missing != null);
			Assert.That(missing.Prefab != null);
		}

		[Test] public void LoadNonExistingTileResourceThrows() =>
			Assert.Throws<ArgumentNullException>(() => Tile3DAssetCreation.LoadTile3DAssetResource("this path is invalid"));
	}
}
