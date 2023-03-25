// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	[RequireComponent(typeof(TileLayer), typeof(TileLayerRenderer), typeof(TileLayerPreviewRenderer))]
	public class TileLayerToolbox : MonoBehaviour
	{
		[Header("Debug")]
		[SerializeField] private bool m_DebugClearTilesButton;
		[ReadOnlyField] [SerializeField] private string m_DebugSelectedTileName;

		private int m_TileSetInstanceId;
		private TileLayer m_Layer;
		private TileLayerPreviewRenderer m_PreviewRenderer;
		private TileLayerRenderer m_LayerRenderer;

		private TileLayerRenderer LayerRenderer
		{
			get
			{
				if (m_LayerRenderer == null)
					m_LayerRenderer = gameObject.GetOrAddComponent<TileLayerRenderer>();
				return m_LayerRenderer;
			}
		}

		public TileBrush DrawBrush
		{
			get => PreviewRenderer.PreviewBrush;
			set
			{
				PreviewRenderer.PreviewBrush = value;
				DebugSetTileName(value.TileSetIndex);
			}
		}

		public TileLayer Layer
		{
			get
			{
				if (m_Layer == null)
					m_Layer = GetComponent<TileLayer>();
				return m_Layer;
			}
		}
		public bool TileDrawPreviewEnabled
		{
			get => PreviewRenderer.enabled;
			set => PreviewRenderer.enabled = value;
		}
		private TileLayerPreviewRenderer PreviewRenderer
		{
			get
			{
				if (m_PreviewRenderer == null)
					m_PreviewRenderer = gameObject.GetOrAddComponent<TileLayerPreviewRenderer>();
				return m_PreviewRenderer;
			}
		}

		public void OnValidate()
		{
			if (isActiveAndEnabled)
			{
				StopAllCoroutines();
				StartCoroutine(new WaitForFramesElapsed(1, () =>
				{
					CheckForClearTilesButton();
					ForceRedrawIfTileSetChanged();
				}));
			}
		}

		public void DrawLine(GridCoord start, GridCoord end)
		{
			Layer.RecordUndoInEditor(DrawBrush.IsClearing ? "Clear Tiles" : "Draw Tiles");
			var (coords, tiles) = Layer.DrawLine(start, end, DrawBrush);
			Layer.SetDirtyInEditor();

			LayerRenderer.RedrawTiles(coords, tiles);
		}

		public void DrawRect(RectInt rect)
		{
			Layer.RecordUndoInEditor(DrawBrush.IsClearing ? "Clear Tiles" : "Draw Tiles");
			var (coords, tiles) = Layer.DrawRect(rect, DrawBrush);
			Layer.SetDirtyInEditor();

			LayerRenderer.RedrawTiles(coords, tiles);
		}

		public void ClearAllTiles()
		{
			Layer.RecordUndoInEditor(nameof(ClearAllTiles));
			Layer.ClearAllTiles();
			Layer.SetDirtyInEditor();

			LayerRenderer.ForceRedraw();
		}

		public void SetTileFlags(GridCoord coord, TileFlags flags)
		{
			Layer.RecordUndoInEditor(nameof(SetTileFlags));
			var tileFlags = Layer.SetTileFlags(coord, flags);
			Layer.SetDirtyInEditor();

			LayerRenderer.UpdateTileFlagsAndRedraw(coord, tileFlags);
		}

		public void ClearTileFlags(GridCoord coord, TileFlags flags)
		{
			Layer.RecordUndoInEditor(nameof(ClearTileFlags));
			var tileFlags = Layer.ClearTileFlags(coord, flags);
			Layer.SetDirtyInEditor();

			LayerRenderer.UpdateTileFlagsAndRedraw(coord, tileFlags);
		}

		public void RotateTile(GridCoord coord, int delta)
		{
			Layer.RecordUndoInEditor(nameof(RotateTile));
			var tileFlags = Layer.RotateTile(coord, delta);
			Layer.SetDirtyInEditor();

			LayerRenderer.UpdateTileFlagsAndRedraw(coord, tileFlags);
		}

		public void FlipTile(GridCoord coord, int delta)
		{
			Layer.RecordUndoInEditor(nameof(FlipTile));
			var tileFlags = Layer.FlipTile(coord, delta);
			Layer.SetDirtyInEditor();

			LayerRenderer.UpdateTileFlagsAndRedraw(coord, tileFlags);
		}

		private void DebugSetTileName(int index) => m_DebugSelectedTileName = Layer.TileSet.GetPrefab(index).name;

		private void CheckForClearTilesButton()
		{
			if (isActiveAndEnabled && m_DebugClearTilesButton)
			{
				m_DebugClearTilesButton = false;
				ClearAllTiles();
			}
		}

		private void ForceRedrawIfTileSetChanged()
		{
			var tileSetId = Layer.TileSet.GetInstanceID();
			if (m_TileSetInstanceId != tileSetId)
			{
				m_TileSetInstanceId = tileSetId;
				LayerRenderer.ForceRedraw();
			}
		}
	}
}