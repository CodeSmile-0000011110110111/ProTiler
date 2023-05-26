// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Rendering
{
	public class Tilemap3DFrustumCulling : Tilemap3DCullingBase
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
