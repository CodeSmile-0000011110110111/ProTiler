// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Tilemap;
using CodeSmile.ProTiler.Utility;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Tilemap
{
	[CustomEditor(typeof(Tilemap3DDebugBehaviour))]
	public class Tilemap3DDebugBehaviourEditor : UnityEditor.Editor
	{
		private Tilemap3DDebugBehaviour Target => target as Tilemap3DDebugBehaviour;

		private static Ray ToWorldRay(Vector2 mousePosition) => HandleUtility.GUIPointToWorldRay(mousePosition);

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

		private void OnMouseMove()
		{
			var mousePos = Event.current.mousePosition;
			var mouseWorldRay = ToWorldRay(mousePos);
			var lastMouseWorldRay =ToWorldRay(mousePos - Event.current.delta);
			Target.OnMouseMove(mouseWorldRay, lastMouseWorldRay);
		}
	}
}
