// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace CodeSmile.Editor.Tools
{
	public static class ReloadActiveScene
	{
		private const string MenuItemText = "Tools/CodeSmile/Reload Scene #%r";

		[MenuItem(MenuItemText)]
		public static void ReloadScene()
		{
			EditorSceneManager.SaveOpenScenes();
			var activeScene = SceneManager.GetActiveScene();
			EditorSceneManager.OpenScene(activeScene.path);
		}
	}
}