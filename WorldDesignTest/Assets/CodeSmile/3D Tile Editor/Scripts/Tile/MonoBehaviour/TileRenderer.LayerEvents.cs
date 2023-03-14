// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public partial class TileLayerRenderer
	{
		private void RegisterTileWorldEvents()
		{
			var layer = m_World.ActiveLayer;
			layer.OnClearTiles += OnClearActiveLayer;
			layer.OnSetTiles += SetOrReplaceTiles;
			layer.OnSetTileFlags += SetTileFlags;
		}

		private void UnregisterTileWorldEvents()
		{
			var layer = m_World.ActiveLayer;
			layer.OnClearTiles -= OnClearActiveLayer;
			layer.OnSetTiles -= SetOrReplaceTiles;
			layer.OnSetTileFlags -= SetTileFlags;
		}

		private void OnClearActiveLayer() => RecreateTileProxyPool();

		private void SetOrReplaceTiles(GridRect dirtyRect)
		{
			// FIXME: this is brute force recreating
			RecreateTileProxyPool();
			
			/*
			// mark visible rect as requiring update
			m_PrevVisibleRect = new GridRect();
			SetTilesInRectAsDirty(dirtyRect);
			UpdateTileProxyObjects(m_World.ActiveLayer);
			*/
			
		}

		private void SetTileFlags(GridCoord coord, TileFlags flags) => Debug.LogWarning("SetTileFlags not implemented");
		// if (TryGetGameObjectAtCoord(coord, out var go))
		// 	ApplyTileFlags(go, flags);
	}
}