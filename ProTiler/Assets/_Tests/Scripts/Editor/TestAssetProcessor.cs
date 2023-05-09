// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace _Tests.Scripts.Editor
{
	public class TestAssetProcessor : AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(string[] importedAssetPaths, string[] deletedAssetPaths,
			string[] movedAssetPaths, string[] fromAssetPaths, bool didDomainReload)
		{
			if (didDomainReload)
				Debug.Log("Did Reload Domain ...");

			foreach (var str in importedAssetPaths)
				Debug.Log($"Did Import: '{str}'");
			foreach (var str in deletedAssetPaths)
				Debug.Log($"Did Delete: '{str}'");
			for (var i = 0; i < movedAssetPaths.Length; i++)
				Debug.Log($"Did Move: '{fromAssetPaths[i]}' to '{movedAssetPaths[i]}'");
		}

		private void OnPreprocessAsset() => Debug.Log($"Will Process: '{assetPath}' ({assetImporter})");
	}
}
