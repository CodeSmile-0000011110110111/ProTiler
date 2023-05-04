// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace CodeSmile.Tests.Tools
{
	/// <summary>
	///     Handles TestRunner callbacks mainly to set the running status of the TestRunner to EditorPrefs.
	/// </summary>
	[InitializeOnLoad]
	public class BeforeAndAfterTestsCleaner
	{
		static BeforeAndAfterTestsCleaner() => ScriptableObject.CreateInstance<TestRunnerApi>().RegisterCallbacks(new Callbacks());

		private class Callbacks : ICallbacks
		{
			/// <summary>
			///     safety: ensure we have the AssetDatabase up-to-date after tests finished
			/// </summary>
			private static void SynchronizeAssetDatabase() => AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

			/// <summary>
			///     delete temp test assets dir and its contents
			/// </summary>
			/// <exception cref="UnityException"></exception>
			private static void DeleteTempTestAssetsDirectoryAndContents()
			{
				if (Directory.Exists(TestPaths.TempTestAssets) && AssetDatabase.DeleteAsset(TestPaths.TempTestAssets) == false)
					throw new Exception($"failed to delete temp test assets dir: '{TestPaths.TempTestAssets}'");
			}

			/// <summary>
			///     signal to interested parties that we are running a test (ie disable logging)
			/// </summary>
			/// <param name="isRunning"></param>
			private static void SetTestsAreRunningEditorPrefsFlag(bool isRunning) => EditorPref.TestRunnerRunning = isRunning;

			/// <summary>
			///     make sure we start with an empty Undo/Redo stack
			/// </summary>
			private static void ClearUndoRedoStack() => Undo.ClearAll();

			public void RunStarted(ITestAdaptor testsToRun)
			{
				// safety: ensure we have the AssetDatabase up-to-date before testing
				DeleteTempTestAssetsDirectoryAndContents();
				SynchronizeAssetDatabase();
				ClearUndoRedoStack();
				SetTestsAreRunningEditorPrefsFlag(true);
			}

			public void RunFinished(ITestResultAdaptor result)
			{
				DeleteTempTestAssetsDirectoryAndContents();
				SynchronizeAssetDatabase();
				ClearUndoRedoStack();
				SetTestsAreRunningEditorPrefsFlag(false);
			}

			public void TestStarted(ITestAdaptor test) {}
			public void TestFinished(ITestResultAdaptor result) {}
		}
	}
}
