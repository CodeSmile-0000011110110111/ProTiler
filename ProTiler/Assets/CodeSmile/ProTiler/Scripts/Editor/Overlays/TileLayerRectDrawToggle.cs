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
	internal class TileLayerRectDrawToggle : TileLayerToolbarRadioToggle
	{
		public const string Id = Global.TileEditorName + "/RectDrawing";

		public TileLayerRectDrawToggle()
		{
			value = ProTilerState.instance.TileEditMode == TileEditMode.RectFill;
			tooltip = "Rectangle Fill Tiles";

			onIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Global.OverlayIconPath + "TileRectDrawingOnIcon.png");
			offIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Global.OverlayIconPath + "TileRectDrawingOffIcon.png");
			this.RegisterValueChangedCallback(OnToggleChange);
		}

		private void OnToggleChange(ChangeEvent<bool> evt)
		{
			ProTilerState.instance.TileEditMode = TileEditMode.RectFill;
			//EditorPrefs.SetInt(Global.EditorPrefEditMode, (int)EditMode.RectFill);
		}
	}
}