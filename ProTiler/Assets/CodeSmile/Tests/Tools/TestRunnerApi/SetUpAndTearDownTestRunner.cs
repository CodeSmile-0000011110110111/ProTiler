// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeSmile.Tests.Tools.TestRunnerApi
{
	/// <summary>
	///     Handles TestRunner callbacks mainly to set the running status of the TestRunner to EditorPrefs.
	/// </summary>
	[InitializeOnLoad] [ExcludeFromCodeCoverage]
	public static class SetUpAndTearDownTestRunner
	{
		static SetUpAndTearDownTestRunner() => ScriptableObject
			.CreateInstance<UnityEditor.TestTools.TestRunner.Api.TestRunnerApi>()
			.RegisterCallbacks(new Callbacks());

		[ExcludeFromCodeCoverage]
		private sealed class Callbacks : ICallbacks
		{
			/// <summary>
			///     safety: ensure we have the AssetDatabase up-to-date after tests finished
			/// </summary>
			private static void SynchronizeAssetDatabase() =>
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

			/// <summary>
			///     delete temp test assets dir and its contents
			/// </summary>
			/// <exception cref="UnityException"></exception>
			private static void DeleteTempTestAssetsDirectoryAndContents()
			{
				if (Directory.Exists(TestPaths.TempTestAssets) &&
				    AssetDatabase.DeleteAsset(TestPaths.TempTestAssets) == false)
					throw new UnityException($"failed to delete temp test assets dir: '{TestPaths.TempTestAssets}'");
			}

			/// <summary>
			///     signal to interested parties that we are running a test (ie disable logging)
			/// </summary>
			/// <param name="isRunning"></param>
			private static void SetTestsAreRunningEditorPrefsFlag(Boolean isRunning) =>
				EditorPref.TestRunnerRunning = isRunning;

			/// <summary>
			///     make sure we start with an empty Undo/Redo stack
			/// </summary>
			private static void ClearUndoRedoStack() => Undo.ClearAll();

			private static void CloseActiveScene() =>
				EditorSceneManager.CloseScene(SceneManager.GetActiveScene(), false);

			/// <summary>
			///     TestRunner, if interrupted, sometimes leaves "InitTestScene*.unity" scenes at the Assets root.
			///     This method deletes those remnants just to keep the assets tree "clean".
			/// </summary>
			private static void DeleteTestRunnerLeftOverTempScenes()
			{
				const String searchRoot = "Assets";
				const String sceneAssets = "t:SceneAsset";
				const String tempScenesStartWith = "Assets/InitTestScene";

				var sceneAssetGuids = AssetDatabase.FindAssets(sceneAssets, new[] { searchRoot });
				foreach (var sceneAssetGuid in sceneAssetGuids)
				{
					var scenePath = AssetDatabase.GUIDToAssetPath(sceneAssetGuid);
					if (scenePath.StartsWith(tempScenesStartWith))
					{
						Debug.Log($"Deleting TestRunner left-over scene '{scenePath}'");
						AssetDatabase.DeleteAsset(scenePath);
					}
				}
			}

			public void RunStarted(ITestAdaptor testsToRun)
			{
				if (EditorApplication.isPlaying == false)
					CloseActiveScene();
				DeleteTestRunnerLeftOverTempScenes();
				DeleteTempTestAssetsDirectoryAndContents();
				SynchronizeAssetDatabase();
				ClearUndoRedoStack();
				SetTestsAreRunningEditorPrefsFlag(true);
			}

			public void RunFinished(ITestResultAdaptor result)
			{
				DeleteTestRunnerLeftOverTempScenes();
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
