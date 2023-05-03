// Copyright (C) 2021-2023 Steffen Itterheim
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
		/// <summary>
		/// Creates a Tile3DAssetRegister asset if one does not exist.
		/// </summary>
		[ExcludeFromCodeCoverage]
		public static void CreateTile3DAssetRegisterIfNotExists()
		{
			if (AssetDatabaseExt.AssetExists<Tile3DAssetRegister>() == false)
			{
				var assetPath = $"{Paths.Tile3DAssetRegister}/{nameof(Tile3DAssetRegister)}.asset";
				AssetDatabaseExt.CreateScriptableObjectAssetAndDirectory<Tile3DAssetRegister>(assetPath);
			}
			else
			{
				// assign singleton by loading the asset otherwise tests may fail
				var register = AssetDatabaseExt.LoadAsset<Tile3DAssetRegister>();
				register.AssignSingletonInstance();
			}
		}

		/// <summary>
		/// Initialization must be delayed because if the Library is deleted, the AssetDatabase won't find the Tile3DAssetRegister
		/// asset even if it exists due to running this from an InitializeOnLoad static ctor.
		/// </summary>
		[ExcludeFromCodeCoverage]
		static Tile3DAssetRegisterCreation() => EditorApplication.delayCall += CreateTile3DAssetRegisterIfNotExists;
	}
}
