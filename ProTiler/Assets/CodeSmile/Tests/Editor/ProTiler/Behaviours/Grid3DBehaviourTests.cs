// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Behaviours;
using CodeSmile.ProTiler.Data;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using UnityEngine;

namespace CodeSmile.Tests.Editor.ProTiler.Behaviours
{
	public class Grid3DBehaviourTests
	{
		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Grid3DBehaviour), typeof(Grid3DBehaviour))]
		public void GetSetCellLayout()
		{
			var grid = ObjectExt.FindObjectByTypeFast<Grid3DBehaviour>();
			var cellLayout = CellLayout.Hexagonal;

			grid.CellLayout = cellLayout;

			Assert.That(cellLayout, Is.EqualTo(grid.CellLayout));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Grid3DBehaviour), typeof(Grid3DBehaviour))]
		public void GetSetCellSize()
		{
			var grid = ObjectExt.FindObjectByTypeFast<Grid3DBehaviour>();
			var cellSize = new Vector3(3f, 4f, 7f);

			grid.CellSize = cellSize;

			Assert.That(cellSize, Is.EqualTo(grid.CellSize));
		}

		[Test] [CreateEmptyScene] [CreateGameObject(nameof(Grid3DBehaviour), typeof(Grid3DBehaviour))]
		public void GetSetCellGap()
		{
			var grid = ObjectExt.FindObjectByTypeFast<Grid3DBehaviour>();
			var cellGap = new Vector3(1.1f, 2.2f, 3.3f);

			grid.CellGap = cellGap;

			Assert.That(cellGap, Is.EqualTo(grid.CellGap));
		}
	}
}
