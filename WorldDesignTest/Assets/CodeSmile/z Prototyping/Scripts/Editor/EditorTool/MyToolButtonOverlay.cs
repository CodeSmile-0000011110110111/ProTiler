// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "MyToolButton Overlay", true)]
public class MyToolButtonOverlay : Overlay
{
	public override VisualElement CreatePanelContent()
	{
		var root = new VisualElement { name = "My Toolbar Root" };
		root.Add(new Label { text = "Hello" });
		var r = new RadioButton();
		root.Add(r);
		 r = new RadioButton();
		root.Add(r);
		var radio = new RadioButtonGroup(null, new List<string>() { "one", "two", "trhee" });
		root.Add(radio);
		var button = new Button { focusable = true, text = "Button" };
		root.Add(button);
		return root;
	}
}