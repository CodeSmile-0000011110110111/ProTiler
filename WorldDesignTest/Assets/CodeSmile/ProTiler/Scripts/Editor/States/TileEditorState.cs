// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tile;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace CodeSmileEditor.Tile
{
	[FilePath(Const.TileEditorName + "/TileEditorState.settings", FilePathAttribute.Location.PreferencesFolder)]
	internal sealed class TileEditorState : ScriptableSingleton<TileEditorState>
	{
		[SerializeField] private TileEditMode m_TileEditMode = TileEditMode.Selection;
		[SerializeField] private int m_DrawTileSetIndex;
		public TileEditMode TileEditMode
		{
			get => m_TileEditMode;
			set
			{
				m_TileEditMode = value;
				Save(true);
			}
		}
		public int DrawTileSetIndex
		{
			get => m_DrawTileSetIndex;
			set => m_DrawTileSetIndex = math.max(value, Const.InvalidTileSetIndex);
		}

		// save on exit in case any property does not get immediately saved
		private void OnDisable() => Save(true);
	}
}