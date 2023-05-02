// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace CodeSmile.Tests.Utilities
{
	/// <summary>
	///     Handles TestRunner callbacks mainly to set the running status of the TestRunner to EditorPrefs.
	/// </summary>
	[InitializeOnLoad]
	public class TestAssetCleaner
	{
		static TestAssetCleaner() => ScriptableObject.CreateInstance<TestRunnerApi>().RegisterCallbacks(new Callbacks());

		private class Callbacks : ICallbacks
		{
			private static void SynchronizeAssetDatabase() => AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

			private static void DeleteTempTestAssetsDirectory()
			{
				if (Directory.Exists(TestPaths.TempTestAssets) && AssetDatabase.DeleteAsset(TestPaths.TempTestAssets) == false)
					throw new UnityException($"failed to delete temp test assets dir: '{TestPaths.TempTestAssets}'");
			}

			public void RunStarted(ITestAdaptor testsToRun)
			{
				// safety: ensure we have the AssetDatabase up-to-date before testing
				DeleteTempTestAssetsDirectory();
				SynchronizeAssetDatabase();
				EditorPref.TestRunnerRunning = true;
			}

			public void RunFinished(ITestResultAdaptor result)
			{
				// safety: ensure we have the AssetDatabase up-to-date after tests finished
				DeleteTempTestAssetsDirectory();
				SynchronizeAssetDatabase();
				EditorPref.TestRunnerRunning = false;
			}

			public void TestStarted(ITestAdaptor test) {}
			public void TestFinished(ITestResultAdaptor result) {}
		}
	}
}
