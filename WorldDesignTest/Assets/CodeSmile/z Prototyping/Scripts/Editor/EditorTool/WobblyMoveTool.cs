// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

// EditorToolContextAttribute is what registers a context with the UI.
// Note that tools used by an EditorToolContext do not need to use EditorToolAttribute.
internal class WobblyMoveTool : EditorTool
{
	private Vector3 m_Origin;
	private readonly List<Selected> m_Selected = new();

	private void StartMove(Vector3 origin)
	{
		Debug.Log($"StartMove {origin}");
		m_Origin = origin;
		m_Selected.Clear();
		foreach (var trs in Selection.transforms)
			m_Selected.Add(new Selected { transform = trs, localScale = trs.localScale });
		Undo.RecordObjects(Selection.transforms, "Wobble Move Tool");
	}

	// This is silly example that oscillates the scale of the selected objects as they are moved.
	public override void OnToolGUI(EditorWindow _)
	{
		var evt = Event.current.type;
		var hot = GUIUtility.hotControl;
		EditorGUI.BeginChangeCheck();
		var p = Handles.PositionHandle(Tools.handlePosition, Tools.handleRotation);
		if (evt == EventType.MouseDown && hot != GUIUtility.hotControl)
			StartMove(p);
		if (EditorGUI.EndChangeCheck())
		{
			foreach (var selected in m_Selected)
			{
				selected.transform.position += p - Tools.handlePosition;
				var scale = Vector3.one * (Mathf.Sin(Mathf.Abs(Vector3.Distance(m_Origin, p))) * .5f);
				selected.transform.localScale = selected.localScale + scale;
			}
		}
	}

	private struct Selected
	{
		public Transform transform;
		public Vector3 localScale;
	}
}