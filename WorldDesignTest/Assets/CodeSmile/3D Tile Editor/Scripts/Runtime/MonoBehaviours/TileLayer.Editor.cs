// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

namespace CodeSmile.Tile
{
	public sealed partial class TileLayer
	{
		public void OnValidate()
		{
			if (m_DebugClearTilesButton)
			{
				m_DebugClearTilesButton = false;
				ClearTiles();
			}

			var tileSetId = m_TileSet.GetInstanceID();
			if (m_TileSetInstanceId != tileSetId)
			{
				m_TileSetInstanceId = tileSetId;
				ForceUpdateTileLayerRenderer();
			}

			UpdateDebugTileCount();
			ClampGridSize();
			DebugClampTileSetIndex();
			DebugSetTileName();
		}

		private void ForceUpdateTileLayerRenderer() => StartCoroutine(new WaitForFramesElapsed(1, () =>
		{
			var renderer = GetComponent<TileLayerRenderer>();
			renderer.enabled = false;
			renderer.enabled = true;
		}));

		private void DebugSetTileName() => m_DebugSelectedTileName =
			TileSet != null ? TileSet.GetPrefab(m_DebugSelectedTileSetIndex)?.name : "<TileSet missing>";

		private void ClampGridSize()
		{
			if (Grid != null)
				Grid.ClampGridSize();
		}
	}
}