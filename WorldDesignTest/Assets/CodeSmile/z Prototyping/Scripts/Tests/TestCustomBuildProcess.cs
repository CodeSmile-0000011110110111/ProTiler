// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeSmile.EditorTests
{
	[InitializeOnLoad]
	public class TestCustomBuildProcess
	{
		private static string m_TempScene;
		private static BuildPlayerOptions ProcessBuildOptions(BuildPlayerOptions options)
		{
	
			// NOTE: this will open a dialog where the user can select the location of the build to be made.
			options = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(options);

			// remember the scene the user had open
			var userOpenScenePath = SceneManager.GetActiveScene().path;
			Debug.Log($"open scene: {userOpenScenePath}");

			var firstScene = options.scenes[0];
			
			// make temp copy of scene
			var tempScene = firstScene.Replace(".unity", "_temp_build_copy.unity");
			m_TempScene = tempScene;
			AssetDatabase.CopyAsset(firstScene, tempScene);

			var tempGuid = AssetDatabase.GUIDFromAssetPath(tempScene);
			var tempGuid2 = AssetDatabase.AssetPathToGUID(tempScene);
			Debug.Log($"tempGuid: {tempGuid} ({tempGuid.ToString().Length}) / {tempGuid2} ({tempGuid2.ToString().Length})");

			 tempGuid = AssetDatabase.GUIDFromAssetPath(tempScene + "notexist");
			 tempGuid2 = AssetDatabase.AssetPathToGUID(tempScene + "notexist");
			Debug.Log($"not exist tempGuid: {tempGuid} / {tempGuid2}");

			//var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(tempScene);
			//Debug.Log($"temp scene asset: {sceneAsset.name}");
			
			// replace scene in build list
			options.scenes = new[] { tempScene };
			
			// open the temp scene
			//Debug.Log($"scene: {tempScene}, open for editing ...");
			EditorSceneManager.OpenScene(tempScene, OpenSceneMode.Single);

			// modify temp scene
			var replaceMe = GameObject.FindGameObjectsWithTag("ReplaceMe");
			for (int i = replaceMe.Length - 1; i >= 0; i--)
			{
				var go = replaceMe[i];
				Debug.LogWarning($"removing unwanted object from build: {go.name}");
				GameObject.DestroyImmediate(go);
			}

			EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), tempScene, false);
			
			// re-open the last scene the user had open just because we're trying not to be annoying, right ;)
			Debug.Log($"re-open scene: {userOpenScenePath}");
			EditorSceneManager.OpenScene(userOpenScenePath);
			
			return options;
		}

		private static void OnClickBuildPlayer(BuildPlayerOptions options)
		{
			BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
			Debug.Log("perform default build - DONE");
			if (string.IsNullOrWhiteSpace(m_TempScene) == false)
			{
				AssetDatabase.DeleteAsset(m_TempScene);
			}
		}

		static TestCustomBuildProcess()
		{
			BuildPlayerWindow.RegisterGetBuildPlayerOptionsHandler(ProcessBuildOptions);
			BuildPlayerWindow.RegisterBuildPlayerHandler(OnClickBuildPlayer);
		}
	}
}
#endif
