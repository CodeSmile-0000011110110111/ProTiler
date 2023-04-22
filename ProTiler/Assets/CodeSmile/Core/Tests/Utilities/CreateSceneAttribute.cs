// Copyright (C) 2021-2023 Steffen Itterheim
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

namespace CodeSmile.ProTiler.Tests.Utilities
{
	[AttributeUsage(AttributeTargets.Method)]
	public class EmptySceneAttribute : CreateSceneAttribute
	{
		public EmptySceneAttribute(string scenePath = null)
			: base(scenePath, NewSceneSetup.EmptyScene) {}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class DefaultSceneAttribute : CreateSceneAttribute
	{
		public DefaultSceneAttribute(string scenePath = null)
			: base(scenePath, NewSceneSetup.DefaultGameObjects) {}
	}


	public class CreateSceneAttribute : NUnitAttribute, IOuterUnityTestAction
	{
		private readonly NewSceneSetup m_Setup;
		private readonly string m_ScenePath;

		public CreateSceneAttribute(string scenePath = null, NewSceneSetup setup = NewSceneSetup.EmptyScene)
		{
			m_ScenePath = string.IsNullOrWhiteSpace(scenePath) == false ? Defines.TestAssetsPath + scenePath.Trim() : null;
			m_Setup = setup;

			if (m_ScenePath != null)
				CreateDirectoryIfNotExists();
		}

		IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
		{
			// only create new scene if the existing scene isn't already an empty scene
			var scene = SceneManager.GetActiveScene();
			var rootObjects = scene.GetRootGameObjects();
			if (rootObjects != null && rootObjects.Length != 0)
			{
				scene = EditorSceneManager.NewScene(m_Setup);
			}

			if (m_ScenePath != null)
				EditorSceneManager.SaveScene(scene, m_ScenePath);

			yield return null;
		}

		IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
		{
			var scene = SceneManager.GetActiveScene();
			var rootObjects = scene.GetRootGameObjects();
			foreach (var rootObject in rootObjects)
				rootObject.DestroyInAnyMode();

			if (m_ScenePath != null)
			{
				if (AssetDatabase.DeleteAsset(m_ScenePath) != true)
					Debug.LogWarning($"failed to delete NewScene named '{m_ScenePath}'");
			}

			yield return null;
		}

		private void CreateDirectoryIfNotExists()
		{
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
