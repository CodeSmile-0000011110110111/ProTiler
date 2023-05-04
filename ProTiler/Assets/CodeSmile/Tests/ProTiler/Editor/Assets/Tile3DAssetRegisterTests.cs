// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Creation;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.ProTiler.Editor.Assets
{
	public class Tile3DAssetRegisterTests
	{
		[Test] public void PersistentAssetSingletonIsNotNull() => Assert.That(Tile3DAssetRegister.Singleton != null);

		[Test] public void IsEmptyAfterCreation()
		{
			var register = ScriptableObject.CreateInstance<Tile3DAssetRegister>();

			Assert.That(register.Count == 0);
		}

		[Test] public void CreatedWithPrefabHasPrefabAssigned()
		{
			var register = ScriptableObject.CreateInstance<Tile3DAssetRegister>();

			Assert.That(register.Count == 0);
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

		[Test] public void AddAndContainsTileAsset()
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

		[Test] public void RemoveAndDoesNotContainTileAsset()
		{
			var register = ScriptableObject.CreateInstance<Tile3DAssetRegister>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			register.Add(tileAsset);
			register.Remove(tileAsset);

			Assert.That(register.Contains(tileAsset) == false);
		}
	}
}
