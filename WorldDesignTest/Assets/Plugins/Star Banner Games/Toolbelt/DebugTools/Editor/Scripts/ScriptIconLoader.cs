using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace SBG.Toolbelt.DebugTools.Editor
{
	public static class ScriptIconLoader
	{
		[InitializeOnLoadMethod]
		public static void SetScriptIcons()
        {
            if (AssetDatabase.IsValidFolder("Assets/Gizmos/SBG/Toolbelt/DebugTools")) return;

            if (!AssetDatabase.IsValidFolder("Assets/Gizmos")) AssetDatabase.CreateFolder("Assets", "Gizmos");

            AssetDatabase.CreateFolder("Assets/Gizmos", "SBG");
            AssetDatabase.CreateFolder("Assets/Gizmos/SBG", "Toolbelt");
            AssetDatabase.CreateFolder("Assets/Gizmos/SBG/Toolbelt", "DebugTools");

            string[] iconPaths = GetIconFolderPaths();

			if (iconPaths == null || iconPaths.Length == 0) return;

            foreach (string path in iconPaths)
            {
				string relativePath = path.Replace(Application.dataPath, "Assets");
				string fileName = Path.GetFileName(path);

				AssetDatabase.CopyAsset(relativePath, $"Assets/Gizmos/SBG/Toolbelt/DebugTools/{fileName}");
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private static string[] GetIconFolderPaths()
        {
			string targetPath = @"Star Banner Games\Toolbelt\DebugTools\Editor\Icons";

			string[] dirs = Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories).Where(s => s.EndsWith(targetPath)).ToArray();

			if (dirs == null || dirs.Length == 0) return null;

			return Directory.GetFiles(dirs[0], "*.png", SearchOption.TopDirectoryOnly);
		}
	}
}