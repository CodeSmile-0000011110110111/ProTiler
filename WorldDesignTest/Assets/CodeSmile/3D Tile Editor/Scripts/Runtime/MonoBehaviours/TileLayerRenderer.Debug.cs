// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Tile
{
	public sealed partial class TileLayerRenderer
	{
#if UNITY_EDITOR
		private void OnDrawGizmosSelected() => DrawTilePoolGizmos();

		private void DrawTilePoolGizmos()
		{
			var gridSize = m_Layer.Grid.Size;
			GizmosDrawRect(m_PrevVisibleRect.ToWorldRect(gridSize), Color.yellow);
			GizmosDrawRect(m_VisibleRect.ToWorldRect(gridSize), Color.green);
			//GizmosDrawVisibleTiles();
		}

		private void GizmosDrawVisibleTiles()
		{
			var grid = m_Layer.Grid;
			if (m_GizmosVisibleTiles != null)
			{
				foreach (var coord in m_GizmosVisibleTiles.Keys)
				{
					var tile = m_GizmosVisibleTiles[coord];
					var pos = grid.ToWorldPosition(coord);
					Handles.Label(pos + new float3(0f, 3f, 0f), tile.TileSetIndex.ToString());
				}
			}
		}

		private void GizmosDrawRect(Rect rect, Color color)
		{
			var prevColor = Gizmos.color;
			Gizmos.color = color;
			Gizmos.DrawWireCube(new Vector3(rect.center.x, 0f, rect.center.y),
				new Vector3(rect.size.x, 1f, rect.size.y));
			Gizmos.color = prevColor;
		}
#endif
	}
}