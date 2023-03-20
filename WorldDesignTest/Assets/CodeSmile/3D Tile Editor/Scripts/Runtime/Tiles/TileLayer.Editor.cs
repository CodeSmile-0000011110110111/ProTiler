// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;

namespace CodeSmile.Tile
{
	public sealed partial class TileLayer
	{
#if UNITY_EDITOR
		public void OnValidate()
		{
			if (m_ClearTilesButton)
			{
				m_ClearTilesButton = false;

				if (m_TileWorld != null)
					Undo.RecordObject(m_TileWorld, "Clear Tiles");
				ClearTiles();
			}

			UpdateDebugTileCount();
			ClampGridSize();
			ClampTileSetIndex();
			SetTileName();
		}

		private void SetTileName() => m_DebugSelectedTileName = TileSet.GetPrefab(m_DebugSelectedTileSetIndex)?.name;

		private void ClampGridSize()
		{
			if (Grid != null)
				Grid.ClampGridSize();
		}

#endif
	}
}