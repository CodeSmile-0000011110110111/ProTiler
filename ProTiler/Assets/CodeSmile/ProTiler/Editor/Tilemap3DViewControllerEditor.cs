﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using CodeSmile.ProTiler.Controller;
using CodeSmile.ProTiler.Events;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor
{
	[SuppressMessage("NDepend", "ND1204:OverridesOfMethodShouldCallBaseMethod", Justification="not expected")]
	[ExcludeFromCodeCoverage] // don't test the UI, it's a 'detail'
	[CustomEditor(typeof(Tilemap3DViewController))]
	public class Tilemap3DViewControllerEditor : OnSceneEventEditorBase
	{
		private ITilemap3DViewController Target => target as ITilemap3DViewController;

		protected override void OnMouseMove(Event evt)
		{
			var currentMousePosition = Event.current.mousePosition;
			var lastMousePosition = currentMousePosition - Event.current.delta;

			var currentWorldRay = HandleUtility.GUIPointToWorldRay(currentMousePosition);
			var lastWorldRay = HandleUtility.GUIPointToWorldRay(lastMousePosition);
			Target.OnMouseMove(new MouseMoveEventData(currentWorldRay, lastWorldRay));
		}
	}
}