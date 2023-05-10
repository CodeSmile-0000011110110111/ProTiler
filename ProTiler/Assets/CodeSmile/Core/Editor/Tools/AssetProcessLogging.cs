// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

namespace CodeSmile.Editor.Tools
{
#if ENABLE_ASSET_PROCESS_LOGGING
	public class AssetProcessLogging : UnityEditor.AssetPostprocessor
	{
		private static void OnPostprocessAllAssets(string[] importedAssetPaths, string[] deletedAssetPaths,
			string[] movedAssetPaths, string[] fromAssetPaths, bool didDomainReload)
		{
			if (didDomainReload)
			{
				UnityEngine.Debug.Log($"Did Reload Domain ... Imported: {importedAssetPaths.Length}, " +
				          $"Deleted: {deletedAssetPaths.Length}, Moved: {movedAssetPaths.Length}");
			}

			foreach (var str in importedAssetPaths)
				UnityEngine.Debug.Log($"Did Import: '{str}'");
			foreach (var str in deletedAssetPaths)
				UnityEngine.Debug.Log($"Did Delete: '{str}'");
			for (var i = 0; i < movedAssetPaths.Length; i++)
				UnityEngine.Debug.Log($"Did Move: '{fromAssetPaths[i]}' to '{movedAssetPaths[i]}'");
		}

		private void OnPreprocessAsset() => UnityEngine.Debug.Log($"Will Process: '{assetPath}' ({assetImporter})");
	}

	public class AssetModificationLogging : UnityEditor.AssetModificationProcessor
	{
		private static void OnWillCreateAsset(string assetName) => UnityEngine.Debug.Log($"Will Create: '{assetName}'");

		private static UnityEditor.AssetDeleteResult OnWillDeleteAsset(string assetPath, UnityEditor.RemoveAssetOptions options)
		{
			UnityEngine.Debug.Log($"Will Delete: '{assetPath}' ({options})");
			return UnityEditor.AssetDeleteResult.DidNotDelete;
		}

		private static UnityEditor.AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
		{
			UnityEngine.Debug.Log($"Will Move: '{sourcePath}' to '{destinationPath}'");
			return UnityEditor.AssetMoveResult.DidNotMove;
		}

		private static string[] OnWillSaveAssets(string[] paths)
		{
			UnityEngine.Debug.Log($"Will Save: {paths.Length} assets ...");
			foreach (var path in paths)
				UnityEngine.Debug.Log($"   {path}");

			return paths;
		}
	}
#endif
}
