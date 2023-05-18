// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace CodeSmile.ProTiler.Tilemap
{
	public partial class Tilemap3DBehaviour
	{
		[Pure] internal void RegisterEditorEvents()
		{
#if UNITY_EDITOR
			UnregisterEditorSceneEvents();
			EditorSceneManager.sceneOpened += OnSceneOpened;
			EditorSceneManager.sceneSaving += OnSceneSaving;
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
			Undo.undoRedoPerformed += OnUndoRedoPerformed;
#endif
		}

		[Pure] internal void UnregisterEditorSceneEvents()
		{
#if UNITY_EDITOR
			EditorSceneManager.sceneOpened -= OnSceneOpened;
			EditorSceneManager.sceneSaving -= OnSceneSaving;
			AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
			Undo.undoRedoPerformed -= OnUndoRedoPerformed;
#endif
		}

#if UNITY_EDITOR
		/// <summary>
		///     Save the map before domain reload.
		/// </summary>
		[Pure] [ExcludeFromCodeCoverage] private void OnBeforeAssemblyReload() => SerializeTilemap();

		/// <summary>
		///     Save the map before saving the scene.
		/// </summary>
		[Pure] private void OnSceneSaving(Scene scene, String path) => SerializeTilemap();

		/// <summary>
		///     Restore the map after domain reload.
		/// </summary>
		[Pure] [ExcludeFromCodeCoverage] private void OnAfterAssemblyReload() => DeserializeTilemap();

		/// <summary>
		///     Restore the map after opening the scene.
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="mode"></param>
		[Pure] private void OnSceneOpened(Scene scene, OpenSceneMode mode) => DeserializeTilemap();

		/// <summary>
		///     Restore the map after Undo/Redo.
		///     This merely requires deserialization for both Undo and Redo because Unity's Undo/Redo system
		///     has already restored the MonoBehaviour's tilemap byte array to the expected map state,
		///     thus we only need to deserialize the tilemap from its buffer. Neat! :)
		/// </summary>
		[Pure] private void OnUndoRedoPerformed() => DeserializeTilemap();
#endif
	}
}
