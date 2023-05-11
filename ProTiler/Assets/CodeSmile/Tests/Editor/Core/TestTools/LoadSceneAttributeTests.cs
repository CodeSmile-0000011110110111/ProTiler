// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tests.Tools;
using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using UnityEngine.SceneManagement;

namespace CodeSmile.Tests.Editor.Core.TestTools
{
	public class LoadSceneAttributeTests
	{
		[Test] [LoadScene(TestPaths.EmptyTestScene)]
		public void LoadEmptyTestSceneIsEmpty() => Assert.That(SceneManager.GetActiveScene().GetRootGameObjects().Length == 0);

		[Test] [LoadScene(TestPaths.DefaultObjectsTestScene)]
		public void LoadDefaultObjectsTestSceneIsNotEmpty() =>
			Assert.That(SceneManager.GetActiveScene().GetRootGameObjects().Length != 0);
	}
}
