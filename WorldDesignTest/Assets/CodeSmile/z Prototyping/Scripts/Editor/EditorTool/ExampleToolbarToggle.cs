// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

[EditorToolbarElement(id, typeof(SceneView))]
internal class ExampleToolbarToggle : EditorToolbarToggle
{
	public const string id = "ExampleToolbar/Toggle";

	public ExampleToolbarToggle()
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