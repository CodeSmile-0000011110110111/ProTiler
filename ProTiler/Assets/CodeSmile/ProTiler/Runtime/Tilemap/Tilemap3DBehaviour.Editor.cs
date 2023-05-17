// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.Contracts;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace CodeSmile.ProTiler.Tilemap
{
	public partial class Tilemap3DBehaviour
	{
		[Pure] internal void RegisterEditorSceneEvents()
		{
#if UNITY_EDITOR
			UnregisterEditorSceneEvents();
			EditorSceneManager.sceneOpened += OnSceneOpened;
			EditorSceneManager.sceneSaving += OnSceneSaving;
#endif
		}

		[Pure] internal void UnregisterEditorSceneEvents()
		{
#if UNITY_EDITOR
			EditorSceneManager.sceneOpened -= OnSceneOpened;
			EditorSceneManager.sceneSaving -= OnSceneSaving;
#endif
		}

#if UNITY_EDITOR
		[Pure] private void OnSceneOpened(Scene scene, OpenSceneMode mode) => LoadTilemap();
		[Pure] private void OnSceneSaving(Scene scene, String path) => SaveTilemap();
#endif
	}
}
