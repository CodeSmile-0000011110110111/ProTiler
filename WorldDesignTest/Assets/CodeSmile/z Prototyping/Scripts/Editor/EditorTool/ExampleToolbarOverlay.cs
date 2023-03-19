// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

[Overlay(typeof(SceneView), "ElementToolbars Example")]
// IconAttribute provides a way to define an icon for when an Overlay is in collapsed form. If not provided, the name initials are used.
[Icon("Assets/unity.png")]
// Toolbar Overlays must inherit `ToolbarOverlay` and implement a parameter-less constructor.
// The contents of a toolbar are populated with string IDs, which are passed to the base constructor.
// IDs are defined by EditorToolbarElementAttribute.
public class ExampleToolbarOverlay : ToolbarOverlay
{
	// ToolbarOverlay implements a parameterless constructor, passing the EditorToolbarElementAttribute ID.
	// This is the only code required to implement a toolbar Overlay. Unlike panel Overlays, the contents are defined
	// as standalone pieces that will be collected to form a strip of elements.
	private ExampleToolbarOverlay()
		: base(ExampleCreateCubeToolbarButton.id, ExampleToolbarToggle.id, ExampleToolbarDropdown.id, ExampleToolbarDropdownToggle.id) {}
}