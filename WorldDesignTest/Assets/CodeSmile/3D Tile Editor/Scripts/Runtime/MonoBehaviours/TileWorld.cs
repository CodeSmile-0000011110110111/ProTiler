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

		public TileLayer ActiveLayer { get => m_Layers[m_ActiveLayerIndex]; }
		public static List<GameObject> ToBeDeletedInstances { get => m_ToBeDeletedInstances; }

		public void OnBeforeSerialize() {}

		public void OnAfterDeserialize()
		{
			foreach (var layer in m_Layers)
				layer.TileWorld = this;
		}

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

			if (GetComponent<TileLayerRenderer>() == null)
				gameObject.AddComponent<TileLayerRenderer>();
			if (GetComponent<TilePreviewRenderer>() == null)
				gameObject.AddComponent<TilePreviewRenderer>();
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

		/*
		private void OnEnable()
		{
			Undo.undoRedoEvent += (in UndoRedoInfo undo) =>
			{
				Debug.Log($"UndoRedoEvent: {undo} / {undo.undoName} / {undo.undoGroup} / {undo.isRedo}");
			};
			Undo.undoRedoPerformed += () =>
			{
				Debug.Log($"UndoRedoPerformed");
				//ActiveLayer.OnClearTiles?.Invoke();
			};
		}
	*/
	}
}