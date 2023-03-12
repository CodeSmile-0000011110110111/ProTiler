// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Tile
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(TileRenderer))]
	public sealed partial class TileWorld : MonoBehaviour, ISerializationCallbackReceiver
	{
		[SerializeField] private int m_ActiveLayerIndex;
		[SerializeField] private List<TileLayer> m_Layers = new();

		private void Reset()
		{
#if UNITY_EDITOR
			var guids1 = AssetDatabase.FindAssets("t:TileWorldEditorSettings");
			Debug.Log($"found {guids1.Length} TileWorldEditorSettings asset");
			foreach (var guid1 in guids1)
				Debug.Log(AssetDatabase.GUIDToAssetPath(guid1));
#endif

			if (m_Layers.Count == 0)
			{
				name = nameof(TileWorld);
				m_ActiveLayerIndex = 0;
				m_Layers.Add(new TileLayer(this));
			}

			if (GetComponent<TileRenderer>() == null)
				gameObject.AddComponent<TileRenderer>();
		}

		public void OnBeforeSerialize() {}

		public void OnAfterDeserialize()
		{
			foreach (var layer in m_Layers)
				layer.TileWorld = this;
		}

		public TileLayer ActiveLayer => m_Layers[m_ActiveLayerIndex];
	}
}