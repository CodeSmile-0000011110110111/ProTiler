// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.ProTiler.Editor
{
	public class Grid3DTests
	{
		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Grid3D), typeof(Grid3D))]
		public void GetSetCellLayout()
		{
			var grid = ObjectExt.FindObjectByTypeFast<Grid3D>();
			var cellLayout = CellLayout.Hexagonal;

			grid.CellLayout = cellLayout;

			Assert.That(cellLayout, Is.EqualTo(grid.CellLayout));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Grid3D), typeof(Grid3D))]
		public void GetSetCellSize()
		{
			var grid = ObjectExt.FindObjectByTypeFast<Grid3D>();
			var cellSize = new Vector3Int(3, 4, 7);

			grid.CellSize = cellSize;

			Assert.That(cellSize, Is.EqualTo(grid.CellSize));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Grid3D), typeof(Grid3D))]
		public void GetSetCellGap()
		{
			var grid = ObjectExt.FindObjectByTypeFast<Grid3D>();
			var cellGap = new Vector3(1.1f, 2.2f, 3.3f);

			grid.CellGap = cellGap;

			Assert.That(cellGap, Is.EqualTo(grid.CellGap));
		}
	}
}
