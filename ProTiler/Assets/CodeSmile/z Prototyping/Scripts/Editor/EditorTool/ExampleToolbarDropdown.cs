// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

// Use [EditorToolbarElement(Identifier, EditorWindowType)] to register toolbar elements for use in ToolbarOverlay implementation.
[EditorToolbarElement(id, typeof(SceneView))]
internal class ExampleToolbarDropdown : EditorToolbarDropdown
{
	public const string id = "ExampleToolbar/Dropdown";

	private static string dropChoice;

	public ExampleToolbarDropdown()
	{
		text = "Axis";
		clicked += ShowDropdown;
	}

	private void ShowDropdown()
	{
		var menu = new GenericMenu();
		menu.AddItem(new GUIContent("X"), dropChoice == "X", () =>
		{
			text = "X";
			dropChoice = "X";
		});
		menu.AddItem(new GUIContent("Y"), dropChoice == "Y", () =>
		{
			text = "Y";
			dropChoice = "Y";
		});
		menu.AddItem(new GUIContent("Z"), dropChoice == "Z", () =>
		{
			text = "Z";
			dropChoice = "Z";
		});
		menu.ShowAsContext();
	}
}

// All Overlays must be tagged with the OverlayAttribute