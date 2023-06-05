// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler3.Runtime.Grid;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.Editor.ProTiler3.Grid
{
	public class Grid3DBehaviourTests
	{
		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Grid3DController), typeof(Grid3DController))]
		public void GetSetCellLayout()
		{
			var grid = ObjectExt.FindObjectByTypeFast<Grid3DController>();
			var cellLayout = CellLayout.Hexagonal;

			grid.CellLayout = cellLayout;

			Assert.That(cellLayout, Is.EqualTo(grid.CellLayout));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Grid3DController), typeof(Grid3DController))]
		public void GetSetCellSize()
		{
			var grid = ObjectExt.FindObjectByTypeFast<Grid3DController>();
			var cellSize = new Vector3(3f, 4f, 7f);

			grid.CellSize = cellSize;

			Assert.That(cellSize, Is.EqualTo(grid.CellSize));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Grid3DController), typeof(Grid3DController))]
		public void GetSetCellGap()
		{
			var grid = ObjectExt.FindObjectByTypeFast<Grid3DController>();
			var cellGap = new Vector3(1.1f, 2.2f, 3.3f);

			grid.CellGap = cellGap;

			Assert.That(cellGap, Is.EqualTo(grid.CellGap));
		}
	}
}
