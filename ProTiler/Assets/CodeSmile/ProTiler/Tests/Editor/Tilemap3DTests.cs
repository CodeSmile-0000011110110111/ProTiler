// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Tests.Utilities;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Tilemap3DTests
	{
		private Tilemap3D m_Tilemap;

		[SetUp]
		public void SetUp()
		{
			m_Tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			Assert.NotNull(m_Tilemap);
		}

		[TearDown]
		public void TearDown()
		{
			if (m_Tilemap != null && m_Tilemap.gameObject.IsMissing() == false)
				m_Tilemap.gameObject.DestroyInAnyMode();
		}

		[Test]
		[LoadScene(Defines.UnitTestScene)]
		public void TilemapCreationUndoRedo()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			Assert.NotNull(tilemap);
			Assert.NotNull(Object.FindObjectOfType<Grid3D>());
			Assert.NotNull(Object.FindObjectOfType<Tilemap3D>());
			Assert.Contains(tilemap.Grid.gameObject, SceneManager.GetActiveScene().GetRootGameObjects());

			tilemap = null; // reference goes missing upon Undo
			Undo.PerformUndo();
			Assert.Null(Object.FindObjectOfType<Grid3D>());
			Assert.Null(Object.FindObjectOfType<Tilemap3D>());

			Undo.PerformRedo();
			Assert.NotNull(Object.FindObjectOfType<Grid3D>());
			Assert.NotNull(Object.FindObjectOfType<Tilemap3D>());
		}

		[Test]
		[LoadScene(Defines.UnitTestScene)]
		public void TilemapCreation()
		{
			var tilemap = Tilemap3DCreation.CreateRectangularTilemap3D();
			Assert.NotNull(tilemap);
			Assert.NotNull(tilemap.Grid);
			Assert.NotNull(tilemap.Chunks);
			Assert.AreEqual(0, tilemap.Chunks.Count);
		}
	}
}
