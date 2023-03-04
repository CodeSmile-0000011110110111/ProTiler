using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmile.UnityEditor
{
	public class TileInspectorTest : EditorWindow
	{
		private Label m_Label;

		[MenuItem("Tools/Tile Inspector")]
		public static void ShowExample()
		{
			var wnd = GetWindow<TileInspectorTest>();
			wnd.titleContent = new GUIContent("TileInspectorTest");
		}

		public void CreateGUI()
		{
			// Each editor window contains a root VisualElement object
			var root = rootVisualElement;

			// VisualElements objects can contain other VisualElement following a tree hierarchy.
			m_Label = new Label("Hello World! From C#");
			root.Add(m_Label);
		}

		public void UpdateTileGridPos(Vector2 mousePos, Vector3 origin)
		{
				var ray = HandleUtility.GUIPointToWorldRay(mousePos);
				if (Ray.IntersectsVirtualPlane(ray, out var intersectPoint))
				{
					var gridSize = new Vector3Int(30, 1, 30);
					var gridPoint = HandlesExt.SnapPointToGrid(intersectPoint + origin, gridSize);
					//Debug.Log($"hit: {gridPoint} from intersect point {intersectPoint}");
					m_Label.text = $"hit: {gridPoint} from intersect point {intersectPoint}";
				}
		}
	}
}