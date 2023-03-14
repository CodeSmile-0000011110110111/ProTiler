// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.Tile
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(TileWorld))]
	public sealed class TileCursorRenderer : MonoBehaviour
	{
		private readonly List<GameObject> m_ToBeDeletedCursors = new();
		[NonSerialized] private GameObject m_Cursor;
		[NonSerialized] private int3 m_CursorRenderCoord;
		[NonSerialized] private int m_SelectedTileSetIndex = Global.InvalidTileSetIndex;
		[NonSerialized] private TileWorld m_World;

		private void Awake() => m_World = GetComponent<TileWorld>();

		private void Update()
		{
			if (m_ToBeDeletedCursors.Count > 0)
			{
				foreach (var cursor in m_ToBeDeletedCursors)
					cursor.DestroyInAnyMode();
				m_ToBeDeletedCursors.Clear();
			}
		}

		private void OnRenderObject()
		{
			if (CameraExt.IsCurrentCameraValid() == false)
				return;

			var layer = m_World.ActiveLayer;
			UpdateCursorTile(layer);
		}

		private void UpdateCursorTile(TileLayer layer)
		{
			var cursorCoord = layer.CursorCoord;

			var index = layer.SelectedTileSetIndex;
			if (m_SelectedTileSetIndex != index)
			{
				m_SelectedTileSetIndex = index;
				Debug.Log($"selected tile index: {m_SelectedTileSetIndex}");

				UpdateCursorInstance(layer, m_SelectedTileSetIndex, cursorCoord);
			}

			if (m_CursorRenderCoord.Equals(cursorCoord) == false)
			{
				SetCursorPosition(layer, cursorCoord);
				//Debug.Log($"cursor pos changed: {m_CursorRenderCoord}");
			}
		}

		private void UpdateCursorInstance(TileLayer layer, int index, int3 cursorCoord)
		{
			var prefab = layer.TileSet.GetPrefab(index);
			if (prefab != null)
			{
				if (m_Cursor != null)
					m_ToBeDeletedCursors.Add(m_Cursor);

				m_Cursor = Instantiate(prefab);
				m_Cursor.name = "Cursor";
				m_Cursor.hideFlags = Global.TileRenderHideFlags;
				m_Cursor.transform.parent = transform;
				SetCursorPosition(layer, cursorCoord);
			}
		}

		private void SetCursorPosition(TileLayer layer, int3 cursorCoord)
		{
			m_CursorRenderCoord = cursorCoord;
			m_Cursor.transform.position = layer.Grid.ToWorldPosition(m_CursorRenderCoord) + layer.TileSet.GetTileOffset();
		}
	}
}