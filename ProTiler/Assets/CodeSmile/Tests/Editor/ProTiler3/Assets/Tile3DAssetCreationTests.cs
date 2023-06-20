// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler3.Assets;
using CodeSmile.ProTiler3.Editor.Creation;
using CodeSmile.Tests.Tools;
using NUnit.Framework;
using UnityEditor;

namespace CodeSmile.Tests.Editor.ProTiler3.Assets
{
	public class Tile3DAssetCreationTests
	{
		[Test] public void CreateTile3DAssetInstance() =>
			Assert.That(Tile3DAssetCreation.CreateInstance<Tile3DAsset>() != null);

		[Test] public void CreateTile3DAssetInstanceWithPrefab()
		{
			var prefab = TestAssets.LoadTestPrefab();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>(prefab);

			Assert.That(prefab.IsPrefab());
			Assert.That(tileAsset.Prefab != null);
			Assert.That(tileAsset.Prefab, Is.EqualTo(prefab));
		}

		[TestCase(TestPaths.TempTestAssets + "TestTile3DAsset.asset")]
		public void CreateRegisteredTile3DAssetAtPath(string path)
		{
			var tileAsset = Tile3DAssetCreation.CreateRegisteredAsset<Tile3DAsset>(path);

			Assert.That(tileAsset != null);
			Assert.That(AssetDatabase.LoadAssetAtPath<Tile3DAsset>(path) != null);
			Assert.That(Tile3DAssetRegister.Singleton.Contains(tileAsset));

			Tile3DAssetRegister.Singleton.Remove(tileAsset);
		}

		/*[TestCase(TestPaths.TempTestAssets + "TestTile3DAsset.asset")]
		public void CreateRegisteredTile3DAssetWithSelection(string path)
		{
			Tile3DAssetBase tileAsset = null;
			Tile3DAssetCreation.CreateRegisteredAssetWithSelection<Tile3DAsset>("TestTile3DAsset",
				(createdTileAsset =>
				{
					tileAsset = createdTileAsset;
				}));

			Assert.That(tileAsset != null);
			Assert.That(Selection.activeObject == tileAsset);
			Assert.That(AssetDatabase.LoadAssetAtPath<Tile3DAsset>(path) != null);
			Assert.That(Tile3DAssetRegister.Singleton.Contains(tileAsset));

			Tile3DAssetRegister.Singleton.Remove(tileAsset);
			AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(tileAsset));
		}*/

	}
}
