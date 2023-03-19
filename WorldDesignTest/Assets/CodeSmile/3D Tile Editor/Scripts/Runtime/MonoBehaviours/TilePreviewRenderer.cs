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
	public sealed class TilePreviewRenderer : MonoBehaviour
	{
		[NonSerialized] private readonly List<GameObject> m_ToBeDeletedPreviews = new();
		[NonSerialized] private GameObject m_Preview;
		[NonSerialized] private int3 m_RenderCoord;
		[NonSerialized] private int m_TileSetIndex = Global.InvalidTileSetIndex;
		[NonSerialized] private TileWorld m_World;

		private bool m_ShowPreview;

		private void Update() => DestroyCursorsScheduledForDeletion();

		private void OnEnable() => m_World = GetComponent<TileWorld>();

		private void OnDisable()
		{
			ScheduleCursorForDeletion();
			DestroyCursorsScheduledForDeletion();
		}

		private void OnRenderObject()
		{
			if (CameraExt.IsSceneViewOrGameCamera(Camera.current) == false)
				return;

			if (m_ShowPreview)
				UpdateCursorTile();
		}

		private void DestroyCursorsScheduledForDeletion()
		{
			if (m_ToBeDeletedPreviews.Count > 0)
			{
				foreach (var cursor in m_ToBeDeletedPreviews)
					cursor.DestroyInAnyMode();
				m_ToBeDeletedPreviews.Clear();
			}
		}

		private void UpdateCursorTile()
		{
			var layer = m_World.ActiveLayer;
			var cursorCoord = layer.DebugCursorCoord;
			var index = layer.SelectedTileSetIndex;
			if (m_TileSetIndex != index || m_Preview == null)
			{
				m_TileSetIndex = index;
				//Debug.Log($"selected tile index: {m_SelectedTileSetIndex}");

				UpdateCursorInstance(layer, m_TileSetIndex, cursorCoord);
			}

			if (m_RenderCoord.Equals(cursorCoord) == false)
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
				ScheduleCursorForDeletion();
				InstantiateCursor(prefab);
				SetCursorPosition(layer, cursorCoord);
			}
		}

		private void InstantiateCursor(GameObject prefab)
		{
			m_Preview = Instantiate(prefab);
			m_Preview.name = "Cursor";
			m_Preview.hideFlags = Global.TileRenderHideFlags;
			m_Preview.transform.parent = transform;
		}

		private void ScheduleCursorForDeletion()
		{
			if (m_Preview != null)
			{
				m_ToBeDeletedPreviews.Add(m_Preview);
				m_Preview = null;
			}
		}

		private void SetCursorPosition(TileLayer layer, int3 cursorCoord)
		{
			if (m_Preview != null)
			{
				m_RenderCoord = cursorCoord;
				m_Preview.transform.position = layer.Grid.ToWorldPosition(m_RenderCoord) + layer.TileSet.GetTileOffset();
			}
		}

		public bool ShowPreview
		{
			get => m_ShowPreview;
			set
			{
				m_ShowPreview = value;
				if (m_ShowPreview)
					UpdateCursorTile();
				else
					ScheduleCursorForDeletion();
			}
		}
	}
}