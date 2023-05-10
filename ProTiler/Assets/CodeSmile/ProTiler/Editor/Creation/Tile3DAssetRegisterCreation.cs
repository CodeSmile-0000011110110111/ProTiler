﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Data;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Creation
{
	[InitializeOnLoad]
	public sealed class Tile3DAssetRegisterCreation : ScriptableObject
	{
		private const string RegisterAssetFilePath = Paths.Tile3DAssetRegister + "/" + nameof(Tile3DAssetRegister) + ".asset";

		/// <summary>
		///     Initialization must be delayed because if the Library is deleted, the AssetDatabase won't find the
		///     Tile3DAssetRegister asset even if it exists due to running this from an InitializeOnLoad static ctor.
		/// </summary>
		[ExcludeFromCodeCoverage]
		static Tile3DAssetRegisterCreation() => EditorApplication.delayCall += InitializeTileAssetRegister;

		/// <summary>
		///     Creates a Tile3DAssetRegister asset if one does not exist.
		/// </summary>
		[ExcludeFromCodeCoverage]
		public static void InitializeTileAssetRegister()
		{
			var register = LoadOrCreateTileAssetRegister();
			register.AssignSingletonInstance();
			register.LoadMissingTileAssetAndSetAsDefault();
		}

		[ExcludeFromCodeCoverage]
		private static Tile3DAssetRegister LoadOrCreateTileAssetRegister() => AssetDatabaseExt.AssetExists<Tile3DAssetRegister>()
			? AssetDatabaseExt.LoadAsset<Tile3DAssetRegister>()
			: AssetDatabaseExt.CreateScriptableObjectAssetAndDirectory<Tile3DAssetRegister>(RegisterAssetFilePath);
	}
}