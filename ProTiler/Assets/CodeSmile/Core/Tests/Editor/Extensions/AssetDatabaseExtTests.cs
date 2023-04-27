// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.Tests.Utilities;
using NUnit.Framework;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Tests.Editor
{
	public class AssetDatabaseExtTests
	{
		private static void DeleteDirectoryIfExists(string path)
		{
			if (Directory.Exists(path))
			{
				Debug.LogWarning($"delete dir before test: {path}");
				Directory.Delete(path);
			}
		}

		private static void DeleteTestAsset(string path)
		{
			Assert.That(AssetDatabase.DeleteAsset(path));
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
		}

		[TestCase(TestPaths.TempTestAssets + nameof(AssetDatabaseExtTestSO))]
		[TestCase(TestPaths.TempTestAssets + nameof(AssetDatabaseExtTestSO) + "/")]
#if UNITY_EDITOR_WIN
		[TestCase(TestPaths.TempTestAssets + nameof(AssetDatabaseExtTestSO) + @"\")]
#endif
		public void CreateDirectoryIfNotExists(string path)
		{
			DeleteDirectoryIfExists(path);

			AssetDatabaseExt.CreateDirectoryIfNotExists(path);

			Assert.That(Directory.Exists(path));
			Assert.That(AssetDatabase.IsValidFolder(path));

			DeleteTestAsset(path);
		}

#if UNITY_EDITOR_WIN
		[TestCase(TestPaths.TempTestAssets + "*?<>/\\")]
		public void CreateDirectoryWithInvalidFilePathThrows(string path) =>
			Assert.Throws<ArgumentException>(() => AssetDatabaseExt.CreateDirectoryIfNotExists(path));
#endif

		[TestCase(TestPaths.TempTestAssets + "CreateTest/" + nameof(AssetDatabaseExtTestSO) + ".asset")]
		public void CreateAssetAndDirectory(string path)
		{
			DeleteDirectoryIfExists(Path.GetDirectoryName(path));

			var asset = AssetDatabaseExt.CreateScriptableObjectAssetAndDirectory<AssetDatabaseExtTestSO>(path);

			Assert.That(asset != null);
			Assert.That(AssetDatabaseExt.AssetExists<AssetDatabaseExtTestSO>());

			DeleteTestAsset(path);
		}

		[Test] public void AssetDoesNotExist() => Assert.That(AssetDatabaseExt.AssetExists<AssetDatabaseExtTestSO>() == false);

		[Test] public void FindAssetGuidByType()
		{
			AssetDatabaseExt.CreateScriptableObjectAssetAndDirectory<AssetDatabaseExtTestSO>(AssetDatabaseExtTestSO.TestPath);

			var found = AssetDatabaseExt.FindAssets<AssetDatabaseExtTestSO>();

			Assert.That(found.Length == 1);
			Assert.That(found[0] == AssetDatabase.AssetPathToGUID(AssetDatabaseExtTestSO.TestPath));

			DeleteTestAsset(AssetDatabaseExtTestSO.TestPath);
		}

		[Test] public void FindAssetPathByType()
		{
			AssetDatabaseExt.CreateScriptableObjectAssetAndDirectory<AssetDatabaseExtTestSO>(AssetDatabaseExtTestSO.TestPath);

			var paths = AssetDatabaseExt.FindAssetPaths<AssetDatabaseExtTestSO>();

			Assert.That(paths.Length == 1);
			Assert.That(paths[0] == AssetDatabaseExtTestSO.TestPath);

			DeleteTestAsset(AssetDatabaseExtTestSO.TestPath);
		}

		[Test] public void LoadAssetsByType()
		{
			AssetDatabaseExt.CreateScriptableObjectAssetAndDirectory<AssetDatabaseExtTestSO>(AssetDatabaseExtTestSO.TestPath);

			var instances = AssetDatabaseExt.LoadAssets<AssetDatabaseExtTestSO>();

			Assert.That(instances.Length == 1);
			Assert.That(instances[0] != null);
			Assert.That(instances[0].GetType() == typeof(AssetDatabaseExtTestSO));

			DeleteTestAsset(AssetDatabaseExtTestSO.TestPath);
		}

		[Test] public void LoadAssetByType()
		{
			AssetDatabaseExt.CreateScriptableObjectAssetAndDirectory<AssetDatabaseExtTestSO>(AssetDatabaseExtTestSO.TestPath);

			var instance = AssetDatabaseExt.LoadAsset<AssetDatabaseExtTestSO>();

			Assert.That(instance != null);
			Assert.That(instance.GetType() == typeof(AssetDatabaseExtTestSO));

			DeleteTestAsset(AssetDatabaseExtTestSO.TestPath);
		}
	}
}
