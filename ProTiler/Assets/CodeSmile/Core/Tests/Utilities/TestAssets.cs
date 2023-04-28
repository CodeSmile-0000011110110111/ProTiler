// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tests.Utilities;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Tests.Utilities
{
	public static class TestAssets
	{
		public static GameObject LoadTestPrefab()
		{
#if UNITY_EDITOR
			return AssetDatabase.LoadAssetAtPath<GameObject>(TestPaths.TestAssets + "TestPrefab.prefab");
#else
			return null;
#endif
		}
	}
}
