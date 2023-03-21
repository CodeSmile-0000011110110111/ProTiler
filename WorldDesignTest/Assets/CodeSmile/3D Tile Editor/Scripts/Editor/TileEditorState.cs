// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tile;
using UnityEditor;
using UnityEngine;

namespace CodeSmileEditor.Tile
{
	[FilePath(Global.TileEditorName + "/TileEditorState.settings", FilePathAttribute.Location.PreferencesFolder)]
	internal sealed class TileEditorState : ScriptableSingleton<TileEditorState>
	{
		[SerializeField] private EditMode m_EditMode = EditMode.Selection;
		[SerializeField] private int m_DrawingTileSetIndex;
		public EditMode EditMode
		{
			get => m_EditMode;
			set
			{
				m_EditMode = value;
				Save(true);
			}
		}
		public int DrawingTileSetIndex
		{
			get => m_DrawingTileSetIndex;
			set => m_DrawingTileSetIndex = value;
		}

		// save on exit in case any property does not get immediately saved
		private void OnDisable() => Save(true);
	}
}