// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler3.Grid
{
	public sealed class Grid3DSelection
	{
		public IEnumerable<GridCoord> Cells = new List<GridCoord>();
	}
}
