// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace CodeSmile.ProTiler.Tests.Utilities
{
	[AttributeUsage(AttributeTargets.Method)]
	public class NewEmptySceneAttribute : NewSceneAttribute
	{
		public NewEmptySceneAttribute(string scenePath = null)
			: base(scenePath, NewSceneSetup.EmptyScene) {}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class NewSceneAttribute : NUnitAttribute, IOuterUnityTestAction
	{
		private readonly NewSceneSetup m_Setup;
		private readonly string m_ScenePath;

		public NewSceneAttribute(string scenePath = null, NewSceneSetup setup = NewSceneSetup.DefaultGameObjects)
		{
			m_ScenePath = string.IsNullOrWhiteSpace(scenePath) == false ? scenePath.Trim() : null;
			m_Setup = setup;

			if (m_ScenePath != null)
				CreateDirectoryIfNotExists();
		}

		IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
		{
			var scene = EditorSceneManager.NewScene(m_Setup, NewSceneMode.Single);
			if (m_ScenePath != null)
				EditorSceneManager.SaveScene(scene, Defines.TestAssetsPath + m_ScenePath);

			yield return null;
		}

		IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
		{
			if (m_ScenePath != null && File.Exists(m_ScenePath))
				AssetDatabase.DeleteAsset(m_ScenePath);
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
