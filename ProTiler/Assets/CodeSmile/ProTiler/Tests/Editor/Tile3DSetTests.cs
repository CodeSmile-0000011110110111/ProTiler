// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.Tests.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.ProTiler.Tests.Editor
{
	public class Tile3DSetTests
	{
		[Test] [EmptyScene] [CreateGameObject(nameof(Tile3DSet), typeof(Tile3DSet))]
		public void AddAndContainsTileAsset()
		{
			var tileSet = Object.FindObjectOfType<Tile3DSet>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset);

			Assert.That(tileSet.Contains(tileAsset));
		}

		[Test] [EmptyScene] [CreateGameObject(nameof(Tile3DSet), typeof(Tile3DSet))]
		public void AddTileAssetWithIndex()
		{
			var tileSet = Object.FindObjectOfType<Tile3DSet>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset, out var index);

			Assert.That(index > 0);
			Assert.That(tileSet.Contains(tileAsset));
		}

		[Test] [EmptyScene] [CreateGameObject(nameof(Tile3DSet), typeof(Tile3DSet))]
		public void RemoveAndDoesNotContainTileAsset()
		{
			var tileSet = Object.FindObjectOfType<Tile3DSet>();
			var tileAsset = Tile3DAssetCreation.CreateInstance<Tile3DAsset>();

			tileSet.Add(tileAsset);
			tileSet.Remove(tileAsset);

			Assert.That(tileSet.Contains(tileAsset) == false);
		}
	}
}
