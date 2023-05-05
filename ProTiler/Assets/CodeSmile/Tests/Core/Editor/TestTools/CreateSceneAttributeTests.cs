// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tests.Tools.Attributes;
using NUnit.Framework;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeSmile.Tests.Editor.TestTools
{
	public class CreateSceneAttributeTests
	{
		private const string TestSceneName = "TestScene";
		private const string TestSceneFullPath = "Assets/" + TestSceneName + ".unity";

		[Test] [CreateEmptyScene]
		public void CreateEmptySceneIsEmpty() => Assert.That(SceneManager.GetActiveScene().rootCount == 0);

		[Test] [CreateDefaultScene]
		public void CreateDefaultSceneContainsDefaultObjects()
		{
			var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

			Assert.That(rootObjects.Length == 2);
			Assert.That(GameObject.Find("Main Camera") != null);
			Assert.That(GameObject.Find("Directional Light") != null);
		}

		[Test] [CreateDefaultScene(TestSceneName)]
		public void CreateDefaultSceneWithoutPathAndExtensionCanBeLoadedAsAsset()
		{
			var loadedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(TestSceneFullPath);

			Assert.That(loadedScene != null);
			Assert.That(loadedScene.name, Is.EqualTo(SceneManager.GetActiveScene().name));
		}

		[Test] [CreateEmptyScene(TestSceneName)]
		public void CreateEmptySceneWithoutPathAndExtensionCanBeLoadedAsAsset()
		{
			var loadedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(TestSceneFullPath);

			Assert.That(loadedScene != null);
			Assert.That(loadedScene.name, Is.EqualTo(SceneManager.GetActiveScene().name));
		}

		[Test] [CreateEmptyScene("Assets/" + TestSceneName)]
		public void CreateSceneWithPathWithoutExtensionCanBeLoadedAsAsset()
		{
			var loadedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(TestSceneFullPath);

			Assert.That(loadedScene != null);
			Assert.That(loadedScene.name, Is.EqualTo(SceneManager.GetActiveScene().name));
		}

		[Test] [CreateEmptyScene(TestSceneFullPath)]
		public void CreateSceneWithPathAndExtensionCanBeLoadedAsAsset()
		{
			var loadedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(TestSceneFullPath);

			Assert.That(loadedScene != null);
			Assert.That(loadedScene.name, Is.EqualTo(SceneManager.GetActiveScene().name));
		}
	}
}
