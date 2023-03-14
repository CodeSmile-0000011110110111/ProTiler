// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public partial class TileRenderer
	{
		private GameObject FindOrCreateGameObject(string name, HideFlags hideFlags = HideFlags.None)
		{
			var prefab = transform.Find(name);
			if (prefab == null)
			{
				prefab = new GameObject(name).transform;
				prefab.gameObject.hideFlags = hideFlags;
				prefab.transform.parent = transform;
			}
			return prefab.gameObject;
		}

		private RectInt GetVisibleRect(TileLayer layer)
		{
			var visibleRect = layer.Grid.GetCameraRect(Camera.current, m_DrawDistance, m_VisibleRectDistance);
			return visibleRect;
		}
	}
}