// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using CodeSmile.ProTiler.Data;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmile.Editor.ProTiler.Windows
{
	public class TileSetWindow : EditorWindow
	{
		private const string WindowName = "TileSet Tiles";
		private static readonly Vector2Int TileImageSize = new(100, 100);
		private static readonly Vector2Int MinTileImageSize = new(50, 50);

		private static readonly Color TileHighlightColor = new(.8f, 1f, .8f);

		private readonly int m_TilesPerRow = 5;

		[NonSerialized] private TileLayer m_ActiveLayer;
		private ListView m_TileSetTilesList;
		private Button m_HighlightedTileButton;

		[MenuItem("Tools/" + Names.TileEditor + "/" + WindowName)]
		public static void ShowExample()
		{
			var wnd = GetWindow<TileSetWindow>();
			wnd.titleContent = new GUIContent(WindowName);
		}

		private static Texture2D GetPrefabPreview(GameObject prefab)
		{
			Texture2D tex = null;
			var path = AssetDatabase.GetAssetPath(prefab);
			var editor = UnityEditor.Editor.CreateEditor(prefab);
			if (editor != null)
			{
				tex = editor.RenderStaticPreview(path, null, TileImageSize.x, TileImageSize.y);
				DestroyImmediate(editor);
			}
			return tex;
		}

		private void OnDisable() => AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;

		public void CreateGUI()
		{
			AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
			RecreateGUI();
		}

		private void OnSelectionChange() => UpdateSelectedLayer();

		private void OnAfterAssemblyReload() => RecreateGUI();

		private void UpdateSelectedLayer()
		{
			var activeGameObject = Selection.activeGameObject;
			if (activeGameObject != null)
			{
				var tileLayer = activeGameObject.GetComponent<TileLayer>();
				if (tileLayer != null)
				{
					if (m_ActiveLayer != tileLayer)
					{
						m_ActiveLayer = tileLayer;
						RecreateGUI();
					}
				}
			}
		}

		private void RecreateGUI()
		{
			UpdateSelectedLayer();
			if (m_ActiveLayer == null || m_ActiveLayer.TileSet == null || rootVisualElement == null)
				return;

			m_HighlightedTileButton = null;
			rootVisualElement.Clear();

			var tileSet = m_ActiveLayer.TileSet;
			var tileCount = tileSet.Count;
			var rowCount = tileCount / m_TilesPerRow + (tileCount % m_TilesPerRow == 0 ? 0 : 1);

			var scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
			rootVisualElement.Add(scrollView);

			VisualElement listView = null;
			var selectedIndex = ProTilerState.instance.DrawTileSetIndex;
			for (var index = 0; index < tileSet.Tiles.Count; index++)
			{
				if (index % m_TilesPerRow == 0)
				{
					listView = CreateTileRowListView();
					scrollView.Add(listView);
				}

				var tile = tileSet.Tiles[index];
				var button = CreateTileButton(tile, index);

				listView.hierarchy.Add(button);

				if (index == selectedIndex)
					SetHighlightedButton(button);
			}
		}

		private VisualElement CreateTileRowListView()
		{
			var listView = new VisualElement();
			var style = listView.style;
			style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
			style.justifyContent = new StyleEnum<Justify>(Justify.FlexStart);
			style.alignItems = new StyleEnum<Align>(Align.Stretch);
			style.alignContent = new StyleEnum<Align>(Align.Stretch);
			style.minWidth = m_TilesPerRow * TileImageSize.x;
			style.minHeight = TileImageSize.y;
			style.marginBottom = style.marginLeft = style.marginRight = style.marginTop = 0;
			style.paddingBottom = style.paddingLeft = style.paddingRight = style.paddingTop = 0;

			rootVisualElement.Add(listView);
			return listView;
		}

		private Button CreateTileButton(TileSetTile tile, int index)
		{
			var name = ""; //tile.Prefab.name;
			var button = new Button { text = name, userData = new TileSetTileInfo { TileSetIndex = index, Tile = tile } };
			button.RegisterCallback<ClickEvent>(OnTileSetTileClicked);
			var style = button.style;
			style.width = TileImageSize.x;
			style.height = TileImageSize.y;
			style.minWidth = MinTileImageSize.x;
			style.minHeight = MinTileImageSize.y;
			style.maxWidth = TileImageSize.x;
			style.maxHeight = TileImageSize.y;
			style.marginBottom = style.marginLeft = style.marginRight = style.marginTop = 0;
			style.paddingBottom = style.paddingLeft = style.paddingRight = style.paddingTop = 0;
			style.justifyContent = new StyleEnum<Justify>(Justify.FlexStart);
			style.alignItems = new StyleEnum<Align>(Align.Stretch);
			style.alignContent = new StyleEnum<Align>(Align.Stretch);
			style.fontSize = new StyleLength(new Length(8));

			style.flexShrink = 1;
			style.flexGrow = 1;
			style.backgroundImage = GetPrefabPreview(tile.Prefab);
			return button;
		}

		private void OnTileSetTileClicked(ClickEvent evt)
		{
			var button = evt.currentTarget as Button;
			var tileInfo = (TileSetTileInfo)button.userData;
			if (tileInfo.Tile != null && tileInfo.Tile.Prefab != null)
			{
				Debug.Log("clicked: " + tileInfo.Tile.Prefab.name);
				ProTilerState.instance.DrawTileSetIndex = tileInfo.TileSetIndex;

				SetHighlightedButton(button);
			}
		}

		private void SetHighlightedButton(Button button)
		{
			ClearButtonHighlight(m_HighlightedTileButton);
			m_HighlightedTileButton = button;
			SetButtonHighlight(m_HighlightedTileButton);
		}

		private void SetButtonHighlight(Button button)
		{
			if (button != null)
				button.style.unityBackgroundImageTintColor = new StyleColor(TileHighlightColor);
		}

		private void ClearButtonHighlight(Button button)
		{
			if (button != null)
				button.style.unityBackgroundImageTintColor = new StyleColor(Color.white);
		}

		private struct TileSetTileInfo
		{
			public int TileSetIndex;
			public TileSetTile Tile;
		}
	}
}