// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Events;
using System;
using System.Diagnostics.Contracts;
using UnityEngine;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Controller
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Tilemap3DModelController))]
	public sealed class Tilemap3DViewController : MonoBehaviour, ITilemap3DViewController
	{
		public event Action<Grid3DCursor> OnCursorUpdate;
		private Grid3DCursor m_Cursor;

		[Pure] private Tilemap3DModelController TilemapModelController => GetComponent<Tilemap3DModelController>();
		[Pure] private Grid3DController Grid => transform.parent.GetComponent<Grid3DController>();

		[Pure] public void OnMouseMove(MouseMoveEventData eventData)
		{
			var cursor = new Grid3DCursor(eventData.CurrentWorldRay, eventData.LastWorldRay, Grid.CellSize);
			if (cursor.Equals(m_Cursor) == false)
			{
				m_Cursor = cursor;
				OnCursorUpdate?.Invoke(m_Cursor);
			}
		}
	}
}