// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using CodeSmile.ProTiler3.Controller;
using CodeSmile.ProTiler3.Events;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler3.Editor
{
	[SuppressMessage("NDepend", "ND1204:OverridesOfMethodShouldCallBaseMethod", Justification = "not expected")]
	[ExcludeFromCodeCoverage] // don't test the UI, it's a 'detail'
	[CustomEditor(typeof(Tilemap3DViewController))]
	public class Tilemap3DViewControllerEditor : OnSceneEventEditorBase
	{
		private ITilemap3DViewController Target => target as ITilemap3DViewController;

		protected override void OnMouseMove(Event evt)
		{
			var worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Target.OnMouseMove(new MouseMoveEventData(worldRay));
		}

		protected override void OnMouseEnterWindow(Event evt) => Target.EnableCursor();

		protected override void OnMouseLeaveWindow(Event evt) => Target.DisableCursor();
	}
}
