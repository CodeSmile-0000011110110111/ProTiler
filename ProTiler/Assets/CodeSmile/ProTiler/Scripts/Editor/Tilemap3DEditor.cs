// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor.ProTiler;
using CodeSmile.Editor.ProTiler.Extensions;
using CodeSmile.Extensions;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor
{
	[CustomEditor(typeof(Tilemap3D))]
	public partial class Tilemap3DEditor : UnityEditor.Editor
	{
		private Vector3Int m_StartSelectionCoord;
		private Vector3Int m_CursorCoord;
		private RectInt m_SelectionRect;
		private bool m_IsDrawingTiles;
		private bool m_IsClearingTiles;
		private bool m_HasMouseFocus;
		private Tilemap3D Tilemap => (Tilemap3D)target;

		private void OnEnable() => RegisterInputEvents();

		private void OnDisable() => UnregisterInputEvents();

		public void OnSceneGUI()
		{
			if (Selection.activeGameObject != Tilemap.gameObject)
			{
				//Debug.LogError($"Tilemap Editor object selection mismatch: {Selection.activeGameObject}");
				return;
			}

			//UpdateTileDrawPreviewEnabledState();
			m_Input.Update();

			switch (Event.current.type)
			{
				case EventType.Layout:
					OnLayout();
					break;
				case EventType.MouseMove:
					OnMouseMove();
					break;
				case EventType.MouseDrag:
					OnMouseDrag();
					break;
				case EventType.MouseEnterWindow:
					OnMouseEnterWindow();
					break;
				case EventType.MouseLeaveWindow:
					OnMouseLeaveWindow();
					break;
				case EventType.Repaint:
					OnRepaint();
					break;
			}

			UpdateClearingState();
			//UpdateLayerDrawBrush();
		}

		private void OnLayout() => HandleUtilityExt.AddDefaultControl(GetHashCode());

		private void OnRepaint()
		{
			if (m_HasMouseFocus && IsRightMouseButtonDown() == false)
				DrawCursorHandle(TileEditMode.PenDraw);
		}

		private Vector3Int GetMouseCursorCoord() => HandleUtilityExt.GUIPointToGridCoord
			(m_Input.MousePosition, Tilemap.Grid.CellSize, out var coord, Tilemap.transform.position.y)
			? coord
			: Vector3Int.zero;

		public bool StartTileDrawing(TileEditMode editMode)
		{
			var useEvent = false;

			UpdateStartSelectionCoord();
			UpdateCursorCoord();

			if (editMode is TileEditMode.PenDraw or TileEditMode.RectFill)
			{
				m_IsDrawingTiles = true;
				useEvent = true;
			}

			return useEvent;
		}

		private bool ContinueTileDrawing(TileEditMode editMode)
		{
			var useEvent = false;

			UpdateCursorCoord();
			if (m_IsDrawingTiles)
			{
				if (editMode == TileEditMode.PenDraw)
				{
					DrawLineFromStartToCursor();
					UpdateStartSelectionCoord();
					useEvent = true;
				}
				else if (editMode == TileEditMode.RectFill)
					useEvent = true;
			}
			else
				UpdateStartSelectionCoord();

			return useEvent;
		}

		private bool FinishTileDrawing(TileEditMode editMode)
		{
			var useEvent = false;

			if (m_IsDrawingTiles)
			{
				UpdateCursorCoord();

				if (editMode == TileEditMode.PenDraw)
				{
					DrawLineFromStartToCursor();
					useEvent = true;
				}
				else if (editMode == TileEditMode.RectFill)
				{
					DrawRectFromStartToCursor();
					useEvent = true;
				}

				m_IsDrawingTiles = false;
			}

			return useEvent;
		}

		private void CancelTileDrawing()
		{
			m_IsDrawingTiles = false;
			UpdateCursorCoord();
			UpdateStartSelectionCoord();
			ShowDrawBrush(false);
		}

		private void DrawLineFromStartToCursor()
		{
			Tilemap3DStats.instance.DrawTileCount++;
			Tilemap.DrawLine(m_StartSelectionCoord, m_CursorCoord);
		}

		private void DrawRectFromStartToCursor() => Tilemap.DrawRect(m_StartSelectionCoord.MakeRect(m_CursorCoord));

		private void ShowDrawBrush(bool show = true)
		{
			//Toolbox.TileDrawPreviewEnabled = show;
		}

		private void UpdateClearingState()
		{
			if (m_IsDrawingTiles == false)
			{
				var shouldClear = m_Input.IsAltKeyDown || IsRightMouseButtonDown();
				if (m_IsClearingTiles != shouldClear)
					m_IsClearingTiles = shouldClear;
			}
		}

		private void UpdateStartSelectionCoord()
		{
			m_StartSelectionCoord = GetMouseCursorCoord();
			UpdateSelectionRect();
		}

		private void UpdateCursorCoord()
		{
			m_CursorCoord = GetMouseCursorCoord();
			UpdateSelectionRect();
		}

		private void UpdateSelectionRect()
		{
			var coordMin = Vector3Int.Min(m_StartSelectionCoord, m_CursorCoord);
			var coordMax = Vector3Int.Max(m_StartSelectionCoord, m_CursorCoord);
			m_SelectionRect = new RectInt(coordMin.x, coordMin.z, coordMax.x - coordMin.x + 1, coordMax.z - coordMin.z + 1);
		}

		private void DrawCursorHandle(TileEditMode editMode)
		{
			if (editMode is TileEditMode.PenDraw or TileEditMode.RectFill)
			{
				var cellSize = Tilemap.Grid.CellSize;
				var worldRect = m_SelectionRect.ToWorldRect(cellSize);
				var worldPos = Tilemap.transform.position;
				var cubePos = worldRect.GetWorldCenter() + worldPos;
				var cubeSize = worldRect.GetWorldSize(cellSize.y);

				var prevColor = Handles.color;
				Handles.color = Colors.OutlineColor;
				Handles.DrawWireCube(cubePos, cubeSize);
				Handles.color = prevColor;

				//Handles.DrawAAPolyLine();

				// FIXME: get the preview objects from the toolbox instead
				/*
				var renderer = Tilemap.GetComponent<TileLayerPreviewRenderer>();
				var cursor = renderer.transform.Find("Cursor");
				if (cursor != null)
				{
					var meshRenderer = cursor.GetComponent<MeshRenderer>();
					if (meshRenderer == null && cursor.childCount > 0)
					{
						meshRenderer = cursor.GetChild(0).GetComponent<MeshRenderer>();
						if (meshRenderer == null && cursor.GetChild(0).childCount > 0)
							meshRenderer = cursor.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
					}

					if (meshRenderer != null)
						Handles.DrawOutline(new[] { meshRenderer.gameObject }, Global.OutlineColor);
				}
			*/
			}
		}
	}
}
