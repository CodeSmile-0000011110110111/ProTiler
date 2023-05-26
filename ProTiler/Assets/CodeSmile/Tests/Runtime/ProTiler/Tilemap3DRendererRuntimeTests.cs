// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.ProTiler.Model;
using CodeSmile.ProTiler.Rendering;
using CodeSmile.Tests.Tools;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.Tests.Runtime.ProTiler
{
	public class Tilemap3DRendererRuntimeTests
	{
		[UnityTest] [CreateEmptyScene]
		public IEnumerator ClearTilemapEmptiesRendererPoolFolder()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var renderer = model.GetComponent<Tilemap3DRenderer>();
			renderer.Culling = new TestCulling3By3();

			yield return null;

			model.SetTile(Vector3Int.one, new Tile3D(1));

			yield return null;

			Debug.Log(SceneManager.GetActiveScene().DumpAll());
			Debug.Log(model.transform.Find(Tile3DRendererPool.RendererFolderName).DumpAll());

			Assert.That(model.transform.Find(Tile3DRendererPool.RendererFolderName).childCount, Is.EqualTo(1));

			model.ClearTilemap(new Vector2Int(10, 10));

			yield return null;

			Assert.That(model.transform.Find(Tile3DRendererPool.RendererFolderName).childCount, Is.EqualTo(0));
		}

		private sealed class TestCulling3By3 : Tilemap3DCullingBase
		{
			public override IEnumerable<GridCoord> GetVisibleCoords()
			{
				const Int32 width = 3;
				const Int32 length = 3;
				var coords = new List<GridCoord>(width * length);

				for (var z = 0; z < length; z++)
					for (var x = 0; x < width; x++)
						coords.Add(new GridCoord(x, 0, z));

				return coords;
			}
		}
	}
}
