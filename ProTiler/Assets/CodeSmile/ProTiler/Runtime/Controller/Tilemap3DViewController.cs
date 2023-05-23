// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Events;
using CodeSmile.ProTiler.Grid;
using CodeSmile.ProTiler.Tilemap;
using System;
using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEngine;
using GridCoord = UnityEngine.Vector3Int;
using ChunkCoord = UnityEngine.Vector2Int;
using ChunkSize = UnityEngine.Vector2Int;

namespace CodeSmile.ProTiler.Controller
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Tilemap3DModelController))]
	public sealed class Tilemap3DViewController : MonoBehaviour, ITilemap3DViewController
	{
		private Vector3 m_CurrentCursorCoord;
		private Boolean m_IsCurrentCursorCoordValid;

		[Pure] private Tilemap3DModelController TilemapModelController => GetComponent<Tilemap3DModelController>();
		[Pure] private Grid3DController Grid => transform.parent.GetComponent<Grid3DController>();
		[Pure] private Tilemap3D Tilemap
		{
			get => TilemapModelController.Tilemap;
			set => TilemapModelController.Tilemap = value;
		}

		private static void IncrementCurrentUndoGroup()
		{
#if UNITY_EDITOR
			Undo.IncrementCurrentGroup();
#endif
		}

		public void OnMouseMove(MouseMoveEventData eventData)
		{
			var currentWorldRay = eventData.CurrentWorldRay;
			var lastWorldRay = eventData.LastWorldRay;
			m_IsCurrentCursorCoordValid = IntersectsCoord(currentWorldRay, out var currentCoord);
			if (m_IsCurrentCursorCoordValid)
			{
				var isLastCoordValid = IntersectsCoord(lastWorldRay, out var lastCoord);
				if (isLastCoordValid == false || currentCoord != lastCoord)
				{
					Debug.Log("    coord changed: " + currentCoord);
					var cellSize = TilemapModelController.Grid.CellSize;
					var pos = new Vector3(currentCoord.x * cellSize.x, currentCoord.y * cellSize.y,
						currentCoord.z * cellSize.z) + cellSize * .5f;
					m_CurrentCursorCoord = pos;
				}
			}
		}

		[Pure] private void Reset() => TilemapModelController.ClearTilemap(Tilemap3DUtility.DefaultChunkSize);

		private Boolean IntersectsCoord(Ray ray, out GridCoord coord)
		{
			var intersects = ray.IntersectsPlane(out var point);
			coord = intersects ? Grid3DUtility.ToCoord(point, TilemapModelController.Grid.CellSize) : default;
			return intersects;
		}

	}
}
