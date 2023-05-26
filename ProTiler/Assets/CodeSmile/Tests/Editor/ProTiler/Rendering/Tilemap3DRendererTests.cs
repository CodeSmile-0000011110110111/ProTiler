// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.ProTiler.Model;
using CodeSmile.ProTiler.Rendering;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.TestTools;
using ChunkSize = UnityEngine.Vector2Int;
using GridCoord = UnityEngine.Vector3Int;
using Object = UnityEngine.Object;

namespace CodeSmile.Tests.Editor.ProTiler.Rendering
{
	public class Tilemap3DRendererTests
	{
		[Test] public void TilemapRendererIsNotNull()
		{
			var renderer = CreateTilemapRenderer();

			Assert.That(renderer != null);
		}

		[Test] public void TilemapRendererHasDefaultCullingInstance()
		{
			var renderer = CreateTilemapRenderer();

			Assert.That(renderer.Culling is Tilemap3DFrustumCulling);
		}

		[Test] public void DefaultCullingReturnsFixedNumberOfTiles()
		{
			var renderer = CreateTilemapRenderer();
			var coords = renderer.Culling.GetVisibleCoords();

			Assert.That(coords.Count(), Is.EqualTo(100));
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
			renderer.TileAssetSet.Add(tileAsset);

			Assert.That(renderer.TileAssetSet[1] != null);
		}

		[Test] public void GetPrefabWithInvalidIndexReturnsMissingTile()
		{
			var renderer = CreateTilemapRenderer();

			Assert.That(renderer.TileAssetSet[1000], Is.EqualTo(Tile3DAssetBaseSet.LoadMissingTileAsset()));
		}

		[Test][CreateEmptyScene()]
		public void NewTilemapCreatedRendererPoolFolder()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();

			Assert.True(model.transform.Find(Tile3DRendererPool.RendererFolderName));
		}

		[Test][CreateEmptyScene()]
		public void DestroyRendererRemovesRendererPoolFolder()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var renderer = model.GetComponent<Tilemap3DRenderer>();

			Object.DestroyImmediate(renderer);

			Assert.False(model.transform.Find(Tile3DRendererPool.RendererFolderName));
		}

		private Tilemap3DRenderer CreateTilemapRenderer() =>
			Tilemap3DCreation.CreateRectangularTilemap3D().GetComponent<Tilemap3DRenderer>();
		private static Tilemap3D CreateTilemap() => new(new ChunkSize(10,10));

		private sealed class TestCulling10By10 : Tilemap3DCullingBase
		{
			public override IEnumerable<GridCoord> GetVisibleCoords()
			{
				const Int32 width = 10;
				const Int32 length = 10;
				var coords = new List<GridCoord>(width * length);

				for (var z = 0; z < length; z++)
					for (var x = 0; x < width; x++)
						coords.Add(new GridCoord(x, 0, z));

				return coords;
			}
		}
	}
}
