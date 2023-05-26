// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.Extensions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Tools.Attributes
{
	[FullCovered]
	public abstract class CreateSceneAttribute : NUnitAttribute, IOuterUnityTestAction
	{
		private readonly NewSceneSetup m_Setup;
		private String m_ScenePath;

		private static Boolean IsObjectNamedLikeADefaultObject(GameObject rootGameObject) =>
			rootGameObject.name == "Main Camera" || rootGameObject.name == "Directional Light";

		protected CreateSceneAttribute(String scenePath = null, NewSceneSetup setup = NewSceneSetup.EmptyScene)
		{
			m_Setup = setup;
			SetupAndVerifyScenePath(scenePath);
		}

		[ExcludeFromCodeCoverage] IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
		{
			yield return OnBeforeTest();
		}

		[ExcludeFromCodeCoverage] IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
		{
			yield return OnAfterTest();
		}

		private IEnumerator OnBeforeTest()
		{
			if (Application.isPlaying)
				RuntimeLoadScene();
			else
				EditorCreateNewScene();

			return null;
		}

		private IEnumerator OnAfterTest()
		{
			if (Application.isPlaying)
				RuntimeLoadScene();
			else
				EditorCleanupScene();

			return null;
		}

		private void EditorCleanupScene()
		{
			var activeScene = SceneManager.GetActiveScene();
			foreach (var rootGameObject in activeScene.GetRootGameObjects())
			{
				if (ShouldSkipDefaultObjects(rootGameObject))
					continue;

				rootGameObject.DestroyInAnyMode();
			}

			if (IsScenePathValid())
				DeleteTestScene();
		}

		private Boolean ShouldSkipDefaultObjects(GameObject rootGameObject) =>
			m_Setup == NewSceneSetup.DefaultGameObjects &&
			IsObjectNamedLikeADefaultObject(rootGameObject);

		[ExcludeFromCodeCoverage]
		private void DeleteTestScene()
		{
			Debug.Log($"Deleting test scene from: {m_ScenePath}");
			if (AssetDatabase.DeleteAsset(m_ScenePath) != true)
				throw new UnityException($"AssetDatabase failed to delete test scene in: '{m_ScenePath}'");
		}

		private void EditorCreateNewScene()
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

		[ExcludeFromCodeCoverage]
		private void SaveTestScene(Scene scene)
		{
			Debug.Log($"Saving '{scene.name}' to {m_ScenePath} ...");
			if (EditorSceneManager.SaveScene(scene, m_ScenePath) == false)
				throw new UnityException($"EditorSceneManager failed to save test scene to: '{m_ScenePath}'");
		}

		private void RuntimeLoadScene()
		{
			var sceneName = m_Setup == NewSceneSetup.EmptyScene
				? TestNames.EmptyTestScene
				: TestNames.DefaultObjectsTestScene;
			SceneManager.LoadScene(sceneName);
		}

		private void SetupAndVerifyScenePath(String scenePath)
		{
			m_ScenePath = String.IsNullOrWhiteSpace(scenePath) == false ? scenePath : null;
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

		private Boolean IsScenePathValid() => String.IsNullOrWhiteSpace(m_ScenePath) == false;

		private String CreateTestSceneName()
		{
			var name = m_ScenePath != null ? Path.GetFileName(m_ScenePath) : m_Setup.ToString();
			return $"Test [{GetType().Name.Replace("Attribute", "")}] {name}";
		}
	}
}
