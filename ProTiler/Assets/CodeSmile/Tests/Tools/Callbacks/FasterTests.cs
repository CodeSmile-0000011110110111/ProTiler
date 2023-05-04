// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Reflection;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace CodeSmile.Tests.Tools.Callbacks
{
	[InitializeOnLoad]
	public class FasterTests
	{
		static FasterTests() => ScriptableObject.CreateInstance<TestRunnerApi>().RegisterCallbacks(new Callbacks());

		private class Callbacks : ICallbacks
		{
			private const string ApplicationIdleTimeKey = "ApplicationIdleTime";
			private const string InteractionModeKey = "InteractionMode";

			private int m_UserApplicationIdleTime;
			private int m_UserInteractionMode;

			private static void UpdateInteractionModeSettings()
			{
				const string UpdateInteractionModeMethodName = "UpdateInteractionModeSettings";

				var bindingFlags = BindingFlags.Static | BindingFlags.NonPublic;
				var type = typeof(EditorApplication);
				var method = type.GetMethod(UpdateInteractionModeMethodName, bindingFlags);
				method.Invoke(null, null);
			}

			public void RunStarted(ITestAdaptor testsToRun) => SpeedUpTestRunner();
			public void RunFinished(ITestResultAdaptor result) => ResetInteractionMode();
			public void TestStarted(ITestAdaptor test) {}
			public void TestFinished(ITestResultAdaptor result) {}

			private void SpeedUpTestRunner()
			{
				Debug.Log("Set Interaction Mode to 'No Throttling' during tests.");
				SetInteractionModeToNoThrottling();
				UpdateInteractionModeSettings();
			}

			private void ResetInteractionMode()
			{
				Debug.Log("Reset Interaction Mode to user settings.");
				SetInteractionModeToUserSetting();
				UpdateInteractionModeSettings();
			}

			private void SetInteractionModeToNoThrottling()
			{
				m_UserApplicationIdleTime = EditorPrefs.GetInt(ApplicationIdleTimeKey);
				m_UserInteractionMode = EditorPrefs.GetInt(InteractionModeKey);
				EditorPrefs.SetInt(ApplicationIdleTimeKey, 0);
				EditorPrefs.SetInt(InteractionModeKey, 1);
			}

			private void SetInteractionModeToUserSetting()
			{
				EditorPrefs.SetInt(ApplicationIdleTimeKey, m_UserApplicationIdleTime);
				EditorPrefs.SetInt(InteractionModeKey, m_UserInteractionMode);
			}
		}
	}
}
