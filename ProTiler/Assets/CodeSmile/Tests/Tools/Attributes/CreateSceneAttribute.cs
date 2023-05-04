// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor.Extensions;
using CodeSmile.Extensions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Tools.Attributes
{
	public abstract class CreateSceneAttribute : NUnitAttribute, IOuterUnityTestAction
	{
		private readonly NewSceneSetup m_Setup;
		private string m_ScenePath;

		public CreateSceneAttribute(string scenePath = null, NewSceneSetup setup = NewSceneSetup.EmptyScene)
		{
			m_Setup = setup;
			SetupAndVerifyScenePath(scenePath);
		}

		IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
		{
			if (Application.isPlaying)
				RuntimeLoadScene();
			else
				EditorLoadScene();

			yield return null;
		}

		IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
		{
			if (Application.isPlaying) {}
			else
				EditorCleanupScene();

			yield return null;
		}

		private void EditorCleanupScene()
		{
			var activeScene = SceneManager.GetActiveScene();
			foreach (var rootGameObject in activeScene.GetRootGameObjects())
			{
				if (m_Setup == NewSceneSetup.DefaultGameObjects)
				{
					if (rootGameObject.name == "Main Camera" || rootGameObject.name == "Directional Light")
						continue;
				}

				rootGameObject.DestroyInAnyMode();
			}

			if (IsScenePathValid())
				DeleteTestScene();
		}

		private void DeleteTestScene()
		{
			Debug.Log($"Deleting test scene from: {m_ScenePath}");
			if (AssetDatabase.DeleteAsset(m_ScenePath) != true)
				throw new UnityException($"AssetDatabase failed to delete test scene in: '{m_ScenePath}'");
		}

		private void EditorLoadScene()
		{
			var activeScene = SceneManager.GetActiveScene();
			var sceneName = CreateTestSceneName();
			if (activeScene.name != sceneName)
			{
				var scene = EditorSceneManager.NewScene(m_Setup);
				scene.name = sceneName;

				if (IsScenePathValid())
					SaveTestScene(scene);
			}
		}

		private void SaveTestScene(Scene scene)
		{
			Debug.Log($"Saving '{scene.name}' to {m_ScenePath} ...");
			CreateScenePathDirectoryIfNotExists();
			if (EditorSceneManager.SaveScene(scene, m_ScenePath) == false)
				throw new UnityException($"EditorSceneManager failed to save test scene to: '{m_ScenePath}'");
		}

		private void RuntimeLoadScene()
		{
			var sceneName = m_Setup == NewSceneSetup.EmptyScene ? TestNames.EmptyTestScene : TestNames.DefaultObjectsTestScene;
			SceneManager.LoadScene(sceneName);
		}

		private void SetupAndVerifyScenePath(string scenePath)
		{
			m_ScenePath = string.IsNullOrWhiteSpace(scenePath) == false ? TestPaths.TempTestAssets + scenePath : null;
			if (m_ScenePath != null)
			{
				PrefixAssetsPathIfNeeded();
				AppendSceneExtensionIfNeeded();
			}
		}

		private void PrefixAssetsPathIfNeeded()
		{
			if (m_ScenePath.StartsWith("Assets") == false)
				m_ScenePath = "Assets/" + m_ScenePath;
		}

		private void AppendSceneExtensionIfNeeded()
		{
			if (m_ScenePath.EndsWith(".unity") == false)
				m_ScenePath += ".unity";
		}

		private void CreateScenePathDirectoryIfNotExists()
		{
			var path = Application.dataPath.Replace("/Assets", "/") + m_ScenePath;
			path = Path.GetDirectoryName(path);
			AssetDatabaseExt.CreateDirectoryIfNotExists(path);
		}

		private bool IsScenePathValid() => string.IsNullOrWhiteSpace(m_ScenePath) == false;

		private string CreateTestSceneName()
		{
			var name = m_ScenePath != null ? Path.GetFileName(m_ScenePath) : m_Setup.ToString();
			return $"Test [CreateScene] {name}";
		}
	}
}
