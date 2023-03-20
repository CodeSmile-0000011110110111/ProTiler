// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tile;
using System;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.ShortcutManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmileEditor.Tile
{
	[Overlay(typeof(SceneView), Global.TileEditorName, Global.TileEditorName)]
	[Icon(Global.OverlayIconPath + "OverlayToolbarIcon.png")]
	public class TileWorldOverlay : ToolbarOverlay
	{
		public static void SetToggleOn(Type toggleType)
		{
			SceneView.lastActiveSceneView.TryGetOverlay(Global.TileEditorName, out var overlay);
			var tileWorldOverlay = overlay as TileWorldOverlay;
			if (tileWorldOverlay != null)
			{
				// TODO: how to access the overlay's elements??
				
				/*
				foreach (var element in tileWorldOverlay.)
				{
					var otherToggle = element as TileEditorToolbarRadioToggle;
					if (otherToggle != null && otherToggle != toggle)
						otherToggle.SetValueWithoutNotify(false);
				}
			*/
			}
		}

		private TileWorldOverlay()
			: base(TileWorldSelectionToggle.Id, TileWorldPenDrawingToggle.Id, TileWorldRectDrawingToggle.Id) {}
	}

	internal class TileEditorToolbarRadioToggle : EditorToolbarToggle
	{
		internal static void TurnAllToolbarTogglesOff(VisualElement toggle)
		{
			foreach (var element in toggle.parent.Children())
			{
				var otherToggle = element as TileEditorToolbarRadioToggle;
				if (otherToggle != null && otherToggle != toggle)
					otherToggle.SetValueWithoutNotify(false);
			}
		}

		public TileEditorToolbarRadioToggle()
		{
			this.RegisterValueChangedCallback(OnRadioToggleChange);
		}

		private void OnRadioToggleChange(ChangeEvent<bool> evt)
		{
			if (evt.newValue)
				TurnAllToolbarTogglesOff(this);
			else
				SetValueWithoutNotify(true);
		}
	}

	[EditorToolbarElement(Id, typeof(SceneView))]
	internal class TileWorldSelectionToggle : TileEditorToolbarRadioToggle
	{
		public const string Id = Global.TileEditorName + "/Selection";

		[Shortcut(Global.TileEditorName + "/Tile Selection Tool", typeof(SceneView), KeyCode.S)]
		private static void TileWorldSelectionShortcut()
		{
			Debug.Log("selection shortcut!");
			TileWorldOverlay.SetToggleOn(typeof(TileWorldSelectionToggle));
		}

		public TileWorldSelectionToggle()
		{
			value = TileEditorState.instance.EditMode == EditMode.Selection;
			tooltip = "Select Tiles";

			onIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Global.OverlayIconPath + "TileSelectionOnIcon.png");
			offIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Global.OverlayIconPath + "TileSelectionOffIcon.png");
			this.RegisterValueChangedCallback(OnToggleChange);
		}

		private void OnToggleChange(ChangeEvent<bool> evt)
		{
			if (evt.newValue)
			{
				TileEditorState.instance.EditMode = EditMode.Selection;
				//EditorPrefs.SetInt(Global.EditorPrefEditMode, (int)EditMode.Selection);
				//EditorWindow.GetWindow<TileWorldEditor>();
				//var editor = Editor.FindObjectOfType<TileWorldEditor>();
				//Debug.Log($"found: {editor}");
			}
		}
	}

	[EditorToolbarElement(Id, typeof(SceneView))]
	internal class TileWorldPenDrawingToggle : TileEditorToolbarRadioToggle
	{
		public const string Id = Global.TileEditorName + "/PenDrawing";

		public TileWorldPenDrawingToggle()
		{
			value = TileEditorState.instance.EditMode == EditMode.PenDraw;
			tooltip = "Draw Tiles";

			onIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Global.OverlayIconPath + "TilePenDrawingOnIcon.png");
			offIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Global.OverlayIconPath + "TilePenDrawingOffIcon.png");
			this.RegisterValueChangedCallback(OnToggleChange);
		}

		private void OnToggleChange(ChangeEvent<bool> evt)
		{
			TileEditorState.instance.EditMode = EditMode.PenDraw;
			//EditorPrefs.SetInt(Global.EditorPrefEditMode, (int)EditMode.PenDraw);
		}
	}

	[EditorToolbarElement(Id, typeof(SceneView))]
	internal class TileWorldRectDrawingToggle : TileEditorToolbarRadioToggle
	{
		public const string Id = Global.TileEditorName + "/RectDrawing";

		public TileWorldRectDrawingToggle()
		{
			value = TileEditorState.instance.EditMode == EditMode.RectFill;
			tooltip = "Rectangle Fill Tiles";

			onIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Global.OverlayIconPath + "TileRectDrawingOnIcon.png");
			offIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(Global.OverlayIconPath + "TileRectDrawingOffIcon.png");
			this.RegisterValueChangedCallback(OnToggleChange);
		}

		private void OnToggleChange(ChangeEvent<bool> evt)
		{
			TileEditorState.instance.EditMode = EditMode.RectFill;
			//EditorPrefs.SetInt(Global.EditorPrefEditMode, (int)EditMode.RectFill);
		}
	}
}