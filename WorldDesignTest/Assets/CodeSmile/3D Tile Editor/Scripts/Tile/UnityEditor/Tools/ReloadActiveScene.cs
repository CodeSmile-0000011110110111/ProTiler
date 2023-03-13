// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace CodeSmile.Tile.UnityEditor
{
	public static class ReloadActiveScene
	{
		private const string MenuItemText = "CodeSmile/Reload Scene #%r";

		[MenuItem(MenuItemText)]
		public static void ReloadScene() => EditorSceneManager.OpenScene(SceneManager.GetActiveScene().path);
	}
}