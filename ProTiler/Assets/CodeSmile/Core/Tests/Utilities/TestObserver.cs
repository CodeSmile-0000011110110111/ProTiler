// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace CodeSmile.Tests.Utilities
{
	/// <summary>
	/// Handles TestRunner callbacks mainly to set the running status of the TestRunner to EditorPrefs.
	/// </summary>
	[InitializeOnLoad]
	public class TestObserver
	{
		static TestObserver() => ScriptableObject.CreateInstance<TestRunnerApi>().RegisterCallbacks(new TestCallbacks());

		private class TestCallbacks : ICallbacks
		{
			public void RunStarted(ITestAdaptor testsToRun) => EditorPref.TestRunnerRunning = true;
			public void RunFinished(ITestResultAdaptor result) => EditorPref.TestRunnerRunning = false;
			public void TestStarted(ITestAdaptor test) {}
			public void TestFinished(ITestResultAdaptor result) {}
		}
	}
}
