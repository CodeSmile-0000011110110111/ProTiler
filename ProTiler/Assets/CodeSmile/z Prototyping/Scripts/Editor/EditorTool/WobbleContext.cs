// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

/*
using System;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorToolContext("Wobbly Transform Tools", typeof(Platform2))]
// The icon path can also be used with packages. Ex "Packages/com.wobblestudio.wobblytools/Icons/Transform.png".
//[Icon("Assets/Examples/Icons/TransformIcon.png")]
public class WobbleContext : EditorToolContext
{
	// Tool contexts can also implement an OnToolGUI function that is invoked before tools. This is a good place to
	// add any custom selection logic, for example.
	public override void OnToolGUI(EditorWindow _) {}

	protected override Type GetEditorToolType(Tool tool)
	{
		Debug.Log($"Tool: {tool}");
		switch (tool)
		{
			// Return the type of tool to be used for Tool.Move. The Tool Manager will handle instantiating and
			// activating the tool.
			case Tool.Move:
				return typeof(WobblyMoveTool);
			// For any tools that are not implemented, return null to disable the tool in the menu.
			case Tool.View: break;
			case Tool.Rotate: break;
			case Tool.Scale: break;
			case Tool.Rect: break;
			case Tool.Transform: break;
			case Tool.Custom: break;
			case Tool.None: break;
			default:
				return null;
		}
		return null;
	}
}
*/
