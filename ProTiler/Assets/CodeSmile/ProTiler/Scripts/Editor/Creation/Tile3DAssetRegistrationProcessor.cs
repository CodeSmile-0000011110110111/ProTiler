// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Assets;
using System;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Creation
{
	public class Tile3DAssetRegistrationProcessor : AssetModificationProcessor
	{

		/*private static void OnWillCreateAsset(string assetPath)
		{
			Debug.Log("CREATE: " + assetPath);
			if (assetPath.EndsWith(".asset"))
			{
				var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
				Debug.Log(asset);
				if (asset is Tile3DAssetBase tileBase)
					Tile3DAssetRegister.Singleton.Add(tileBase);
			}
		}*/
	}
}
