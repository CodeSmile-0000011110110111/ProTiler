// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

// Use [EditorToolbarElement(Identifier, EditorWindowType)] to register toolbar elements for use in ToolbarOverlay implementation.
[EditorToolbarElement(id, typeof(SceneView))]
internal class DropdownExample : EditorToolbarDropdown
{
	public const string id = "ExampleToolbar/Dropdown";

	private static string dropChoice;

	public DropdownExample()
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

[EditorToolbarElement(id, typeof(SceneView))]
internal class ToggleExample : EditorToolbarToggle
{
	public const string id = "ExampleToolbar/Toggle";

	public ToggleExample()
	{
		text = "Toggle OFF";
		this.RegisterValueChangedCallback(Test);
	}

	private void Test(ChangeEvent<bool> evt)
	{
		if (evt.newValue)
		{
			Debug.Log("ON");
			text = "Toggle ON";
		}
		else
		{
			Debug.Log("OFF");
			text = "Toggle OFF";
		}
	}
}

[EditorToolbarElement(id, typeof(SceneView))]
internal class DropdownToggleExample : EditorToolbarDropdownToggle, IAccessContainerWindow
{
	public const string id = "ExampleToolbar/DropdownToggle";
	private static int colorIndex;
	private static readonly Color[] colors = { Color.red, Color.green, Color.blue };

	// This property is specified by IAccessContainerWindow and is used to access the Overlay's EditorWindow.
	public EditorWindow containerWindow { get; set; }

	public DropdownToggleExample()
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
}

[EditorToolbarElement(id, typeof(SceneView))]
internal class CreateCube : EditorToolbarButton, IAccessContainerWindow
{
	// This ID is used to populate toolbar elements.
	public const string id = "ExampleToolbar/Button";

	// IAccessContainerWindow provides a way for toolbar elements to access the `EditorWindow` in which they exist.
	// Here we use `containerWindow` to focus the camera on our newly instantiated objects after creation.
	public EditorWindow containerWindow { get; set; }

	// Because this is a VisualElement, it is appropriate to place initialization logic in the constructor.
	// In this method you can also register to any additional events as required. In this example there is a tooltip, an icon, and an action.
	public CreateCube()
	{
		// A toolbar element can be either text, icon, or a combination of the two. Keep in mind that if a toolbar is
		// docked horizontally the text will be clipped, so usually it's a good idea to specify an icon.
		text = "Create Cube";
		icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CreateCubeIcon.png");
		Debug.Log($"icon is: {icon}");
		tooltip = "Instantiate a cube in the scene.";
		clicked += OnClick;
	}

	// This method will be invoked when the `Create Cube` button is clicked.
	private void OnClick()
	{
		var newObj = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
		newObj.gameObject.AddComponent<Rigidbody>();
		newObj.gameObject.AddComponent<SphereCollider>();

		// When writing editor tools don't forget to be a good citizen and implement Undo!
		Undo.RegisterCreatedObjectUndo(newObj.gameObject, "Create Cube");

		if (containerWindow is SceneView view)
			view.FrameSelected();
	}
}

// All Overlays must be tagged with the OverlayAttribute
[Overlay(typeof(SceneView), "ElementToolbars Example")]
// IconAttribute provides a way to define an icon for when an Overlay is in collapsed form. If not provided, the name initials are used.
[Icon("Assets/unity.png")]
// Toolbar Overlays must inherit `ToolbarOverlay` and implement a parameter-less constructor.
// The contents of a toolbar are populated with string IDs, which are passed to the base constructor.
// IDs are defined by EditorToolbarElementAttribute.
public class EditorToolbarExample : ToolbarOverlay
{
	// ToolbarOverlay implements a parameterless constructor, passing the EditorToolbarElementAttribute ID.
	// This is the only code required to implement a toolbar Overlay. Unlike panel Overlays, the contents are defined
	// as standalone pieces that will be collected to form a strip of elements.
	private EditorToolbarExample()
		: base(CreateCube.id, ToggleExample.id, DropdownExample.id, DropdownToggleExample.id) {}
}