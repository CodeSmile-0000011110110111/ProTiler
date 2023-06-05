// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace CodeSmile.ProTiler.Controller
{
	public sealed partial class Tilemap3DModel
	{
		private void StartRecordUndo(String undoGroupName, String undoActionName)
		{
#if UNITY_EDITOR
			if (EditorApplication.isPlaying == false)
			{
				Undo.SetCurrentGroupName(undoGroupName);
				Undo.RecordObject(this, undoActionName);
			}
#endif
		}

		private void EndRecordUndo()
		{
#if UNITY_EDITOR
			if (EditorApplication.isPlaying == false)
			{
				SerializeTilemap();
				m_UndoGroupRegistry.RegisterCurrentUndoGroup();
				Undo.IncrementCurrentGroup();
			}
#endif
		}

		private void RegisterEditorSceneEvents()
		{
#if UNITY_EDITOR
			UnregisterEditorSceneEvents();
			m_UndoGroupRegistry.RegisterUndoRedoEvents(OnRegisteredUndoRedoEvent);
			EditorSceneManager.sceneOpened += OnSceneOpened;
			EditorSceneManager.sceneSaving += OnSceneSaving;
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
#endif
		}

		private void UnregisterEditorSceneEvents()
		{
#if UNITY_EDITOR
			m_UndoGroupRegistry.UnregisterUndoRedoEvents(OnRegisteredUndoRedoEvent);
			EditorSceneManager.sceneOpened -= OnSceneOpened;
			EditorSceneManager.sceneSaving -= OnSceneSaving;
			AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
#endif
		}

#if UNITY_EDITOR
		private void OnRegisteredUndoRedoEvent() => DeserializeTilemap();
		[ExcludeFromCodeCoverage] private void OnBeforeAssemblyReload() => SerializeTilemap();
		private void OnSceneSaving(Scene scene, String path) => SerializeTilemap();
		[ExcludeFromCodeCoverage] private void OnAfterAssemblyReload() => DeserializeTilemap();
		private void OnSceneOpened(Scene scene, OpenSceneMode mode) => DeserializeTilemap();
#endif
	}
}
