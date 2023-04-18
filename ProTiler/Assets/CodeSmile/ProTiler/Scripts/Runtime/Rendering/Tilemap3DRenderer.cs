// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
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
		private Tile3DCoordData[] m_TileCoordDatas;

		private Tilemap3D Tilemap => GetComponent<Tilemap3D>();

		private void OnRenderObject()
		{
			if (CameraExt.IsSceneViewOrGameCamera(Camera.current) == false)
				return;

			if (m_VisibleCoords == null)
			{
				m_VisibleCoords = new Vector3Int[20 * 20];
				m_TileCoordDatas = new Tile3DCoordData[m_VisibleCoords.Length];
			}

			UpdateVisibleTileCoordinates(ref m_VisibleCoords);
			UpdateVisibleTiles();
		}

		private void UpdateVisibleTiles()
		{
			Tilemap.GetTileData(m_VisibleCoords, ref m_TileCoordDatas);

			// TODO dont destroy children, implement pooling
			transform.DestroyAllChildren();

			GameObject instance = null;

			foreach (var tile3DCoordData in m_TileCoordDatas)
			{
				Debug.LogWarning("TODO: get prefab from tiledata");
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
