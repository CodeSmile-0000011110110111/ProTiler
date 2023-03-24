// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Diagnostics.CodeAnalysis;

namespace CodeSmile.Tile
{
	[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
	public sealed partial class TileLayer
	{
		public void OnValidate()
		{
			if (isActiveAndEnabled)
			{
				StopAllCoroutines();
				StartCoroutine(new WaitForFramesElapsed(1, () =>
				{
					CheckForClearTiles();
					CheckForTileSetChange();
					ClampGridSize();
					DebugSetTileName();
				
				}));
			}
		}

		private void CheckForClearTiles()
		{
			if (isActiveAndEnabled && m_DebugClearTilesButton)
			{
				m_DebugClearTilesButton = false;
				ClearAllTiles();
			}
		}

		private void CheckForTileSetChange()
		{
			var tileSetId = TileSet.GetInstanceID();
			if (m_TileSetInstanceId != tileSetId)
			{
				m_TileSetInstanceId = tileSetId;
				LayerRenderer.ForceRedraw();
			}
		}

		private void DebugSetTileName() => m_DebugSelectedTileName = TileSet.GetPrefab(m_DrawBrush.TileSetIndex)?.name;

		private void ClampGridSize() => Grid?.ClampGridSize();
	}
}