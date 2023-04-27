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
		[Test] public void SingletonIsNotNull() => Assert.That(Tile3DAssetRegister.Singleton != null);

		[Test] public void MissingTileIsLoaded()
		{
			var register = ScriptableObject.CreateInstance<Tile3DAssetRegister>();

			Assert.That(register.MissingTileAsset != null);
			Assert.That(register.MissingTileAsset == Tile3DAssetCreation.LoadMissingTile());
		}

		[Test] public void EmptyTileIsLoaded()
		{
			var register = ScriptableObject.CreateInstance<Tile3DAssetRegister>();

			Assert.That(register[0] == register.EmptyTileAsset);
			Assert.That(register.EmptyTileAsset == Tile3DAssetCreation.LoadEmptyTile());
		}

		[TestCase(0)] [TestCase(-1)] [TestCase(int.MinValue)]
		public void ZeroOrNegativeIndexReturnsEmptyTile(int index)
		{
			var register = ScriptableObject.CreateInstance<Tile3DAssetRegister>();

			Assert.That(register[index] != null);
			Assert.That(register[index] == register.EmptyTileAsset);
		}

		[TestCase(1)] [TestCase(int.MaxValue)]
		public void InvalidIndexReturnsMissingTile(int index)
		{
			var register = ScriptableObject.CreateInstance<Tile3DAssetRegister>();

			Assert.That(register[index] != null);
			Assert.That(register[index] == register.MissingTileAsset);
		}

		[Test] public void AddTileAsset()
		{
			var register = ScriptableObject.CreateInstance<Tile3DAssetRegister>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			register.Add(tileAsset);

			Assert.That(register.Contains(tileAsset));
		}


		[Test] public void AddTileAssetWithIndex()
		{
			var register = ScriptableObject.CreateInstance<Tile3DAssetRegister>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			register.Add(tileAsset, out var index);

			Assert.That(index > 0);
			Assert.That(register.Contains(tileAsset));
		}

		[Test] public void RemoveTileAsset()
		{
			var register = ScriptableObject.CreateInstance<Tile3DAssetRegister>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			register.Add(tileAsset);
			register.Remove(tileAsset);

			Assert.That(register.Contains(tileAsset) == false);
		}
	}
}
