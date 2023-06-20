// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Attributes;
using CodeSmile.Editor.Extensions;
using CodeSmile.Extensions;
using CodeSmile.ProTiler3.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.ProTiler3.Editor.Creation
{
	[FullCovered]
	public static class Tile3DAssetCreation
	{
		public static T CreateInstance<T>(GameObject prefab = null) where T : Tile3DAssetBase
		{
			var instance = ScriptableObject.CreateInstance<T>();
			if (prefab != null && prefab.IsPrefab())
				instance.Prefab = prefab;
			return instance;
		}

		public static T CreateRegisteredAsset<T>(string path) where T : Tile3DAssetBase
		{
			var tileAsset = AssetDatabaseExt.CreateScriptableObjectAssetAndDirectory<T>(path);
			Tile3DAssetRegister.Singleton.Add(tileAsset);
			return tileAsset;
		}

		[MenuItem(Menus.CreateTileAssetMenuText)] [ExcludeFromCodeCoverage]
		public static void CreateTileAssetWithSelection() =>
			CreateRegisteredAssetWithSelection<Tile3DAsset>(nameof(Tile3DAsset));

		[MenuItem(Menus.CreateTilesFromSelectedPrefabs)] [ExcludeFromCodeCoverage]
		public static void CreateRegisteredAssetsWithSelection()
		{
			var createdTiles = new List<Tile3DAssetBase>();
			foreach (var gameObject in Selection.gameObjects)
			{
				if (gameObject.IsPrefab())
				{
					var prefabPath = AssetDatabase.GetAssetPath(gameObject);
					var tilePath = Path.ChangeExtension(prefabPath, "Tile3D.asset");
					var tileAsset = CreateRegisteredAsset<Tile3DAsset>(tilePath);
					tileAsset.Prefab = gameObject;
					createdTiles.Add(tileAsset);

					AssetDatabaseExt.ForceSaveAsset(tileAsset);
				}
			}

			Selection.objects = createdTiles.ToArray();
		}

		[MenuItem(Menus.CreateTilesFromSelectedPrefabs, true)] [ExcludeFromCodeCoverage]
		public static bool ValidateCreateTilesFromSelectedPrefabs() => SelectionExt.PrefabCount() > 0;

		[ExcludeFromCodeCoverage]
		public static void CreateRegisteredAssetWithSelection<T>(string proposedAssetName,
			Action<T> createdCallback = null, Action canceledCallback = null) where T : Tile3DAssetBase
		{
			if (SelectionExt.PrefabCount() > 1)
				CreateRegisteredAssetsWithSelection();
			else
				CreateRegisteredAssetWithCallbacks(proposedAssetName, createdCallback, canceledCallback);
		}

		[ExcludeFromCodeCoverage]
		private static void CreateRegisteredAssetWithCallbacks<T>(string proposedAssetName,
			Action<T> createdCallback, Action canceledCallback) where T : Tile3DAssetBase
		{
			var tileAsset = CreateInstance<T>(SelectionExt.GetSelectedPrefab());
			var endAction = CreateEndActionObject(createdCallback, canceledCallback);
			var assetName = NicifyProposedAssetName<T>(proposedAssetName, ".asset");
			var thumbnail = AssetPreview.GetMiniThumbnail(tileAsset);
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(tileAsset.GetInstanceID(),
				endAction, assetName, thumbnail, null);
		}

		[ExcludeFromCodeCoverage]
		private static CreateTile3DAssetEndAction CreateEndActionObject<T>(Action<T> createdCallback, Action canceledCallback)
			where T : Tile3DAssetBase
		{
			var endAction = ScriptableObject.CreateInstance<CreateTile3DAssetEndAction>();
			endAction.CreatedCallback = createdCallback != null ? tileAsset => createdCallback((T)tileAsset) : null;
			endAction.CanceledCallback = canceledCallback;
			return endAction;
		}

		[ExcludeFromCodeCoverage]
		private static string NicifyProposedAssetName<T>(string proposedAssetName, string extension) where T : Tile3DAssetBase
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

		[ExcludeFromCodeCoverage]
		private sealed class CreateTile3DAssetEndAction : EndNameEditAction
		{
			private Tile3DAssetBase TileAsset { get; set; }
			internal Action<Object> CreatedCallback { get; set; }
			internal Action CanceledCallback { get; set; }

			public override void Action(int instanceId, string pathName, string resourceFile)
			{
				TileAsset = EditorUtility.InstanceIDToObject(instanceId) as Tile3DAssetBase;

				CreateRegisteredAsset(pathName);

				Selection.activeObject = TileAsset;
				CreatedCallback?.Invoke(TileAsset);
			}

			private void CreateRegisteredAsset(string pathName)
			{
				AssetDatabase.CreateAsset(TileAsset, AssetDatabase.GenerateUniqueAssetPath(pathName));
				Tile3DAssetRegister.Singleton.Add(TileAsset);
			}

			public override void Cancelled(int instanceId, string pathName, string resourceFile)
			{
				Selection.activeObject = TileAsset = null;
				CanceledCallback?.Invoke();
			}
		}
	}
}
