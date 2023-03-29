// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile3DPaletteWindow : EditorWindow
{
	[SerializeField] private VisualTreeAsset m_VisualTreeAsset;

	[MenuItem(Global.RootMenuName + "/" + Global.TileEditorName + "/" + Global.Tile3DPaletteWindowName)]
	public static void ShowWindow()
	{
		var wnd = GetWindow<Tile3DPaletteWindow>();
		wnd.titleContent = new GUIContent(Global.Tile3DPaletteWindowName);
	}

	public void CreateGUI()
	{
		var root = rootVisualElement;
		VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
		root.Add(labelFromUXML);
	}
}