// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Editor.Creation;
using CodeSmile.ProTiler.Model;
using CodeSmile.ProTiler.Rendering;
using CodeSmile.Tests.Tools;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Runtime.ProTiler
{
	public class Tilemap3DRendererRuntimeTests
	{
		private static IEnumerable TestSource
		{
			get
			{
				yield return new TestCaseData(3, 3).Returns(null);
				//yield return new TestCaseData(4, 4).Returns(null);
				//yield return new TestCaseData(2, 2).Returns(null);
			}
		}

		[UnityTest] [CreateEmptyScene] [TestCaseSource(nameof(TestSource))]
		public IEnumerator CreateSameAmountOfTileRenderersAsCullingIndicates(int width, int length)
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var renderer = model.GetComponent<Tilemap3DRenderer>();
			renderer.Culling = new TestCulling(width, length);

			var rendererFolder = model.transform.Find(Tile3DRendererPoolFirstTry.RendererFolderName);
			Assert.That(rendererFolder != null);
			Assert.That(rendererFolder.childCount, Is.EqualTo(0));

			yield return null;

			Debug.Log(SceneManager.GetActiveScene().DumpAll());
			Assert.That(rendererFolder.childCount, Is.EqualTo(width*length));
		}

		[UnityTest] [CreateEmptyScene]
		public IEnumerator ClearTilemapEmptiesRendererFolder()
		{
			var model = Tilemap3DCreation.CreateRectangularTilemap3D();
			var renderer = model.GetComponent<Tilemap3DRenderer>();
			renderer.Culling = new TestCulling(3,3);
			model.SetTile(Vector3Int.one, new Tile3D(1));

			yield return null;

			var rendererFolder = model.transform.Find(Tile3DRendererPoolFirstTry.RendererFolderName);
			Assert.That(rendererFolder != null);
			Assert.That(rendererFolder.childCount, Is.GreaterThan(0));

			model.ClearTilemap();

			//yield return null;

			Debug.Log(SceneManager.GetActiveScene().DumpAll());
			rendererFolder = model.transform.Find(Tile3DRendererPoolFirstTry.RendererFolderName);
			Assert.That(rendererFolder != null);
			Assert.That(rendererFolder.childCount, Is.EqualTo(0));
		}
	}
}
