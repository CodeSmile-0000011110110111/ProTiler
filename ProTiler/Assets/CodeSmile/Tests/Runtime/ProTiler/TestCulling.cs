// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Rendering;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.Tests.Runtime.ProTiler
{
	[ExcludeFromCodeCoverage]
	public sealed class TestCulling : Tilemap3DCullingBase
	{
		private Vector2Int m_Size = new Vector2Int(2,2);

		public TestCulling(Vector2Int size) => m_Size = size;
		public TestCulling(int width, int length) => m_Size = new Vector2Int(width, length);

		public override IEnumerable<Vector3Int> GetVisibleCoords()
		{
			var coords = new List<Vector3Int>(m_Size.x * m_Size.y);

			for (var z = 0; z < m_Size.y; z++)
				for (var x = 0; x < m_Size.x; x++)
					coords.Add(new Vector3Int(x, 0, z));

			return coords;
		}
	}
}
