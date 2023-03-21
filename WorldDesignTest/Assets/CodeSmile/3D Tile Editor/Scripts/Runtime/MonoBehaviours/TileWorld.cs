// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Tile
{
	[ExecuteInEditMode]
	public sealed class TileWorld : MonoBehaviour
	{
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
				CreateNewLayer("Layer");
			}
		}

		private void CreateNewLayer(string name)
		{
			var go = new GameObject(name, typeof(TileLayer));
			go.hideFlags = Global.TileHideFlags;
			go.transform.parent = transform;
		}
	}
}