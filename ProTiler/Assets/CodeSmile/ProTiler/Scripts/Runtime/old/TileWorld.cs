// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Extensions;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.ProTiler
{
	[ExecuteInEditMode]
	public sealed class TileWorld : MonoBehaviour
	{
		private static void SelectFirstLayerInEditor(GameObject layer)
		{
#if UNITY_EDITOR
			Selection.activeGameObject = layer;
#endif
		}

		private void Reset()
		{
#if UNITY_EDITOR
			var guids1 = AssetDatabase.FindAssets("t:TileWorldEditorSettings");
			Debug.Log($"found {guids1.Length} TileWorldEditorSettings asset");
			foreach (var guid1 in guids1)
				Debug.Log(AssetDatabase.GUIDToAssetPath(guid1));

#endif

			// first time init
			if (GetComponentsInChildren<TileLayer>().Length == 0)
			{
				name = nameof(TileWorld);
				var layer = CreateNewTileLayer("TileLayer");
				layer.GetOrAddComponent<TileLayerToolbox>();
				SelectFirstLayerInEditor(layer);
			}
		}

		private GameObject CreateNewTileLayer(string name) => new(name, typeof(TileLayer)) { transform = { parent = transform } };
	}
}