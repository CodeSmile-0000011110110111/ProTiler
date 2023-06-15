// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeSmile.ProTiler.Runtime.CodeDesign._old.Controller
{
	public sealed class UnityEditorEventsController : ScriptableObject
	{
#if UNITY_EDITOR
		private void OnEnable()
		{
			EditorApplication.update += EditorUpdate;
			Undo.undoRedoEvent += OnUndoRedoEvent;
			EditorSceneManager.sceneSaving += OnSceneWillSave;
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
		}

		private void EditorUpdate() {}

		private void OnSceneWillSave(Scene scene, String path) {}

		private void OnBeforeAssemblyReload() {}
		private void OnAfterAssemblyReload() {}

		private void OnUndoRedoEvent(in UndoRedoInfo undo) =>
			Debug.Log($"UndoRedo #{undo.undoGroup} '{undo.undoName}', redo: {undo.isRedo}");
#endif
	}
}
