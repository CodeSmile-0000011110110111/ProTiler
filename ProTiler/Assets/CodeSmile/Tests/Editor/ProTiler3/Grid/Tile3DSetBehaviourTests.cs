// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.ProTiler.Rendering;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;

namespace CodeSmile.Tests.Editor.ProTiler3.Grid
{
	public class Tile3DSetBehaviourTests
	{
		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Tile3DAssetSet), typeof(Tile3DAssetSet))]
		public void AddAndContainsTileAsset()
		{
			var tileSet = ObjectExt.FindObjectByTypeFast<Tile3DAssetSet>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset);

			Assert.That(tileSet.Contains(tileAsset));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Tile3DAssetSet), typeof(Tile3DAssetSet))]
		public void AddTileAssetWithIndex()
		{
			var tileSet = ObjectExt.FindObjectByTypeFast<Tile3DAssetSet>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset, out var index);

			Assert.That(index > 0);
			Assert.That(tileSet.Contains(tileAsset));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Tile3DAssetSet), typeof(Tile3DAssetSet))]
		public void RemoveAndDoesNotContainTileAsset()
		{
			var tileSet = ObjectExt.FindObjectByTypeFast<Tile3DAssetSet>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset);
			tileSet.Remove(tileAsset);

			Assert.That(tileSet.Contains(tileAsset) == false);
		}
	}
}
