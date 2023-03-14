// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.Tile
{

	public partial class TileLayerRenderer
	{
		private GameObject m_Cursor;
		private int3 m_CursorRenderCoord;

		private void UpdateCursorTile(TileLayer layer)
		{
			/*
				var cursorCoord = layer.CursorCoord;

				var index = layer.SelectedTileSetIndex;
				if (m_SelectedTileIndex != index)
				{
					m_SelectedTileIndex = index;
					Debug.Log($"selected tile index: {m_SelectedTileIndex}");

					m_Cursor.gameObject.DestroyInAnyMode();

					var prefab = layer.TileSet.GetPrefab(index);
					m_Cursor = FindOrCreateChildObject("Cursor", prefab, RenderHideFlags);
					SetCursorPosition(layer, cursorCoord);
				}

				if (m_CursorRenderCoord.Equals(cursorCoord) == false)
				{
					SetCursorPosition(layer, cursorCoord);
					//Debug.Log($"cursor pos changed: {m_CursorRenderCoord}");
				}
				*/
		}

		private void CreateCursorOnce()
		{
			m_Cursor = FindOrCreateGameObject("Cursor", m_PersistentObjectHideFlags);
		}

		private void SetCursorPosition(TileLayer layer, int3 cursorCoord)
		{
			m_CursorRenderCoord = cursorCoord;
			m_Cursor.transform.position = layer.Grid.ToWorldPosition(m_CursorRenderCoord) + layer.TileSet.GetTileOffset();
		}
	}
}