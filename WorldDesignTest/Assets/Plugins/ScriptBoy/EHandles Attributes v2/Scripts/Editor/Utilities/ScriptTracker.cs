using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;


namespace EHandles
{
    internal static class ScriptTracker
    {
        public static MonoBehaviour[] allScripts;
        public static MonoBehaviour[] selectedScripts;
        public static MonoBehaviour[] markedScripts;
        public static MonoBehaviour[] targetScripts;

        private static bool allScriptsHasChanged;
        private static bool selectedScriptsHasChanged;
        private static bool markedScriptsHasChanged;

        private static bool componentWasAdded;
        private static bool hierarchyWindowChanged;
        private static bool selectionChanged;

        public static Action OnAllScriptsChanged;
        public static Action OnMarkedScriptsChanged;
        public static Action OnTargetScriptsChanged;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            allScripts = new MonoBehaviour[0];
            selectedScripts = new MonoBehaviour[0];
            markedScripts = new MonoBehaviour[0];
            targetScripts = new MonoBehaviour[0];

            hierarchyWindowChanged = true;
            selectionChanged = true;

            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.hierarchyChanged += HierarchyWindowChanged;
            Selection.selectionChanged += SelectionChanged;
            ObjectFactory.componentWasAdded += ComponentWasAdded;
        }

        private static void ComponentWasAdded(Component component)
        {
            if (!(component is MonoBehaviour)) return;

            if (Application.isPlaying)
            {
                componentWasAdded = true;
            }
            else
            {
                RefreshAllScripts();
                RefreshSelectedScripts();
            }
        }

        private static void SelectionChanged()
        {
            if (Application.isPlaying)
            {
                selectionChanged = true;
            }
            else
            {
                RefreshSelectedScripts();
            }
        }

        private static void HierarchyWindowChanged()
        {
            if (Application.isPlaying)
            {
                hierarchyWindowChanged = true;
            }
            else
            {
                RefreshAllScripts();
            }
        }

        private static void RefreshAllScripts()
        {
            MonoBehaviour[] newScripts = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true);
            if (newScripts.SequenceEqual(allScripts)) return;
            allScripts = newScripts;
            allScriptsHasChanged = true;
        }

        private static void RefreshSelectedScripts()
        {
            MonoBehaviour[] newScripts = Selection.GetFiltered<MonoBehaviour>(SelectionMode.Editable);
            List<MonoBehaviour> list = new List<MonoBehaviour>(newScripts.Length);
            foreach (var e in newScripts)
            {
                list.AddRange(e.gameObject.GetComponents<MonoBehaviour>());
            }
            newScripts = list.ToArray();
            if (selectedScripts.SequenceEqual(newScripts)) return;
            selectedScripts = newScripts;
            selectedScriptsHasChanged = true;
        }

        private static void RefreshMarkedScripts()
        {
            List<MonoBehaviour> list = new List<MonoBehaviour>();
            var attrType = typeof(ExecuteAlwaysAttribute);
            foreach (var script in allScripts)
            {
                if (script.GetType().GetCustomAttributes(attrType, true).Length > 0)
                {
                    list.Add(script);
                }
            }

            if (markedScripts.SequenceEqual(list)) return;

            markedScriptsHasChanged = true;
            markedScripts = list.ToArray();
        }

        private static void RefreshTargetScripts()
        {
            HashSet<MonoBehaviour> list = new HashSet<MonoBehaviour>(selectedScripts);
            foreach (var e in markedScripts)
            {
                list.Add(e);
            }
            targetScripts = list.ToArray();
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (componentWasAdded)
            {
                componentWasAdded = false;
                RefreshAllScripts();
                RefreshSelectedScripts();
            }

            if (hierarchyWindowChanged)
            {
                hierarchyWindowChanged = false;
                RefreshAllScripts();
            }

            if (selectionChanged)
            {
                selectionChanged = false;
                RefreshSelectedScripts();
            }

            if (allScriptsHasChanged)
            {
                allScriptsHasChanged = false;
                OnAllScriptsChanged?.Invoke();
                RefreshMarkedScripts();
            }

            if (selectedScriptsHasChanged || markedScriptsHasChanged)
            {
                if (markedScriptsHasChanged) OnMarkedScriptsChanged?.Invoke();

                selectedScriptsHasChanged = markedScriptsHasChanged = false;
                RefreshTargetScripts();
                OnTargetScriptsChanged.Invoke();
            }
        }
    }
}