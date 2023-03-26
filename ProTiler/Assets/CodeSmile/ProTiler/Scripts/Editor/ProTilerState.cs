// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler
{
	[FilePath(Global.TileEditorName + "/EditorState.settings", FilePathAttribute.Location.PreferencesFolder)]
	internal sealed class ProTilerState : ScriptableSingleton<ProTilerState>
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
			set => m_DrawTileSetIndex = math.max(value, Global.InvalidTileSetIndex);
		}

		// save on exit in case any property does not get immediately saved
		private void OnDisable() => Save(true);
	}
}