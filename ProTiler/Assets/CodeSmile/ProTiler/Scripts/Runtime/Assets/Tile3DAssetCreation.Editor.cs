// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

#if UNITY_EDITOR

using CodeSmile.Extensions;
using CodeSmile.ProTiler.Assets;
using CodeSmile.ProTiler.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.ProTiler.Editor.Creation
{
	public static partial class Tile3DAssetCreation
	{
		[ExcludeFromCodeCoverage] [MenuItem(Menus.CreateTilesFromSelectedPrefabs)]
		public static void CreateMultipleRegisteredAssetsWithSelection()
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
					EditorUtility.SetDirty(tileAsset);
					AssetDatabase.SaveAssetIfDirty(tileAsset);

					Debug.Log($"Create {tilePath} {tileAsset.GetInstanceID()} with " +
					          $"{tileAsset.Prefab.name} {tileAsset.Prefab.GetInstanceID()}");

					createdTiles.Add(tileAsset);
				}
			}

			Selection.objects = createdTiles.ToArray();
		}

		[ExcludeFromCodeCoverage] [MenuItem(Menus.CreateTilesFromSelectedPrefabs, true)]
		public static bool ValidateCreateTilesFromSelectedPrefabs() => CountPrefabsInSelection() > 0;

		public static T CreateRegisteredAsset<T>(string path) where T : Tile3DAssetBase
		{
			var tileAsset = AssetDatabaseExt.CreateScriptableObjectAssetAndDirectory<T>(path);
			Tile3DAssetRegister.Singleton.Add(tileAsset);
			return tileAsset;
		}

		public static void CreateRegisteredAssetWithSelection<T>(string proposedAssetName,
			Action<T> createdCallback = null, Action canceledCallback = null) where T : Tile3DAsset
		{
			if (CountPrefabsInSelection() > 1)
				CreateMultipleRegisteredAssetsWithSelection();
			else
				CreateRegisteredAssetWithCallbacks(proposedAssetName, createdCallback, canceledCallback);
		}

		private static void CreateRegisteredAssetWithCallbacks<T>(string proposedAssetName,
			Action<T> createdCallback, Action canceledCallback) where T : Tile3DAsset
		{
			var tileAsset = CreateInstance<T>();
			var endAction = CreateEndActionObject(createdCallback, canceledCallback);
			var assetName = NicifyProposedAssetName<T>(proposedAssetName, ".asset");
			var thumbnail = AssetPreview.GetMiniThumbnail(tileAsset);
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(tileAsset.GetInstanceID(),
				endAction, assetName, thumbnail, null);
		}

		private static CreateTile3DAssetEndAction CreateEndActionObject<T>(Action<T> createdCallback, Action canceledCallback)
			where T : Tile3DAsset
		{
			var endAction = ScriptableObject.CreateInstance<CreateTile3DAssetEndAction>();
			endAction.CreatedCallback = createdCallback != null ? tileAsset => createdCallback((T)tileAsset) : null;
			endAction.CanceledCallback = canceledCallback;
			if (Selection.activeObject is GameObject go && go.IsPrefab())
				endAction.SelectedPrefab = go;
			return endAction;
		}

		private static int CountPrefabsInSelection()
		{
			var prefabCount = 0;
			foreach (var gameObject in Selection.gameObjects)
			{
				if (gameObject.IsPrefab())
					prefabCount++;
			}

			return prefabCount;
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

		private class CreateTile3DAssetEndAction : EndNameEditAction
		{
			public Tile3DAssetBase TileAsset;
			public GameObject SelectedPrefab;
			public Action<Object> CreatedCallback;
			public Action CanceledCallback;

			public override void Action(int instanceId, string pathName, string resourceFile)
			{
				var obj = EditorUtility.InstanceIDToObject(instanceId);
				TileAsset = obj as Tile3DAssetBase;

				AssetDatabase.CreateAsset(TileAsset, AssetDatabase.GenerateUniqueAssetPath(pathName));
				Selection.activeObject = TileAsset;

				Tile3DAssetRegister.Singleton.Add(TileAsset);

				TileAsset.Prefab = SelectedPrefab;
				CreatedCallback?.Invoke(TileAsset);

				EditorUtility.SetDirty(TileAsset);
				AssetDatabase.SaveAssetIfDirty(TileAsset);
			}

			public override void Cancelled(int instanceId, string pathName, string resourceFile)
			{
				Selection.activeObject = TileAsset = null;
				CanceledCallback?.Invoke();
			}
		}
	}
}
#endif
