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
		public static GameObject GetSelectedPrefab() => IsPrefabSelected() ? Selection.activeObject as GameObject : null;

		[ExcludeFromCodeCoverage]
		public static bool IsPrefabSelected() => Selection.activeObject is GameObject go && go.IsPrefab();

		[ExcludeFromCodeCoverage]
		public static int PrefabCount()
		{
			var prefabCount = 0;
			foreach (var gameObject in Selection.gameObjects)
			{
				if (gameObject.IsPrefab())
					prefabCount++;
			}

			return prefabCount;
		}
	}
}
