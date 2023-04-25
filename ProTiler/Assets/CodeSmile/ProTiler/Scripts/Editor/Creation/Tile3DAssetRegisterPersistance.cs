﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Data;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Creation
{
	public class Tile3DAssetRegisterPersistance : AssetModificationProcessor
	{
		private static readonly string TileRegisterAssetFilename = $"{nameof(Tile3DAssetRegister)}.asset";

		private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
		{
			if (WillRenameAsset(sourcePath, destinationPath, TileRegisterAssetFilename))
			{
				LogFailureMessage();
				return AssetMoveResult.FailedMove;
			}

			return AssetMoveResult.DidNotMove;
		}

		private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
		{
			var registerPath = AssetDatabaseExt.FindAssetPaths<Tile3DAssetRegister>().FirstOrDefault();

			if (registerPath != null && AreEqual(registerPath, assetPath) || IsInFolder(registerPath, assetPath))
			{
				LogFailureMessage();
				return AssetDeleteResult.FailedDelete;
			}

			return AssetDeleteResult.DidNotDelete;
		}

		private static bool AreEqual(string registerPath, string assetPath) => registerPath.Equals(assetPath);

		private static bool IsInFolder(string registerPath, string assetPath) =>
			AssetDatabase.IsValidFolder(assetPath) && registerPath.StartsWith(assetPath);

		private static void LogFailureMessage() => Debug.LogError(
			$"{nameof(Tile3DAssetRegister)} cannot be deleted or renamed. " +
			$"It is required for {Names.TileEditor} to function properly. " +
			$"To modify the asset anyway you have to first uninstall {Names.TileEditor}.");

		private static bool WillRenameAsset(string sourcePath, string destinationPath, string assetFilename) =>
			sourcePath.EndsWith(assetFilename) && destinationPath.EndsWith(assetFilename) == false;
	}
}
