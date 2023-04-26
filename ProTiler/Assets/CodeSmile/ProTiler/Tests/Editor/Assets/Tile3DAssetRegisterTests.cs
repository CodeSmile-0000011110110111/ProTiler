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
		[Test] public void EnsureTile3DRegisterSingletonNotNull() => Assert.That(Tile3DAssetRegister.Singleton != null);

		[Test] public void AddTileAsset()
		{
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			Tile3DAssetRegister.Singleton.Add(tileAsset);
		}
	}
}
