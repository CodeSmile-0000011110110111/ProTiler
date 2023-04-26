// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Creation;
using NUnit.Framework;

namespace CodeSmile.ProTiler.Tests.Editor.Assets
{
	public class Tile3DAssetCreationTests
	{
		[Test] public void CreateTile3DAsset() => Assert.That(Tile3DAssetCreation.CreateInstance<Tile3DAsset>() != null);
		[Test] public void CreateTile3DAssetAtPath()
		{
			Assert.That(Tile3DAssetCreation.CreateInstance<Tile3DAsset>() != null);
			Assert.Fail();
		}
	}
}
