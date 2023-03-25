// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Tile;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmileEditor.Tile
{
	public class TileInspectorWindow : EditorWindow
	{
		private Label m_DebugLabel;
		private const string WindowName = "TileSet Inspector";

		private TileSet m_ActiveTileSet;

		[MenuItem("Tools/" + Const.TileEditorName + "/" + WindowName)]
		public static void ShowExample()
		{
			var wnd = GetWindow<TileInspectorWindow>();
			wnd.titleContent = new GUIContent(WindowName);
			
		}

		private void UpdateActiveTileSet()
		{
			var activeGameObject = Selection.activeGameObject;
			if (activeGameObject != null)
			{
				var tileLayer = activeGameObject.GetComponent<TileLayer>();
				if (tileLayer != null)
				{
					var selectedTileSet = tileLayer.TileSet;
					if (m_ActiveTileSet!=selectedTileSet )
					{
						m_ActiveTileSet = selectedTileSet;
						Repaint();
					}
				}
			}

		}
		
		private void Awake() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void Reset() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnEnable() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnDisable() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnDestroy() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnGUI() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		public void CreateGUI()
		{
			var root = rootVisualElement;

			m_DebugLabel = new Label("Hello World! From C#");
			root.Add(m_DebugLabel);
		}

		private void ModifierKeysChanged() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnAddedAsTab() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnBecameInvisible() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnBecameVisible() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnDidOpenScene() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnFocus() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnHierarchyChange() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnLostFocus() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnMainWindowMove() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnProjectChange() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnSelectionChange()
		{
			UpdateActiveTileSet();
		}

		private void OnTabDetached() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void OnValidate() => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);

		private void ShowButton(Rect rect) => Debug.Log(GetType() + " " + MethodBase.GetCurrentMethod().Name);
	}
}