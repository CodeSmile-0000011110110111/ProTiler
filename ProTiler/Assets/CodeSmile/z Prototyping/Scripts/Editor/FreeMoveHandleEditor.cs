// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

/*using System;
using UnityEditor;
using UnityEngine;
using UnityEditor_Editor = UnityEditor.Editor;

namespace CodeSmile.HandlesTest.UnityEditor
{
	[CustomEditor(typeof(FreeMoveHandle))] [CanEditMultipleObjects]
	public class FreeMoveHandleEditor : UnityEditor_Editor
	{
		private Vector3 m_IntersectPoint;

		private void OnSceneGUI()
		{
			var go = (FreeMoveHandle)target;
			var pos = go.transform.position;
			var size = go.UseGetHandleSize ? HandleUtility.GetHandleSize(pos) : go.HandleSize;
			//var snap = 1f;
			var snapVec = Vector3.one;
			var capFunc = GetCapFunction(go.HandleCap);

			EditorGUI.BeginChangeCheck();

			var newPos = Handles.FreeMoveHandle(go.FreeMovePosition, size, snapVec, capFunc);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(go, "Change pos");
				go.FreeMovePosition = newPos;

				if (go.MoveGameObject)
					go.transform.position = go.FreeMovePosition;
			}

			var evType = Event.current.type;
			if (evType == EventType.MouseMove)
			{
				var mousePos = Event.current.mousePosition;

				var picked = HandleUtility.PickGameObject(mousePos, false);
				if (picked != null)
					Debug.Log($"picked {picked.name}");
				else
				{
					//var ray = HandleUtility.GUIPointToWorldRay(mousePos);
					//if (ray.IntersectsPlane(out m_IntersectPoint)) Debug.Log($"hit virtual plane at {m_IntersectPoint}");
				}
			}
			else if (evType == EventType.Repaint)
				Handles.DrawWireCube(m_IntersectPoint, Vector3.one * .1f);
		}

		private void CustomCapFunction(int controlid, Vector3 position, Quaternion rotation, float size, EventType eventtype)
		{
			//Debug.Log($"controlId: {controlid}");

			if (eventtype == EventType.Layout)
				HandleUtility.AddControl(0, 10f);

			var prevColor = Handles.color;
			Handles.color = Color.cyan;

			Handles.DrawWireCube(position, new Vector3(size, size, size));
			Handles.DrawWireDisc(position, Vector3.up, size, Handles.lineThickness);
			Handles.DrawWireDisc(position, Vector3.forward, size, Handles.lineThickness);
			Handles.DrawWireDisc(position, Vector3.right, size, Handles.lineThickness);

			Handles.Label(position + Vector3.up * Handles.lineThickness, "Custom CapFunc");

			Handles.BeginGUI();
			GUILayout.BeginArea(new Rect(position + Vector3.right * 400f + Vector3.up * 300f, new Vector2(200, 50)));
			if (GUILayout.Button("Custom CapFunc Button", GUILayout.Width(200)))
				Debug.Log("Custom CapFunc Button clicked, yay!");
			GUILayout.EndArea();
			Handles.EndGUI();

			Handles.color = prevColor;
		}

		private Handles.CapFunction GetCapFunction(CapType capType) => capType switch
		{
			CapType.Arrow => Handles.ArrowHandleCap,
			CapType.Circle => Handles.CircleHandleCap,
			CapType.Cone => Handles.ConeHandleCap,
			CapType.Cube => Handles.CubeHandleCap,
			CapType.Cylinder => Handles.CylinderHandleCap,
			CapType.Dot => Handles.DotHandleCap,
			CapType.Rectangle => Handles.RectangleHandleCap,
			CapType.Sphere => Handles.SphereHandleCap,
			CapType.CustomCapFunction => CustomCapFunction,
			_ => throw new ArgumentOutOfRangeException(nameof(capType), capType, null),
		};
	}
}*/
