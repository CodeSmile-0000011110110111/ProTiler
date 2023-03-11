using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.MeshExtractor
{
    public static class AssetExporter
    {
#if UNITY_EDITOR
        public static void SaveMeshAsAsset(Mesh mesh, string assetPath, bool logFilePaths = true)
        {
            if (mesh == null)
                return;

            // Ensure the path starts with "Assets/".
            if (!assetPath.StartsWith("Assets"))
            {
                if (assetPath.StartsWith("/"))
                {
                    assetPath = "Assets" + assetPath;
                }
                else
                {
                    assetPath = "Assets/" + assetPath;
                }
            }

            string dirPath = System.IO.Path.GetDirectoryName(Application.dataPath + "/../" + assetPath);
            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }

            // Check if the asset already exists.
            var existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
            if (existingMesh != null)
            {
                Undo.RegisterCompleteObjectUndo(existingMesh, "Create new mesh");
            }

            AssetDatabase.CreateAsset(mesh, assetPath);
            AssetDatabase.SaveAssets();
            // Important to force the reimport to avoid the "SkinnedMeshRenderer: Mesh has
            // been changed to one which is not compatibile with the expected mesh data size
            // and vertex stride." error.
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();

            if (logFilePaths)
                Logger.LogMessage($"Saved new mesh under <color=yellow>'{assetPath}'</color>.");
        }
#endif

    }
}

