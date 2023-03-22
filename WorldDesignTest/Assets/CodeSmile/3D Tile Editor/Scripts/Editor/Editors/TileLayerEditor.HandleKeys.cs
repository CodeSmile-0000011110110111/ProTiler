// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tile;
using UnityEngine;

namespace CodeSmileEditor.Tile
{
	public partial class TileLayerEditor
	{
		private void HandleKeyDown()
		{
			UpdateClearingState();

			var shouldUseEvent = false;
			var keyCode = Event.current.keyCode;
			switch (keyCode)
			{
				case KeyCode.LeftArrow:
				case KeyCode.RightArrow:
				case KeyCode.UpArrow:
				case KeyCode.DownArrow:
					Layer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionNorth);
					Layer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionSouth);
					Layer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionEast);
					Layer.ClearTileFlags(m_CursorCoord, TileFlags.DirectionWest);
					break;
			}

			//Debug.Log($"{Event.current.type}: {Event.current.keyCode}" );

			switch (keyCode)
			{
				case KeyCode.F:
				{
					var camera = Camera.current;
					camera.transform.position = Layer.Grid.ToWorldPosition(m_CursorCoord);
					shouldUseEvent = true;
					break;
				}
				case KeyCode.H:
				{
					var tile = Layer.GetTileData(m_CursorCoord);
					if (tile.TileSetIndex < 0)
						break;

					if (tile.Flags.HasFlag(TileFlags.FlipHorizontal))
						Layer.ClearTileFlags(m_CursorCoord, TileFlags.FlipHorizontal);
					else
						Layer.SetTileFlags(m_CursorCoord, TileFlags.FlipHorizontal);
					break;
				}
				case KeyCode.V:
				{
					var tile = Layer.GetTileData(m_CursorCoord);
					if (tile.TileSetIndex < 0)
						break;

					if (tile.Flags.HasFlag(TileFlags.FlipVertical))
						Layer.ClearTileFlags(m_CursorCoord, TileFlags.FlipVertical);
					else
						Layer.SetTileFlags(m_CursorCoord, TileFlags.FlipVertical);
					break;
				}
				case KeyCode.LeftArrow:
					Layer.SetTileFlags(m_CursorCoord, TileFlags.DirectionWest);
					break;
				case KeyCode.RightArrow:
					Layer.SetTileFlags(m_CursorCoord, TileFlags.DirectionEast);
					break;
				case KeyCode.UpArrow:
					Layer.SetTileFlags(m_CursorCoord, TileFlags.DirectionNorth);
					break;
				case KeyCode.DownArrow:
					Layer.SetTileFlags(m_CursorCoord, TileFlags.DirectionSouth);
					break;
			}

			if (shouldUseEvent)
				Event.current.Use();
		}

		private void HandleKeyUp()
		{
			UpdateClearingState();
		}
	}
}