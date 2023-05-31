// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Model;
using System;
using UnityEditor;
using UnityEngine;
using CellSize = UnityEngine.Vector3;
using GridCoord = UnityEngine.Vector3Int;

namespace CodeSmile.ProTiler.Rendering
{
	[AddComponentMenu("")] // hide from Add Component list
	public class Tile3DRenderer : MonoBehaviour
	{
		private GameObject m_TilePrefabInstance;
		private Tile3DCoord m_TileCoord;
		private Boolean m_EnableDebugDrawing;
		public Boolean EnableDebugDrawing
		{
			get => m_EnableDebugDrawing;
			set => m_EnableDebugDrawing = value;
		}
		public Tile3DCoord TileCoord => m_TileCoord;

		public void UpdateTransform(GridCoord coord, CellSize cellSize)
		{
			var worldPos = Grid3DUtility.ToWorldPos(coord, cellSize);
			transform.position = worldPos;
			transform.localScale = cellSize;
		}

		public void UpdateTileCoord(Tile3DCoord tileCoord, ITile3DAssetSet tileAssetSet)
		{
			var tileIndex = tileCoord.Tile.Index;
			var tileFlags = tileCoord.Tile.Flags;
			var tileIndexChanged = m_TileCoord.Tile.Index != tileIndex;
			var flagsChanged = m_TileCoord.Tile.Flags != tileFlags;
			m_TileCoord = tileCoord;

			if (tileIndexChanged)
			{
				DestroyTilePrefabInstance();

				var tilePrefab = tileAssetSet[tileIndex].Prefab;
				m_TilePrefabInstance = Instantiate(tilePrefab, transform.position, Quaternion.identity, transform);
			}

			if (flagsChanged)
			{
				// TODO: update rotation and/or scale of instance
			}
		}

		private void DestroyTilePrefabInstance()
		{
			if (m_TilePrefabInstance != null)
			{
				m_TilePrefabInstance.DestroyInAnyMode();
				m_TilePrefabInstance = null;
			}
		}

#if UNITY_EDITOR
		private Grid3DController Grid => transform.parent.parent.parent.GetComponent<Grid3DController>();
		private void OnDrawGizmos()
		{
			if (EnableDebugDrawing == false || EditorPrefs.GetBool("CodeSmile.TestRunner.Running"))
				return;

			var style = new GUIStyle { fontSize = 20, normal = new GUIStyleState { textColor = Color.white } };
			Handles.Label(transform.position, $"#{m_TileCoord.Tile.Index}", style);
		}
#endif
	}
}
