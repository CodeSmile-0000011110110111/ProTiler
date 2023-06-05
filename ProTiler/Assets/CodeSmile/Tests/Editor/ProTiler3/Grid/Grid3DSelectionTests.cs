// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Grid;
using NUnit.Framework;
using System.Linq;

namespace CodeSmile.Tests.Editor.ProTiler3.Grid
{
	public class Grid3DSelectionTests
	{
		[Test]
		public void NewSelectionIsEmpty()
		{
			var gridSelection = new Grid3DSelection();

			Assert.That(gridSelection.Cells != null);
			Assert.That(gridSelection.Cells.Count(), Is.EqualTo(0));
		}
	}
}
