// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Events;
using CodeSmile.ProTiler.Grid;
using System;
using System.Diagnostics.Contracts;
using UnityEngine;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Controller
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Tilemap3DModel))]
	public sealed class Tilemap3DViewController : MonoBehaviour, ITilemap3DViewController
	{
		public event Action<Grid3DCursor> OnCursorUpdate;
		private Grid3DCursor m_Cursor;
		private Boolean m_CursorEnabled = true;

		private Tilemap3DModel TilemapModel => GetComponent<Tilemap3DModel>();
		internal Grid3DController Grid => TilemapModel.Grid;

		public void OnMouseMove(MouseMoveEventData eventData) => UpdateCursorIfModified(eventData);

		public void EnableCursor()
		{
			m_CursorEnabled = true;
			OnCursorUpdate?.Invoke(m_Cursor);
		}

		public void DisableCursor()
		{
			m_CursorEnabled = false;
			OnCursorUpdate?.Invoke(new Grid3DCursor());
		}

		private void UpdateCursorIfModified(MouseMoveEventData eventData)
		{
			var cursor = new Grid3DCursor(eventData.WorldRay, Grid.CellSize);
			if (cursor != m_Cursor)
				UpdateCursor(cursor);
		}

		private void UpdateCursor(in Grid3DCursor cursor)
		{
			m_Cursor = cursor;
			if (m_CursorEnabled)
				OnCursorUpdate?.Invoke(m_Cursor);
		}
	}
}
