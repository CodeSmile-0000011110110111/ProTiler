// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.ProTiler
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(TileLayer))]
	public sealed class TileLayerPreviewRenderer : MonoBehaviour
	{
		[NonSerialized] private readonly List<GameObject> m_ToBeDeletedPreviews = new();
		[NonSerialized] private TileLayer m_Layer;
		[NonSerialized] private GameObject m_Preview;
		[NonSerialized] private GridCoord m_RenderCoord;
		[NonSerialized] private int m_TileSetIndex = TileData.InvalidTileSetIndex;

		private TileBrush m_PreviewBrush;
		public TileBrush PreviewBrush
		{
			get => m_PreviewBrush;
			set => m_PreviewBrush = value;
		}

		private TileLayer Layer
		{
			get
			{
				if (m_Layer == null)
					m_Layer = GetComponent<TileLayer>();
				return m_Layer;
			}
		}

		private void Reset() => OnEnable(); // Editor will not call OnEnable by itself

		private void Update() => DestroyPreviewsScheduledForDeletion();

		private void OnEnable() => UpdatePreview();

		private void OnDisable()
		{
			ScheduleCurrentPreviewForDeletion();
			DestroyPreviewsScheduledForDeletion();
		}

		private void OnRenderObject() => UpdatePreview();

		private void ScheduleCurrentPreviewForDeletion()
		{
			if (m_Preview != null)
			{
				m_ToBeDeletedPreviews.Add(m_Preview);
				m_Preview = null;
			}
		}

		private void DestroyPreviewsScheduledForDeletion()
		{
			if (m_ToBeDeletedPreviews.Count > 0)
			{
				foreach (var cursor in m_ToBeDeletedPreviews)
					cursor.DestroyInAnyMode();
				m_ToBeDeletedPreviews.Clear();
			}
		}

		private void UpdatePreview()
		{
			var index = m_PreviewBrush.TileSetIndex;
			if (m_TileSetIndex != index || m_Preview == null)
			{
				m_TileSetIndex = index;
				UpdateCursorInstance(m_TileSetIndex, m_PreviewBrush.Coord);
			}

			if (m_RenderCoord.Equals(m_PreviewBrush.Coord) == false)
				SetCursorPosition(m_PreviewBrush.Coord);
		}

		private void UpdateCursorInstance(int index, GridCoord cursorCoord)
		{
			var prefab = Layer.TileSet.GetPrefab(index);
			if (prefab != null)
			{
				ScheduleCurrentPreviewForDeletion();
				InstantiateCursor(prefab);
				SetCursorPosition(cursorCoord);
			}
		}

		private void InstantiateCursor(GameObject prefab)
		{
			m_Preview = Instantiate(prefab);
			m_Preview.name = "DrawPreview";
			m_Preview.hideFlags = Global.TileHideFlags;
			m_Preview.transform.parent = transform;
		}

		private void SetCursorPosition(GridCoord cursorCoord)
		{
			if (m_Preview != null)
			{
				m_RenderCoord = cursorCoord;
				m_Preview.transform.position = Layer.Grid.ToWorldPosition(m_RenderCoord) + Layer.TileSet.GetTileOffset();
			}
		}
	}
}