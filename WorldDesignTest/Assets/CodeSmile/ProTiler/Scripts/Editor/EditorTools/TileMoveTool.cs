// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using Unity.Mathematics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace CodeSmileEditor.Tile
{
	internal class TileMoveTool : EditorTool
	{
		private Vector3 m_Origin;
		private Vector3 m_Current;

		private void OnEnable()
		{
			Debug.Log($"{GetType()} OnEnable");
		}

		private void OnDisable()
		{
			Debug.Log($"{GetType()} OnDisable");
		}

		public override void OnActivated()
		{
			base.OnActivated();
			Debug.Log($"{GetType()} activated");
		}

		private void StartMove(Vector3 origin)
		{
			//Debug.Log($"Tile StartMove {origin}");
			m_Origin = origin;
			Undo.RecordObjects(Selection.transforms, "Tile Move");
		}

		public override void OnToolGUI(EditorWindow window)
		{
			var evType = Event.current.type;
			var control = GUIUtility.hotControl;

			EditorGUI.BeginChangeCheck();
			if (evType == EventType.MouseDown && control != GUIUtility.hotControl)
				StartMove(Tools.handlePosition);

			var pos = Handles.PositionHandle(Tools.handlePosition, Tools.handleRotation);

			if (EditorGUI.EndChangeCheck())
			{
				var scale = 1f;
				var diff = pos - m_Origin;
				//diff = Handles.SnapValue(diff, Vector3.one * 10);
				//diff.x = Handles.SnapValue(diff.x, 10f);
				diff.x = math.round(diff.x);
				Debug.Log($"handle: {Tools.handlePosition}, new: {pos}, diff: {diff} - {target} {target.GetType()}");

				//m_Origin += diff;
				/*
				diff.x = math.round(diff.x) * scale;
				diff.y = math.round(diff.y) * scale;
				diff.z = math.round(diff.z) * scale;
				*/

				foreach (var selected in Selection.transforms)
				{
					//selected.position += pos - m_Origin;
					selected.position += diff;
					//Debug.Log($"handle origin : {m_Origin}");
					//Debug.Log($"handle current: {selected.position}");
				}
			}
		}
	}
}