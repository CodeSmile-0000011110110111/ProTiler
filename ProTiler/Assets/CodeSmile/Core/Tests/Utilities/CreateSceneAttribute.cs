﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Utilities
{
	/// <summary>
	///     Creates a new empty scene for a unit test method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class EmptySceneAttribute : CreateSceneAttribute
	{
		/// <summary>
		///     Creates a new empty scene for a unit test method.
		/// </summary>
		/// <param name="scenePath">
		///     if non-empty, the scene will be saved as an asset for tests that verify correctness of
		///     save/load of a scene's contents. The saved scene asset is deleted after the test ran.
		/// </param>
		public EmptySceneAttribute(string scenePath = null)
			: base(scenePath) {}
	}

	/// <summary>
	///     Creates a new default scene (with Camera + Direct Light) for a unit test method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class DefaultSceneAttribute : CreateSceneAttribute
	{
		/// <summary>
		///     Creates a new default scene (with Camera + Direct Light) for a unit test method.
		///     Caution: the scene contents will be deleted with the exception of the default game objects named
		///     "Main Camera" and "Directional Light". Any changes to these two objects will persist between tests.
		///     If you rename these objects they will be deleted and not restored for other tests.
		/// </summary>
		/// <param name="scenePath">
		///     if non-empty, the scene will be saved as an asset for tests that verify correctness of save/load of a scene's
		///     contents. The saved scene asset is deleted after the test ran.
		/// </param>
		public DefaultSceneAttribute(string scenePath = null)
			: base(scenePath, NewSceneSetup.DefaultGameObjects) {}
	}

	public class CreateSceneAttribute : NUnitAttribute, IOuterUnityTestAction
	{
		private readonly NewSceneSetup m_Setup;
		private readonly string m_ScenePath;

		public CreateSceneAttribute(string scenePath = null, NewSceneSetup setup = NewSceneSetup.EmptyScene)
		{
			m_ScenePath = string.IsNullOrWhiteSpace(scenePath) == false ? TestPaths.TestAssets + scenePath : null;
			m_Setup = setup;

			CreateScenePathDirectoryIfNeeded();
		}

		IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
		{
			var activeScene = SceneManager.GetActiveScene();
			var sceneName = GetSceneName();
			if (activeScene.name != sceneName)
			{
				var scene = EditorSceneManager.NewScene(m_Setup);
				scene.name = sceneName;

				if (m_ScenePath != null)
					EditorSceneManager.SaveScene(scene, m_ScenePath);
			}

			yield return null;
		}

		IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
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

			if (m_ScenePath != null)
			{
				if (AssetDatabase.DeleteAsset(m_ScenePath) != true)
					Debug.LogWarning($"failed to delete NewScene named '{m_ScenePath}'");
			}

			yield return null;
		}

		private string GetSceneName()
		{
			var name = m_ScenePath != null ? Path.GetFileName(m_ScenePath) : m_Setup.ToString();
			return $"Test [CreateScene] {name}";
		}

		private void CreateScenePathDirectoryIfNeeded()
		{
			if (m_ScenePath == null)
				return;

			var path = Application.dataPath.Replace("/Assets", "") + m_ScenePath;
			path = Path.GetFullPath(path);
			if (Directory.Exists(path) == false)
			{
				Directory.CreateDirectory(path);

				// Refresh is needed because "Assets" contents were modified WITHOUT using AssetDatabase methods
				// See: https://forum.unity.com/threads/calling-assetdatabase-refresh-mandatory-reading-or-face-the-consequences.1330947/
				AssetDatabase.Refresh();
			}
		}
	}
}
