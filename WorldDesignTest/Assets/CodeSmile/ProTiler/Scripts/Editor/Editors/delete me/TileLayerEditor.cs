// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

/*using CodeSmile;
using CodeSmile.Tile;
using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;

// ReSharper disable HeapView.ObjectAllocation

namespace CodeSmileEditor.Tile
{
	//[CustomEditor(typeof(TileLayer))]
	[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
	public partial class TileLayerEditor : Editor
	{
		private GridCoord m_StartSelectionCoord;
		private GridCoord m_CursorCoord;
		private GridRect m_SelectionRect;
		private bool m_IsDrawingTiles;
		private bool m_IsClearingTiles;
		private bool m_IsMouseInView;

		private TileLayerPreviewRenderer m_LayerPreviewRenderer;
		private IInputState m_Input;

		private TileLayer Layer => (TileLayer)target;
		private TileGrid Grid => ((TileLayer)target).Grid;


		private void OnEnable()
		{
			if (m_Input == null)
			{
				var input = new EditorInputState();
				input.OnMouseButtonDown += OnMouseButtonDown;
				input.OnMouseButtonUp += OnMouseButtonUp;
				input.OnScrollWheel += OnScrollWheel;
				input.OnKeyDown += OnKeyDown;
				input.OnKeyUp += OnKeyUp;
				m_Input = input;
			}
		}

		private void OnDisable()
		{
			if (m_Input != null)
			{
				var input = m_Input as EditorInputState;
				input.OnMouseButtonDown -= OnMouseButtonDown;
				input.OnMouseButtonUp -= OnMouseButtonUp;
				input.OnScrollWheel -= OnScrollWheel;
				input.OnKeyDown -= OnKeyDown;
				input.OnKeyUp -= OnKeyUp;
			}
		}

		private void OnSceneGUI()
		{
			if (Selection.activeGameObject != Layer.gameObject)
				return;

			UpdatePreviewRendererState();
			HandleEventsAndInput();
		}

		private void UpdatePreviewRendererState()
		{
			if (m_LayerPreviewRenderer == null)
				m_LayerPreviewRenderer = Layer.GetComponent<TileLayerPreviewRenderer>();

			if (m_LayerPreviewRenderer != null)
			{
				var editMode = TileEditorState.instance.TileEditMode;
				m_LayerPreviewRenderer.enabled = m_IsMouseInView && editMode != TileEditMode.Selection;
			}
		}

		private TileBrush CreateDrawBrush(bool clear) =>
			new(m_CursorCoord, clear ? Const.InvalidTileSetIndex : TileEditorState.instance.DrawTileSetIndex);

		private void UpdateLayerDrawBrush() => Layer.DrawBrush = CreateDrawBrush(m_IsClearingTiles);
		private void HideLayerDrawBrush() => Layer.DrawBrush = CreateDrawBrush(true);

		private void StartTileDrawing()
		{
			UpdateStartSelectionCoord();
			UpdateCursorCoord();

			var editMode = TileEditorState.instance.TileEditMode;
			if (editMode == TileEditMode.PenDraw || editMode == TileEditMode.RectFill)
			{
				m_IsDrawingTiles = true;
				Event.current.Use();
			}
		}

		private void ContinueTileDrawing()
		{
			UpdateCursorCoord();

			if (m_IsDrawingTiles)
			{
				var editMode = TileEditorState.instance.TileEditMode;
				if (editMode == TileEditMode.PenDraw)
				{
					DrawLineFromStartToCursor();
					UpdateStartSelectionCoord();
					Event.current.Use();
				}
				else if (editMode == TileEditMode.RectFill)
					Event.current.Use();
			}
			else
				UpdateStartSelectionCoord();
		}

		private void FinishTileDrawing()
		{
			if (m_IsDrawingTiles)
			{
				UpdateCursorCoord();

				var editMode = TileEditorState.instance.TileEditMode;
				if (editMode == TileEditMode.PenDraw)
				{
					DrawLineFromStartToCursor();
					Event.current.Use();
				}
				else if (editMode == TileEditMode.RectFill)
				{
					DrawRectFromStartToCursor();
					Event.current.Use();
				}

				m_IsDrawingTiles = false;
			}
		}

		private void CancelTileDrawing()
		{
			m_IsDrawingTiles = false;
			UpdateCursorCoord();
			UpdateStartSelectionCoord();
			HideLayerDrawBrush();
		}

		private void UpdateClearingState()
		{
			if (m_IsDrawingTiles == false)
			{
				var shouldClear = Event.current.shift || IsRightMouseButtonDown();
				if (m_IsClearingTiles != shouldClear)
					m_IsClearingTiles = shouldClear;
			}
		}

		private void DrawLineFromStartToCursor() => Layer.DrawLine(m_StartSelectionCoord, m_CursorCoord);
		private void DrawRectFromStartToCursor() => Layer.DrawRect(m_StartSelectionCoord.MakeRect(m_CursorCoord));

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

		private void UpdateSelectionRect() => m_SelectionRect = TileGrid.MakeRect(m_StartSelectionCoord, m_CursorCoord);

		private GridCoord GetMouseCursorCoord()
		{
			var planeY = Layer.transform.position.y;
			if (HandleUtilityExt.GUIPointToGridCoord(m_Input.MousePosition, Grid, out var coord, planeY))
				return coord;

			return Const.InvalidGridCoord;
		}

		private void DrawCursorHandle(TileEditMode editMode)
		{
			if (editMode == TileEditMode.PenDraw || editMode == TileEditMode.RectFill)
			{
				var worldRect = TileGrid.ToWorldRect(m_SelectionRect, Grid.Size);
				var worldPos = Layer.transform.position;
				var cubePos = worldRect.GetWorldCenter() + worldPos;
				var cubeSize = worldRect.GetWorldSize(Layer.Grid.Size.y);

				var prevColor = Handles.color;
				Handles.color = Const.OutlineColor;
				Handles.DrawWireCube(cubePos, cubeSize);
				Handles.color = prevColor;

				//Handles.DrawAAPolyLine();

				var renderer = Layer.GetComponent<TileLayerPreviewRenderer>();
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
						Handles.DrawOutline(new[] { meshRenderer.gameObject }, Const.OutlineColor);
				}
			}
		}

		private void OnLayout() => HandleUtilityExt.AddDefaultControl(GetHashCode());

		private void OnRepaint()
		{
			if (m_IsMouseInView && IsRightMouseButtonDown() == false)
				DrawCursorHandle(TileEditorState.instance.TileEditMode);
		}

		private void OnScrollWheel(IInputState inputState, float scrollDelta)
		{
			var editMode = TileEditorState.instance.TileEditMode;
			if (editMode != TileEditMode.Selection)
			{
				var delta = scrollDelta >= 0 ? 1 : -1;
				var shift = inputState.IsShiftKeyDown;
				var ctrl = inputState.IsCtrlKeyDown;
				if (shift && ctrl)
				{
					Layer.FlipTile(m_CursorCoord, delta);
					Event.current.Use();
				}
				else if (shift)
				{
					Layer.RotateTile(m_CursorCoord, delta);
					Event.current.Use();
				}
				else if (ctrl)
				{
					Layer.IncrementDrawTileSetIndex(delta);
					Event.current.Use();
				}
			}
		}
	}
}*/