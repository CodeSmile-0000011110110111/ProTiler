// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

/*using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

[EditorToolbarElement(id, typeof(SceneView))]
internal class ExampleToolbarDropdownToggle : EditorToolbarDropdownToggle, IAccessContainerWindow
{
	public const string id = "ExampleToolbar/DropdownToggle";
	private static int colorIndex;
	private static readonly Color[] colors = { Color.red, Color.green, Color.blue };

	// This property is specified by IAccessContainerWindow and is used to access the Overlay's EditorWindow.
	public EditorWindow containerWindow { get; set; }

	public ExampleToolbarDropdownToggle()
	{
		text = "Color Bar";
		tooltip = "Display a color rectangle in the top left of the Scene view. Toggle on or off, and open the dropdown" +
		          "to change the color.";

		// When the dropdown is opened, ShowColorMenu is invoked and we can create a popup menu.
		dropdownClicked += ShowColorMenu;

		// Subscribe to the Scene view OnGUI callback so that we can draw our color swatch.
		SceneView.duringSceneGui += DrawColorSwatch;
	}

	private void DrawColorSwatch(SceneView view)
	{
		// Test that this callback is for the Scene View that we're interested in, and also check if the toggle is on
		// or off (value).
		if (view != containerWindow || !value)
			return;

		Handles.BeginGUI();
		var prevColor = GUI.color;
		GUI.color = colors[colorIndex];
		GUI.DrawTexture(new Rect(8, 8, 120, 24), Texture2D.whiteTexture);
		Handles.color = GUI.color;
		Handles.DrawWireCube(new Vector3(5, 0, 7), Vector3.one * 100);
		GUI.color = prevColor;
		Handles.EndGUI();
	}

	// When the dropdown button is clicked, this method will create a popup menu at the mouse cursor position.
	private void ShowColorMenu()
	{
		var menu = new GenericMenu();
		menu.AddItem(new GUIContent("Red"), colorIndex == 0, () => colorIndex = 0);
		menu.AddItem(new GUIContent("Green"), colorIndex == 1, () => colorIndex = 1);
		menu.AddItem(new GUIContent("Blue"), colorIndex == 2, () => colorIndex = 2);
		menu.ShowAsContext();
	}
}*/
