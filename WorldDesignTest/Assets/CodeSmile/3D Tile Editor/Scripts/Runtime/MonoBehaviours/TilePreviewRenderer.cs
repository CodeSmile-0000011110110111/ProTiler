// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace CodeSmile.Tile
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(TileLayer))]
	public sealed class TilePreviewRenderer : MonoBehaviour
	{
		[NonSerialized] private TileLayer m_Layer;
		[NonSerialized] private readonly List<GameObject> m_ToBeDeletedPreviews = new();
		[NonSerialized] private GameObject m_Preview;
		[NonSerialized] private int3 m_RenderCoord;
		[NonSerialized] private int m_TileSetIndex = Global.InvalidTileSetIndex;

		private bool m_ShowPreview;

		private void OnEnable()
		{
			m_Layer = GetComponent<TileLayer>();
			if (m_Layer == null)
				throw new NullReferenceException("layer is null");
		}

		private void Update() => DestroyCursorsScheduledForDeletion();

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
			var cursorCoord = m_Layer.DebugCursorCoord;
			var index = m_Layer.SelectedTileSetIndex;
			if (m_TileSetIndex != index || m_Preview == null)
			{
				m_TileSetIndex = index;
				//Debug.Log($"selected tile index: {m_SelectedTileSetIndex}");

				UpdateCursorInstance(m_Layer, m_TileSetIndex, cursorCoord);
			}

			if (m_RenderCoord.Equals(cursorCoord) == false)
			{
				SetCursorPosition(m_Layer, cursorCoord);
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
			m_Preview.hideFlags = Global.TileHideFlags;
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