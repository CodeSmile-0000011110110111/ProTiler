// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Tilemap;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Tilemap
{
	public class OnEventEditorBase : UnityEditor.Editor
	{
		public void OnSceneGUI()
		{
			switch (Event.current.type)
			{
				case EventType.Layout:
					break;
				case EventType.MouseMove:
					OnMouseMove();
					break;
				case EventType.MouseDrag:
					break;
				case EventType.MouseEnterWindow:
					break;
				case EventType.MouseLeaveWindow:
					break;
				case EventType.Repaint:
					break;
			}
		}

		protected virtual void OnMouseMove() {}
	}

	[ExcludeFromCodeCoverage] // don't test the UI, it's a 'detail'
	[CustomEditor(typeof(Tilemap3DDebugBehaviour))]
	public class Tilemap3DDebugBehaviourEditor : OnEventEditorBase
	{
		private Tilemap3DDebugBehaviour Target => target as Tilemap3DDebugBehaviour;

		protected override void OnMouseMove()
		{
			var currentMousePosition = Event.current.mousePosition;
			var lastMousePosition = currentMousePosition - Event.current.delta;
			var currentWorldRay = HandleUtility.GUIPointToWorldRay(currentMousePosition);
			var lastWorldRay = HandleUtility.GUIPointToWorldRay(lastMousePosition);
			Target.OnMouseMove(currentWorldRay, lastWorldRay);
		}
	}
}
