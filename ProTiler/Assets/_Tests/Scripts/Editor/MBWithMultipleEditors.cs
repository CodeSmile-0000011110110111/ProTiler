// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEngine;

namespace _Tests.Scripts.Editor
{
	[CustomEditor(typeof(MBWithMultipleEditors))]
	public class MBWithMultipleEditorsEditor1 : UnityEditor.Editor
	{
		private UnityEditor.Editor m_CachedEditor;
		private MBWithMultipleEditorsEditor222 m_SecondEditor;

		private void OnEnable()
		{
			//m_SecondEditor = CreateEditor(target, typeof(MBWithMultipleEditorsEditor222)) as MBWithMultipleEditorsEditor222;
			//m_SecondEditor = CreateEditorWithContext(new []{target}, this, typeof(MBWithMultipleEditorsEditor222)) as MBWithMultipleEditorsEditor222;
			CreateCachedEditor(target, typeof(MBWithMultipleEditorsEditor222), ref m_CachedEditor);
			m_SecondEditor = m_CachedEditor as MBWithMultipleEditorsEditor222;
		}

		private void OnDestroy()
		{
			if (m_SecondEditor != null)
				DestroyImmediate(m_SecondEditor);
		}

		private void OnSceneGUI()
		{
			Debug.Log("OnSceneGUI: " + nameof(MBWithMultipleEditorsEditor1));

			m_SecondEditor.OnSceneGUI();
		}
	}

	[CustomEditor(typeof(MBWithMultipleEditors))]
	public class MBWithMultipleEditorsEditor222 : UnityEditor.Editor
	{
		internal void OnSceneGUI() => Debug.Log($"OnSceneGUI: {nameof(MBWithMultipleEditorsEditor222)}, " +
		                                        $"target: {target.GetType()}");
	}
}
