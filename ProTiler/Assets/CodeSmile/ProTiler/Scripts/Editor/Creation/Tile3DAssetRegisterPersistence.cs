// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using CodeSmile.Editor.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Creation
{
	public class Tile3DAssetRegisterPersistence : AssetModificationProcessor
	{
		private static readonly string TileRegisterAssetFilename = $"{nameof(Tile3DAssetRegister)}.asset";

		private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
		{
			if (IsAssetGoingToBeRenamed(sourcePath, destinationPath, TileRegisterAssetFilename))
			{
				LogFailureMessage();
				return AssetMoveResult.FailedMove;
			}

			return AssetMoveResult.DidNotMove;
		}

		private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
		{
			var registerPath = AssetDatabaseExt.FindAssetPaths<Tile3DAssetRegister>().FirstOrDefault();

			if (registerPath != null && AreEqual(registerPath, assetPath) || IsInDirectoryTree(registerPath, assetPath))
			{
				LogFailureMessage();
				return AssetDeleteResult.FailedDelete;
			}

			return AssetDeleteResult.DidNotDelete;
		}

		private static bool AreEqual(string registerPath, string assetPath) => registerPath.Equals(assetPath);

		private static bool IsInDirectoryTree(string registerPath, string assetPath) =>
			AssetDatabase.IsValidFolder(assetPath) && registerPath.StartsWith(assetPath);

		private static bool IsAssetGoingToBeRenamed(string sourcePath, string destinationPath, string assetFilename) =>
			sourcePath.EndsWith(assetFilename) && destinationPath.EndsWith(assetFilename) == false;

		[ExcludeFromCodeCoverage] private static void LogFailureMessage()
		{
			// don't log during tests
			if (EditorPref.TestRunnerRunning)
				return;

			// must use error log, warning and info logs are suppressed during AssetModProc callbacks for some reason
			Debug.LogError($"{nameof(Tile3DAssetRegister)} cannot be deleted or renamed. " +
			               $"It is required for {Names.TileEditor} to function properly. " +
			               $"To modify the asset anyway you have to first uninstall {Names.TileEditor}.");
		}

		[ExcludeFromCodeCoverage] public Tile3DAssetRegisterPersistence() {}
	}
}
