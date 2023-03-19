// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Tile
{
	[ExecuteInEditMode]
	public sealed partial class TileWorld : MonoBehaviour, ISerializationCallbackReceiver
	{
		private static readonly List<GameObject> m_ToBeDeletedInstances = new();
		[SerializeField] private int m_ActiveLayerIndex;
		[SerializeField] private List<TileLayer> m_Layers = new();
		
		private void Reset()
		{
#if UNITY_EDITOR
			var guids1 = AssetDatabase.FindAssets("t:TileWorldEditorSettings");
			Debug.Log($"found {guids1.Length} TileWorldEditorSettings asset");
			foreach (var guid1 in guids1)
				Debug.Log(AssetDatabase.GUIDToAssetPath(guid1));

			Selection.selectionChanged -= OnSelectionChanged;
			Selection.selectionChanged += OnSelectionChanged;
#endif

			if (m_Layers.Count == 0)
			{
				name = nameof(TileWorld);
				m_ActiveLayerIndex = 0;
				m_Layers.Add(new TileLayer(this));
			}

			if (GetComponent<TileLayerRenderer>() == null)
			{
				gameObject.AddComponent<TileLayerRenderer>();
			}
			if (GetComponent<TileCursorRenderer>() == null)
			{
				gameObject.AddComponent<TileCursorRenderer>();
			}
		}

		private void Update()
		{
			if (m_ToBeDeletedInstances.Count > 0)
			{
				foreach (var instance in m_ToBeDeletedInstances)
					instance.DestroyInAnyMode();
				m_ToBeDeletedInstances.Clear();
			}
		}

		public void OnBeforeSerialize() {}

		public void OnAfterDeserialize()
		{
			foreach (var layer in m_Layers)
				layer.TileWorld = this;
		}

#if UNITY_EDITOR
		private void OnSelectionChanged()
		{
			if (Selection.activeGameObject != null)
			{
				if (Selection.activeGameObject == gameObject)
				{
					// TODO: disable selection outline
				}
				
				/*
				var proxy = Selection.activeGameObject.GetComponentsInParent<TileProxy>();
				if (proxy.Length > 0)
				{
					Debug.Log($"TileProxy: {proxy[0].name}");
					Selection.SetActiveObjectWithContext(proxy[0].gameObject, Selection.activeGameObject);
				}
				*/
				
			}
		}
#endif

		public TileLayer ActiveLayer => m_Layers[m_ActiveLayerIndex];
		public static List<GameObject> ToBeDeletedInstances => m_ToBeDeletedInstances;
	}
}