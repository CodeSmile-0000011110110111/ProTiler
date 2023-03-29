// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmile.Editor.ProTiler.Overlays
{
	[EditorToolbarElement(Id, typeof(SceneView))]
	internal class TileLayerPenDrawToggle : TileLayerToolbarRadioToggle
	{
		public const string Id = Global.TileEditorName + "/PenDrawing";

		public TileLayerPenDrawToggle()
		{
			value = ProTilerState.instance.TileEditMode == TileEditMode.PenDraw;
			tooltip = "Draw Tiles";

			onIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Global.OverlayIconPath + "TilePenDrawingOnIcon.png");
			offIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Global.OverlayIconPath + "TilePenDrawingOffIcon.png");
			this.RegisterValueChangedCallback(OnToggleChange);
		}

		private void OnToggleChange(ChangeEvent<bool> evt)
		{
			ProTilerState.instance.TileEditMode = TileEditMode.PenDraw;
			//EditorPrefs.SetInt(Global.EditorPrefEditMode, (int)EditMode.PenDraw);
		}
	}
}