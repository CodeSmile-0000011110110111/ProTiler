using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SBG.Toolbelt.UI.Editor
{
	public static class UIPrefabContextMenus
	{
		[MenuItem("GameObject/Toolbelt/UI/ResourceBar_Horizontal")]
		public static void CreateResourceBar_Horizontal() => CreateUIPrefab("ResourceBar_Horizontal");

		[MenuItem("GameObject/Toolbelt/UI/ResourceBar_Vertical")]
		public static void CreateResourceBar_Vertical() => CreateUIPrefab("ResourceBar_Vertical");

		[MenuItem("GameObject/Toolbelt/UI/ResourceBar_Radial")]
		public static void CreateResourceBar_Radial() => CreateUIPrefab("ResourceBar_Radial");

		[MenuItem("GameObject/Toolbelt/UI/ResourceCounter")]
		public static void CreateResourceCounter() => CreateUIPrefab("ResourceCounter");

		private static void CreateUIPrefab(string resourceName)
        {
			GameObject prefab = Resources.Load<GameObject>(resourceName);
			Transform parent = GetParent();

			GameObject obj = GameObject.Instantiate(prefab, parent);
			obj.name = prefab.name;
			Undo.RegisterCreatedObjectUndo(obj, $"Created {obj.name}");
		}

		private static Transform GetParent()
        {
			GameObject selection = Selection.activeGameObject;
			Canvas canvas;

			//If current Selection is a Canvas (Child), use its transform
			if (selection != null)
            {
				canvas = selection.transform.root.GetComponent<Canvas>();
				if (canvas != null) return selection.transform;
			}

			//If a Canvas exists, use its transform
			canvas = GameObject.FindObjectOfType<Canvas>(true);
			if (canvas != null) return canvas.transform;

			//Create a new Canvas to use as parent
			GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
			canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
			canvasGO.layer = 5;
			Undo.RegisterCreatedObjectUndo(canvasGO, $"Created Canvas");

			//Create an EventSystem if none is present
			if (GameObject.FindObjectOfType<EventSystem>(true) == null)
            {
				GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
				Undo.RegisterCreatedObjectUndo(eventSystem, $"Created EventSystem");
			}

			return canvasGO.transform;
        }
	}
}