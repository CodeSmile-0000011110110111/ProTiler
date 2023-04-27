// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Data;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.ProTiler.Editor.Creation
{
	public static partial class Tile3DAssetCreation
	{
#if UNITY_EDITOR
		[MenuItem(Menus.CreateTilesFromSelectedPrefabs)]
		public static void CreateTilesFromSelectedPrefabs()
		{
			foreach (var gameObject in Selection.gameObjects)
			{
				if (gameObject.IsPrefab())
				{
					var prefabPath = AssetDatabase.GetAssetPath(gameObject);
					var tilePath = Path.ChangeExtension(prefabPath, "Tile3D.asset");
					var tileAsset = CreateRegisteredAsset<Tile3DAsset>(tilePath);
					tileAsset.Prefab = gameObject;
				}
			}

			AssetDatabase.SaveAssets();
		}

		[MenuItem(Menus.CreateTilesFromSelectedPrefabs, true)]
		public static bool ValidateCreateTilesFromSelectedPrefabs() => IsPrefabInSelection();

		private static bool IsPrefabInSelection()
		{
			foreach (var gameObject in Selection.gameObjects)
			{
				if (gameObject.IsPrefab())
					return true;
			}

			return false;
		}

		public static T CreateRegisteredAsset<T>(string path) where T : Tile3DAssetBase
		{
			var tileAsset = AssetDatabaseExt.CreateScriptableObjectAssetAndDirectory<T>(path);
			Tile3DAssetRegister.Singleton.Add(tileAsset);
			return tileAsset;
		}

		public static void CreateRegisteredAssetWithSelection<T>(string proposedAssetName,
			Action<T> createdCallback = null,
			Action canceledCallback = null) where T : Tile3DAsset
		{
			var endAction = ScriptableObject.CreateInstance<CreateTileAssetEndAction>();
			endAction.CreatedCallback = createdCallback != null ? tileAsset => createdCallback((T)tileAsset) : null;
			endAction.CanceledCallback = canceledCallback;

			var tileAsset = CreateInstance<T>();
			var assetName = NicifyProposedAssetName<T>(proposedAssetName, ".asset");

			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(tileAsset.GetInstanceID(), endAction, assetName,
				AssetPreview.GetMiniThumbnail(tileAsset), null);
		}

		private static string NicifyProposedAssetName<T>(string proposedAssetName, string extension) where T : Tile3DAsset
		{
			if (Selection.activeObject is GameObject gameObject && gameObject.IsPrefab())
				return gameObject.name + ".Tile3D.asset";

			var assetName = proposedAssetName;
			if (string.IsNullOrWhiteSpace(assetName))
				assetName = "New " + ObjectNames.NicifyVariableName(typeof(T).Name);
			if (assetName.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase) == false)
				assetName += extension;
			return assetName;
		}

		private class CreateTileAssetEndAction : EndNameEditAction
		{
			public Action<Object> CreatedCallback;
			public Action CanceledCallback;

			private static void TrySetPrefabFromSelection(Tile3DAssetBase tileAsset)
			{
				if (Selection.activeObject is GameObject go && go.IsPrefab())
					tileAsset.Prefab = go;
			}

			public override void Action(int instanceId, string pathName, string resourceFile)
			{
				var tileAsset = EditorUtility.InstanceIDToObject(instanceId) as Tile3DAssetBase;

				// set the selected prefab (if any) as the tile's prefab
				TrySetPrefabFromSelection(tileAsset);
				AssetDatabase.CreateAsset(tileAsset, AssetDatabase.GenerateUniqueAssetPath(pathName));
				Tile3DAssetRegister.Singleton.Add(tileAsset);

				CreatedCallback?.Invoke(tileAsset);
			}

			public override void Cancelled(int instanceId, string pathName, string resourceFile)
			{
				Selection.activeObject = null;
				CanceledCallback?.Invoke();
			}
		}
#endif
	}
}
