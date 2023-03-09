using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EHandles
{
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T s_Instance;
        public static T instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var path = GetDefaultPath();
                    s_Instance = FindInstance(path);
                    if (s_Instance == null)
                    {
                        s_Instance = CreateInstance(path);
                    }
                }
      
                return s_Instance;
            }
        }

        private static string GetDefaultPath()
        {
            foreach (var attr in typeof(T).GetCustomAttributes(typeof(FilePathAttribute), true))
            {
                return (attr as FilePathAttribute).path;
            }

            return "";
        }

        private static T FindInstance(string defaultPath)
        {
            var obj = AssetDatabase.LoadAssetAtPath<T>(defaultPath);

            if (obj == null)
            {
                var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");

                if (guids != null && guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    obj = AssetDatabase.LoadAssetAtPath<T>(path);
                }

                if (guids.Length > 1)
                {
                    Debug.LogWarning($"EHandles: There are multiple instances of the {typeof(T)}.");
                }

                if (obj == null) Debug.LogError($"EHandles: Can't find the file in the following path \n {defaultPath}");
            }

            return obj;
        }

        private new static T CreateInstance(string  path)
        {
            if (path == "") return null;

            var obj = CreateInstance<T>();
            AssetDatabase.CreateAsset(obj, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = obj;

            Debug.Log($"EHandles: The {typeof(T).Name} file just created.");

            return obj;
            
        }
    }

    internal class FilePathAttribute : System.Attribute
    {
        public string path;

        public FilePathAttribute(string path)
        {
            this.path = path;
        }
    } 
}