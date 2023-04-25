// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Data;
using System;
using UnityEngine;

namespace CodeSmile.ProTiler.Rendering
{
	[ExecuteAlways]
	[RequireComponent(typeof(Tilemap3D))]
	public class Tilemap3DRenderer : MonoBehaviour
	{
		[SerializeField] private int m_DrawDistance = 20;
		private Vector3Int[] m_VisibleCoords;
		private Tile3DCoord[] m_TileCoordDatas;

		/*
		 * Todo:
		 *
		 * decisions:
		 *
		 * Prio 1:
		 *	work on Tile3D assets and pooling
		 *  connect Tile3DData.TileIndex with an indexed Tile3D asset from pool
		 *  create Tile3DAsset from selected prefab(s)
		 *
		 * Prio 2:
		 *	ComponentPool tests
		 *  pooling strategies (increase, instantiate/destroy, shrink)
		 *
		 *
		 * get the visible tile coords from a callback
		 * get the tiles from the tilemap using those coordinates
		 * instantiate/update the pooled tile proxy objects with tiledata
		 *
		 * needs a pool of RenderTile3D
		 *	size equals visible coords
		 *  if visible coord count changes, pool only expands automatically but won't shrink
		 *	pool can be shrinked through manual set size call
		 *
		 * needs a pool of Tile3D prefab instances
		 *  holds instances of previously instantiated tile instances of a given index (ie cache)
		 *	reasonable base count, expands automatically, shrinks if called manually
		 *
		 *
		 * tile proxy:
		 * get the prefab from the objectset using TileIndex
		 * instantiate prefab instance if it changed
		 *
		 */

		private Tilemap3D Tilemap => GetComponent<Tilemap3D>();

		private void OnRenderObject()
		{
			if (CameraExt.IsSceneViewOrGameCamera(Camera.current) == false)
				return;

			if (m_VisibleCoords == null)
			{
				m_VisibleCoords = new Vector3Int[20 * 20];
				m_TileCoordDatas = new Tile3DCoord[m_VisibleCoords.Length];
			}

			UpdateVisibleTileCoordinates(ref m_VisibleCoords);
			UpdateVisibleTiles();
		}

		private void UpdateVisibleTiles()
		{
			Tilemap.GetTiles(m_VisibleCoords, ref m_TileCoordDatas);

			// TODO dont destroy children, implement pooling
			transform.DestroyAllChildren();

			GameObject instance = null;

			Debug.LogWarning("TODO: get prefab from tiledata");
			foreach (var tile3DCoordData in m_TileCoordDatas)
			{
				/*
				var tile = tile3DCoordData.TileData.Tile;
				if (tile == null || tile.Prefab == null)
					continue;

				if (instance != null)
					DestroyImmediate(instance);

				instance = Instantiate(tile.Prefab, transform);
				*/

			}
		}

		private void UpdateVisibleTileCoordinates(ref Vector3Int[] coords)
		{
			for (var x = 0; x < 20; x++)
			{
				for (var y = 0; y < 20; y++)
				{
					var index = Grid3DUtility.ToIndex2D(x, y, 20);
					coords[index].x = x;
					coords[index].y = 0;
					coords[index].z = y;
				}
			}
		}
	}
}
