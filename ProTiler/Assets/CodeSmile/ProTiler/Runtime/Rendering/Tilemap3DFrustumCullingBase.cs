// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Rendering
{
	public abstract class Tilemap3DFrustumCullingBase : ITilemap3DFrustumCulling
	{
		public abstract IEnumerable<GridCoord> GetVisibleCoords();
	}
}
