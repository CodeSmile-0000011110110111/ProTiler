// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;

namespace CodeSmile.Editor
{
	public static class EditorPref
	{
		/// <summary>
		/// Is true while TestRunner is running tests. Can be used to skip things like user-centric
		/// error logging while running tests. Use wisely!
		/// </summary>
		public static bool TestRunnerRunning
		{
			get => EditorPrefs.GetBool(Key.TestRunnerRunning, false);
			set => EditorPrefs.SetBool(Key.TestRunnerRunning, value);
		}

		private static class Key
		{
			private const string KeyRoot = "CodeSmile.";
			private const string KeyTest = "TestRunner.";

			public const string TestRunnerRunning = KeyRoot + KeyTest + "Running";
		}
	}
}
