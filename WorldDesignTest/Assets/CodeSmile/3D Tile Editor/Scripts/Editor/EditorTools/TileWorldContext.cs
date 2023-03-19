// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tile;
using CodeSmileEditor.Tile;
using System;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorToolContext(Global.TileEditorName + " World Tools", typeof(TileWorld))]
[Icon(Global.EditorToolsIconPath + "TileWorldEditorTools.png")]
public sealed class TileWorldContext : EditorToolContext
{
	// Tool contexts can also implement an OnToolGUI function that is invoked before tools. This is a good place to
	// add any custom selection logic, for example.
	public override void OnToolGUI(EditorWindow _) {}

	protected override Type GetEditorToolType(Tool tool)
	{
		//Debug.Log($"World Tool: {tool}");
		return tool switch
		{
			Tool.Move => typeof(TileWorldMoveTool),
			Tool.View => null,
			Tool.Rotate => null,
			Tool.Scale => null,
			Tool.Rect => null,
			Tool.Transform => null,
			Tool.Custom => null,
			Tool.None => null,
			_ => null
		};
	}
}