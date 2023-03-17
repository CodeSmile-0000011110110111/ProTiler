using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmileEditor.Tile
{
	public class TileInspectorWindow : EditorWindow
	{
		[MenuItem("Window/UI Toolkit/TileInspectorWindow")]
		public static void ShowExample()
		{
			TileInspectorWindow wnd = GetWindow<TileInspectorWindow>();
			wnd.titleContent = new GUIContent("TileInspectorWindow");
		}

		public void CreateGUI()
		{
			// Each editor window contains a root VisualElement object
			VisualElement root = rootVisualElement;

			// VisualElements objects can contain other VisualElement following a tree hierarchy.
			VisualElement label = new Label("Hello World! From C#");
			root.Add(label);

		}
	}
    
}
