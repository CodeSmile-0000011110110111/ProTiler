// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.Tests.Utilities;
using NUnit.Framework;
using System.IO;
using UnityEditor;

namespace CodeSmile.ProTiler.Tests.Editor.Assets
{
	public class Tile3DAssetCreationTests
	{
		[Test] public void CreateTile3DAssetInstance() =>
			Assert.That(Tile3DAssetCreation.CreateInstance<Tile3DAsset>() != null);

		[TestCase(TestPaths.TempTestAssets + "TestTile3DAsset.asset")]
		public void CreateTile3DAsset(string path)
		{
			var tileAsset = Tile3DAssetCreation.CreateAsset<Tile3DAsset>(path);

			Assert.That(tileAsset != null);
			Assert.That(AssetDatabase.LoadAssetAtPath<Tile3DAsset>(path) != null);
		}

		[TestCase(TestPaths.TempTestAssets + "TestTile3DAsset.asset")]
		public void CreateTile3DAssetUndo(string path)
		{
			Tile3DAssetCreation.CreateAsset<Tile3DAsset>(path);

			Undo.PerformUndo();

			Assert.That(File.Exists(path) == false);
			Assert.That(AssetDatabase.LoadAssetAtPath<Tile3DAsset>(path) == null);
		}

		[TestCase(TestPaths.TempTestAssets + "TestTile3DAsset.asset")]
		public void CreateTile3DAssetUndoRedo(string path)
		{
			Tile3DAssetCreation.CreateAsset<Tile3DAsset>(path);

			Undo.PerformUndo();
			Undo.PerformRedo();

			Assert.That(AssetDatabase.LoadAssetAtPath<Tile3DAsset>(path) != null);
		}
	}
}
