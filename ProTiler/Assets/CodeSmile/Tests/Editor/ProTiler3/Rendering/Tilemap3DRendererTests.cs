// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler3.Editor.Creation;
using CodeSmile.ProTiler3.Runtime.Assets;
using CodeSmile.ProTiler3.Runtime.Rendering;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System.Linq;
using UnityEngine;
using ChunkSize = Unity.Mathematics.int2;

namespace CodeSmile.Tests.Editor.ProTiler3.Rendering
{
	public class Tilemap3DRendererTests
	{
		[Test] public void TilemapRendererIsNotNull()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();

			Assert.That(model.GetComponent<Tilemap3DRenderer>() != null);
		}

		[Test] public void TilemapRendererHasDefaultCullingInstance()
		{
			var renderer = CreateTilemapRenderer();

			Assert.That(renderer.Culling is Tilemap3DTopDownCulling);
		}

		[Test] public void DefaultCullingReturnsNonZeroVisibleCoords()
		{
			var renderer = CreateTilemapRenderer();
			var coords = renderer.Culling.GetVisibleCoords(new ChunkSize(16, 16), Vector3.one);

			Assert.That(coords.Count(), Is.GreaterThan(0));
		}

		[Test] public void GetPrefabWithIndexZeroReturnsEmptyTile()
		{
			var renderer = CreateTilemapRenderer();

			Assert.That(renderer.TileAssetSet[0], Is.EqualTo(Tile3DAssetBaseSet.LoadEmptyTileAsset()));
		}

		[Test] public void GetPrefabWithIndexOneReturnsTile()
		{
			var renderer = CreateTilemapRenderer();

			var tileAsset = ScriptableObject.CreateInstance<Tile3DAsset>();
			(renderer.TileAssetSet as Tile3DAssetSet).Add(tileAsset);

			Assert.That(renderer.TileAssetSet[1] != null);
		}

		[Test] public void GetPrefabWithInvalidIndexReturnsMissingTile()
		{
			var renderer = CreateTilemapRenderer();

			Assert.That(renderer.TileAssetSet[1000], Is.EqualTo(Tile3DAssetBaseSet.LoadMissingTileAsset()));
		}

		[Test] [CreateEmptyScene]
		public void DestroyRendererEmptiesActiveRenderersFolder()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var renderer = model.GetComponent<Tilemap3DRenderer>();

			renderer.DestroyInAnyMode();

			var folder = model.transform.Find(Tilemap3DRenderer.ActiveRenderersFolderName);
			Assert.NotNull(folder);
			Assert.That(folder.transform.childCount, Is.Zero);
		}

		[Test] [CreateEmptyScene]
		public void DestroyRendererEmptiesPooledRenderersFolder()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var renderer = model.GetComponent<Tilemap3DRenderer>();

			renderer.DestroyInAnyMode();

			var folder = model.transform.Find(Tilemap3DRenderer.PooledRenderersFolderName);
			Assert.NotNull(folder);
			Assert.That(folder.transform.childCount, Is.Zero);
		}

		private Tilemap3DRenderer CreateTilemapRenderer() =>
			Tilemap3DCreation.CreateRectangularTilemap3D().GetComponent<Tilemap3DRenderer>();
	}
}
