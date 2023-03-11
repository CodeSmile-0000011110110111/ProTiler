using System;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Kamgam.MeshExtractor
{
    [InitializeOnLoad]
    public static class MeshExtractorToolActiveState
    {
        static bool _initialized = false;
        public static bool IsActive = false;

        static MeshExtractorToolActiveState()
        {
            init();
        }

        static void init()
        {
            _initialized = true;

            bool active = false;

            // EditorTools available until Unity 2020.1 (2020.2+ does not longer have this class)
#if UNITY_2020_2_OR_NEWER
            ToolManager.activeToolChanged -= onToolChanged;
            ToolManager.activeToolChanged += onToolChanged;

            // active right from the start?
            if (ToolManager.activeToolType == typeof(MeshExtractorTool))
            {
                // Remember: MeshExtractorTool.Instance is still null here! 
                active = true;
                waitForInstance(active);
            }
#else
            EditorTools.activeToolChanged -= onToolChanged;
            EditorTools.activeToolChanged += onToolChanged;

            // active right from the start?
            if (EditorTools.activeToolType == typeof(MeshExtractorTool))
            {
                // Remember: MeshExtractorTool.Instance is still null here! 
                active = true;
                waitForInstance(active);
            }
#endif
        }

        static async void waitForInstance(bool active) 
        {
            float totalWaitTime = 0f; // precaution against endlessly running task
            while (MeshExtractorTool.Instance == null && totalWaitTime < 3000)
            {
                await System.Threading.Tasks.Task.Delay(50);
                totalWaitTime += 50;
            }

            if (totalWaitTime >= 3000)
                return;

            IsActive = active;
            MeshExtractorTool.Instance.OnToolChanged();
        }

        static void onToolChanged()
        {
#if UNITY_2020_2_OR_NEWER
            IsActive = ToolManager.activeToolType == typeof(MeshExtractorTool);
#else
            IsActive = EditorTools.activeToolType == typeof(MeshExtractorTool);
#endif

            if (MeshExtractorTool.Instance != null)
                MeshExtractorTool.Instance.OnToolChanged();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if(!_initialized)
                init();
        }
    }
}
