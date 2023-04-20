// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CodeSmile.ProTiler.Tests.Utilities
{
	public class LoadSceneAttribute : NUnitAttribute, IOuterUnityTestAction
	{
		private readonly string m_SceneName;

		public LoadSceneAttribute(string sceneName) => m_SceneName = Path.ChangeExtension(sceneName, ".unity");

		IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
		{
			var loadSceneParams = new LoadSceneParameters(LoadSceneMode.Single);
			if (EditorApplication.isPlaying == false)
			{
				EditorSceneManager.OpenScene(m_SceneName);
				yield return null;
			}
			else
				yield return EditorSceneManager.LoadSceneAsyncInPlayMode(m_SceneName, loadSceneParams);
		}

		IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
		{
			yield return null;
		}
	}
}
