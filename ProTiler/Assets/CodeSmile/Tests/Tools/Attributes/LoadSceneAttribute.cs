// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CodeSmile.Tests.Tools.Attributes
{
	[FullCovered]
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class LoadSceneAttribute : NUnitAttribute, IOuterUnityTestAction
	{
		private string m_ScenePath;

		public LoadSceneAttribute(string sceneName) => SetScenePath(sceneName);

		[ExcludeFromCodeCoverage] IEnumerator IOuterUnityTestAction.BeforeTest(ITest test) { yield return OnBeforeTest(); }
		[ExcludeFromCodeCoverage] IEnumerator IOuterUnityTestAction.AfterTest(ITest test) { yield return null; }

		[ExcludeFromCodeCoverage]
		private object OnBeforeTest()
		{
			if (EditorApplication.isPlaying)
				return EditorSceneManager.LoadSceneAsyncInPlayMode(m_ScenePath, new LoadSceneParameters(LoadSceneMode.Single));

			EditorSceneManager.OpenScene(m_ScenePath);
			return null;
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
