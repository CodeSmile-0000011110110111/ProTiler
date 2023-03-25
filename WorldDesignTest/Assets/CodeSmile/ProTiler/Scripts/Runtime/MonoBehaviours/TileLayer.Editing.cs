// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Mathematics;
using UnityEngine;
using GridCoord = Unity.Mathematics.int3;
using GridSize = Unity.Mathematics.int3;
using GridRect = UnityEngine.RectInt;
using WorldRect = UnityEngine.Rect;

namespace CodeSmile.Tile
{
	public sealed partial class TileLayer
	{
		public bool ModifyTileAttributes(GridCoord coord, float scrollDelta, bool shift, bool ctrl)
		{
			var useEvent = false;
			var delta = scrollDelta >= 0 ? 1 : -1;

			if (shift && ctrl)
			{
				FlipTile(coord, delta);
				useEvent = true;
			}
			else if (shift)
			{
				RotateTile(coord, delta);
				useEvent = true;
			}
			else if (ctrl)
			{
				IncrementDrawTileSetIndex(delta);
				useEvent = true;
			}

			Debug.Log($"ModifyTile {coord} {delta}, shift:{shift} ctrl:{ctrl} use: {useEvent}");
			return useEvent;
		}
		
		private void IncrementDrawTileSetIndex(int delta) => DrawTileSetIndex += delta;
		private int DrawTileSetIndex
		{
			get => m_DrawBrush.TileSetIndex;
			set => m_DrawBrush.TileSetIndex = math.clamp(value, 0, TileSet.Count - 1);
		}
	}
}