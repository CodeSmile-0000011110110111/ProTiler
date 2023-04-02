// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using System;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace CodeSmile.Editor.ProTiler.EditorTools
{
	[EditorToolContext(Names.TileEditor + " Layer Tools", typeof(TileLayer))]
	[Icon(Paths.EditorToolsIcon + "TileWorldEditorTools.png")]
	public sealed class TileLayerContext : EditorToolContext
	{
		// Tool contexts can also implement an OnToolGUI function that is invoked before tools. This is a good place to
		// add any custom selection logic, for example.
		public override void OnToolGUI(EditorWindow _) {}

		protected override Type GetEditorToolType(Tool tool) =>
			//Debug.Log($"World Tool: {tool}");
			tool switch
			{
				Tool.Move => typeof(TileLayerMoveTool),
				Tool.View => null,
				Tool.Rotate => null,
				Tool.Scale => null,
				Tool.Rect => null,
				Tool.Transform => null,
				Tool.Custom => null,
				Tool.None => null,
				_ => null,
			};
	}
}