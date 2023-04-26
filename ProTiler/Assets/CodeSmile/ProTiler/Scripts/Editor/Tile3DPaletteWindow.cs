// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.ProTiler.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmile.ProTiler.Editor
{
	public class Tile3DPaletteWindow : EditorWindow
	{
		[SerializeField] private VisualTreeAsset m_VisualTreeAsset;

		[MenuItem(Menus.RootMenu + "/" + Names.TileEditor + "/" + Names.Tile3DPaletteWindow)]
		public static void ShowWindow()
		{
			var wnd = GetWindow<Tile3DPaletteWindow>();
			wnd.titleContent = new GUIContent(Names.Tile3DPaletteWindow);
		}

		public void CreateGUI()
		{
			var root = rootVisualElement;
			VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
			root.Add(labelFromUXML);
		}
	}
}
