// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;

namespace CodeSmile.Tests.ProTiler.Editor
{
	public class Tile3DSetTests
	{
		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Tile3DSet), typeof(Tile3DSet))]
		public void AddAndContainsTileAsset()
		{
			var tileSet = ObjectExt.FindObjectByTypeFast<Tile3DSet>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset);

			Assert.That(tileSet.Contains(tileAsset));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Tile3DSet), typeof(Tile3DSet))]
		public void AddTileAssetWithIndex()
		{
			var tileSet = ObjectExt.FindObjectByTypeFast<Tile3DSet>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset, out var index);

			Assert.That(index > 0);
			Assert.That(tileSet.Contains(tileAsset));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Tile3DSet), typeof(Tile3DSet))]
		public void RemoveAndDoesNotContainTileAsset()
		{
			var tileSet = ObjectExt.FindObjectByTypeFast<Tile3DSet>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset);
			tileSet.Remove(tileAsset);

			Assert.That(tileSet.Contains(tileAsset) == false);
		}
	}
}
