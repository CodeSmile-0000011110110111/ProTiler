// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmile.Editor.ProTiler.Overlays
{
	[EditorToolbarElement(Id, typeof(SceneView))]
	internal class TileLayerSelectionToggle : TileLayerToolbarRadioToggle
	{
		public const string Id = Names.TileEditor + "/Selection";

		[Shortcut(Names.TileEditor + "/Tile Selection Tool", typeof(SceneView), KeyCode.S)]
		private static void TileWorldSelectionShortcut()
		{
			Debug.Log("selection shortcut!");
			TileLayerOverlay.SetToggleOn(typeof(TileLayerSelectionToggle));
		}

		public TileLayerSelectionToggle()
		{
			value = ProTilerState.instance.TileEditMode == TileEditMode.Selection;
			tooltip = "Select Tiles";

			onIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Paths.OverlayIcon + "TileSelectionOnIcon.png");
			offIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Paths.OverlayIcon + "TileSelectionOffIcon.png");
			this.RegisterValueChangedCallback(OnToggleChange);
		}

		private void OnToggleChange(ChangeEvent<bool> evt)
		{
			if (evt.newValue)
			{
				ProTilerState.instance.TileEditMode = TileEditMode.Selection;
				//EditorPrefs.SetInt(Global.EditorPrefEditMode, (int)EditMode.Selection);
				//EditorWindow.GetWindow<TileWorldEditor>();
				//var editor = Editor.FindObjectOfType<TileWorldEditor>();
				//Debug.Log($"found: {editor}");
			}
		}
	}
}