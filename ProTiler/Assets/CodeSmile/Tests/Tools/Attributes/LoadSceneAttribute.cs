// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Tools.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public class LoadSceneAttribute : NUnitAttribute, IOuterUnityTestAction
	{
		private string m_ScenePath;

		public LoadSceneAttribute(string sceneName) => SetScenePath(sceneName);

		IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
		{
			var loadSceneParams = new LoadSceneParameters(LoadSceneMode.Single);
			if (EditorApplication.isPlaying == false)
			{
				EditorSceneManager.OpenScene(m_ScenePath);
				yield return null;
			}
			else
				yield return EditorSceneManager.LoadSceneAsyncInPlayMode(m_ScenePath, loadSceneParams);
		}

		IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
		{
			yield return null;
		}

		private void SetScenePath(string sceneName)
		{
			const string sceneExtension = ".unity";
			m_ScenePath = Path.HasExtension(sceneName) == false
				? $"{sceneName}{sceneExtension}"
				: Path.ChangeExtension(sceneName, sceneExtension);
		}
	}
}
