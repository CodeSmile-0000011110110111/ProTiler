// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler
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
			var visibleTiles = m_Layer.GetTilesInRect(m_VisibleRect);
			if (visibleTiles != null)
			{
				foreach (var coord in visibleTiles.Keys)
				{
					var tile = visibleTiles[coord];
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