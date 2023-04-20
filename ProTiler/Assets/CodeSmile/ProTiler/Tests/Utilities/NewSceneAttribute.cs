// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CodeSmile.ProTiler.Tests.Utilities
{
	public class NewSceneAttribute : NUnitAttribute, IOuterUnityTestAction
	{
		private Scene m_Scene;
		private NewSceneSetup m_Setup;

		public NewSceneAttribute(NewSceneSetup setup = NewSceneSetup.DefaultGameObjects) {}

		IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
		{
			m_Scene = EditorSceneManager.NewScene(m_Setup, NewSceneMode.Single);
			yield return null;
		}

		IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
		{
			yield return null;
		}
	}
}
