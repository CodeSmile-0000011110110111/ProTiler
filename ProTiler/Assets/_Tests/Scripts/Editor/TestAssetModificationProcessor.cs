// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace _Tests.Scripts.Editor
{
	public class TestAssetModificationProcessor : AssetModificationProcessor
	{
		private static void OnWillCreateAsset(string assetName) => Debug.Log($"Will Create: '{assetName}'");

		private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
		{
			Debug.Log($"Will Delete: '{assetPath}' ({options})");
			return AssetDeleteResult.DidNotDelete;
		}

		private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
		{
			Debug.Log($"Will Move: '{sourcePath}' to '{destinationPath}'");
			return AssetMoveResult.DidNotMove;
		}

		private static string[] OnWillSaveAssets(string[] paths)
		{
			Debug.Log($"Will Save: {paths.Length} assets ...");
			foreach (var path in paths)
				Debug.Log($"   {path}");

			return paths;
		}
	}
}
