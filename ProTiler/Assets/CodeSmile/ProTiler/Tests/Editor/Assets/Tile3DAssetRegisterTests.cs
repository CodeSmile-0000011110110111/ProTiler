// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Creation;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.ProTiler.Tests.Editor.Assets
{
	public class Tile3DAssetRegisterTests
	{
		[Test] public void VerifySingletonNotNull() => Assert.That(Tile3DAssetRegister.Singleton != null);

		[Test] public void VerifyRegisterContainsMissingTile()
		{
			var register = Tile3DAssetRegister.Singleton;

			Assert.That(register.MissingTileAsset != null);
			Assert.That(register.MissingTileAsset == Tile3DAssetCreation.LoadMissingTile());
		}

		[Test] public void VerifyRegisterContainsEmptyTile()
		{
			var register = Tile3DAssetRegister.Singleton;

			Assert.That(register[0] == register.EmptyTileAsset);
			Assert.That(register.EmptyTileAsset == Tile3DAssetCreation.LoadEmptyTile());
		}

		[Test] public void AddTileAsset()
		{
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			Tile3DAssetRegister.Singleton.Add(tileAsset);

			Assert.That(Tile3DAssetRegister.Singleton.Contains(tileAsset));
		}


		[Test] public void AddTileAssetWithIndex()
		{
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			Tile3DAssetRegister.Singleton.Add(tileAsset, out var index);

			Assert.That(index > 0);
			Assert.That(Tile3DAssetRegister.Singleton.Contains(tileAsset));
		}

		[Test] public void RemoveTileAsset()
		{
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			Tile3DAssetRegister.Singleton.Add(tileAsset);
			Tile3DAssetRegister.Singleton.Remove(tileAsset);

			Assert.That(Tile3DAssetRegister.Singleton.Contains(tileAsset) == false);
		}
	}
}
