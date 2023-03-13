// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.Tile
{
	public sealed partial class TileWorld
	{}

	public partial class TileRenderer
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

		private void OnClearActiveLayer() => Debug.LogWarning("clear layer not implemented");

		private void SetOrReplaceTiles(RectInt rect)
		{
			UpdateTileProxyObjects(m_World.ActiveLayer);
		}
	}
}