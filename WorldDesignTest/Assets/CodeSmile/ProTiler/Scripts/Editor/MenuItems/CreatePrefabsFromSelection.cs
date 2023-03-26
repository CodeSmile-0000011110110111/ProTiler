// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

// This script creates a new menu item Examples>Create Prefab in the main menu.
// Use it to create Prefab(s) from the selected GameObject(s).
// It is placed in the root Assets folder.

using CodeSmile;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CodeSmileEditor.Tile
{
	public static class CreatePrefabsFromSelection
	{
		private const string MenuItemText = "Tools/CodeSmile/Create Prefabs from Model Prefabs";

		[MenuItem(MenuItemText)]
		private static void CreatePrefabs()
		{
			var selectedObjects = Selection.gameObjects;

			try
			{
				AssetDatabase.StartAssetEditing();

				Debug.Log($"Creating {selectedObjects.Length} prefabs ...");
				foreach (var gameObject in selectedObjects)
				{
					var path = AssetDatabase.GetAssetPath(gameObject);
					var parentFolder = Path.GetDirectoryName(path);

					var prefabsFolder = $"{parentFolder}/Prefabs";
					if (Directory.Exists(prefabsFolder) == false)
						AssetDatabase.CreateFolder(parentFolder, "Prefabs");

					// Make sure the file name is unique, in case an existing Prefab has the same name.
					var assetPath = AssetDatabase.GenerateUniqueAssetPath($"{prefabsFolder}/{gameObject.name}.prefab");

					// Create the new Prefab and log whether Prefab was saved successfully.
					var instance = PrefabUtility.InstantiatePrefab(gameObject) as GameObject;
					var prefabRoot = new GameObject(gameObject.name);

					var instanceTransform = instance.transform;
					var prefabRootTransform = prefabRoot.transform;
					prefabRootTransform.position = instanceTransform.position;
					prefabRootTransform.rotation = instanceTransform.rotation;
					prefabRootTransform.localScale = instanceTransform.localScale;

					var instanceMeshRenderer = instance.GetComponent<MeshRenderer>();
					if (instanceMeshRenderer != null)
					{
						var rootMeshFilter = prefabRoot.AddComponent<MeshFilter>();
						var rootMeshRenderer = prefabRoot.AddComponent<MeshRenderer>();
						rootMeshFilter.sharedMesh = instance.GetComponent<MeshFilter>().sharedMesh;
						rootMeshRenderer.sharedMaterials = instanceMeshRenderer.sharedMaterials;

						if (instance.GetComponent<MeshCollider>() != null)
							prefabRoot.AddComponent<MeshCollider>();
						else
							prefabRoot.AddComponent<BoxCollider>();
					}
					else
					{
						foreach (Transform child in instanceTransform)
						{
							var childInstance = Object.Instantiate(child.gameObject, prefabRootTransform);
							childInstance.name = childInstance.name.Replace("(Clone)", "");
						}
					}

					PrefabUtility.SaveAsPrefabAssetAndConnect(prefabRoot, assetPath, InteractionMode.UserAction, out var success);
					if (success == false)
						Debug.LogError($"Prefab save FAILED: {assetPath}");

					instance.DestroyInAnyMode();
					prefabRoot.DestroyInAnyMode();
				}
			}
			finally
			{
				AssetDatabase.StopAssetEditing();
			}
		}

		// Disable the menu item if no selection is in place.
		[MenuItem(MenuItemText, true)]
		private static bool ValidateCreatePrefab() =>
			Selection.activeGameObject != null && EditorUtility.IsPersistent(Selection.activeGameObject);
	}
}