// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Editor.Data;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Creation
{
	[InitializeOnLoad]
	public sealed class Tile3DAssetRegisterCreation : ScriptableObject
	{
		public static void CreateTile3DAssetRegisterIfNotExists()
		{
			if (AssetDatabaseExt.AssetExists<Tile3DAssetRegister>() == false)
			{
				var assetPath = $"{Paths.Tile3DAssetRegister}/{nameof(Tile3DAssetRegister)}.asset";
				var register = AssetDatabaseExt.CreateAssetAndDirectory<Tile3DAssetRegister>(assetPath);
				register.OnCreated();
			}
		}

		static Tile3DAssetRegisterCreation() => CreateTile3DAssetRegisterIfNotExists();
	}
}
