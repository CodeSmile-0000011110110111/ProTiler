// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Assets;
using UnityEngine;

namespace CodeSmile.ProTiler.Editor.Creation
{
	public static class Tile3DAssetCreation
	{
		public static T CreateInstance<T>() where T : Tile3DAssetBase => ScriptableObject.CreateInstance<T>();
	}
}
