// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Extensions
{
	public static class SelectionExt
	{
		[ExcludeFromCodeCoverage]
		public static GameObject GetSelectedPrefab()
		{
#if UNITY_EDITOR
			return IsPrefabSelected() ? Selection.activeObject as GameObject : null;
#else
			return null;
#endif
		}

		[ExcludeFromCodeCoverage]
		public static bool IsPrefabSelected()
		{
#if UNITY_EDITOR
			return Selection.activeObject is GameObject go && go.IsPrefab();
#else
			return false;
#endif
		}

		[ExcludeFromCodeCoverage]
		public static int PrefabCount()
		{
			var prefabCount = 0;
#if UNITY_EDITOR
			foreach (var gameObject in Selection.gameObjects)
			{
				if (gameObject.IsPrefab())
					prefabCount++;
			}
#endif
			return prefabCount;
		}
	}
}
