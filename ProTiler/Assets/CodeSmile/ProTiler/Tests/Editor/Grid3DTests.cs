// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Tests.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler.Tests
{
	public class Grid3DTests
	{
		[Test] [EmptyScene] [CreateGameObject(nameof(Grid3D), typeof(Grid3D))]
		public void GetSetCellLayout()
		{
			var grid = Object.FindObjectOfType<Grid3D>();
			var cellLayout = CellLayout.Hexagonal;

			grid.CellLayout = cellLayout;

			Assert.AreEqual(cellLayout, grid.CellLayout);
		}

		[Test] [EmptyScene] [CreateGameObject(nameof(Grid3D), typeof(Grid3D))]
		public void GetSetCellSize()
		{
			var grid = Object.FindObjectOfType<Grid3D>();
			var cellSize = new Vector3Int(3, 4, 7);

			grid.CellSize = cellSize;

			Assert.AreEqual(cellSize, grid.CellSize);
		}

		[Test] [EmptyScene] [CreateGameObject(nameof(Grid3D), typeof(Grid3D))]
		public void GetSetCellGap()
		{
			var grid = Object.FindObjectOfType<Grid3D>();
			var cellGap = new Vector3(1.1f, 2.2f, 3.3f);

			grid.CellGap = cellGap;

			Assert.AreEqual(cellGap, grid.CellGap);
		}
	}
}
