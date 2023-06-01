// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Rendering
{
	public class Tilemap3DFrustumCulling : Tilemap3DCullingBase
	{
		private GridCoord m_TestOffset;

		public override IEnumerable<GridCoord> GetVisibleCoords()
		{
			const Int32 width = 14;
			const Int32 height = 1;
			const Int32 length = 14;

			//UpdateTestOffset();

			var coords = new List<GridCoord>(width * height * length);

			for (var z = 0; z < length; z++)
				for (var y = 0; y < height; y++)
					for (var x = 0; x < width; x++)
						coords.Add(new GridCoord(x, y, z) + m_TestOffset);

			return coords;
		}

		private void UpdateTestOffset()
		{
			if (Time.frameCount % 2 == 0)
			{
				m_TestOffset.x--;
				m_TestOffset.z--;

				if (m_TestOffset.x < 0)
				{
					m_TestOffset.x = 24;
					m_TestOffset.z = 24;
				}
			}
		}

		public Camera GetMainOrSceneViewCamera()
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
				return SceneView.lastActiveSceneView.camera;
#endif

			return Camera.main;
		}
	}
}
