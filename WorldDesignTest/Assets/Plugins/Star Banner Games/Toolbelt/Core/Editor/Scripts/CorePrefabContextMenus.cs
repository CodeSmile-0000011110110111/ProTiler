using UnityEditor;
using UnityEngine;

namespace SBG.Toolbelt.Editor
{
	public static class CorePrefabContextMenus
	{
		[MenuItem("GameObject/Toolbelt/Core/Trigger2D")]
		public static void CreateTrigger2D() => CreateCorePrefab("Trigger2D");

		[MenuItem("GameObject/Toolbelt/Core/Trigger3D")]
		public static void CreateTrigger3D() => CreateCorePrefab("Trigger3D");


		private static void CreateCorePrefab(string resourceName)
		{
			GameObject prefab = Resources.Load<GameObject>(resourceName);

			GameObject obj = GameObject.Instantiate(prefab);
			obj.name = prefab.name;
			Undo.RegisterCreatedObjectUndo(obj, $"Created {obj.name}");
		}
	}
}