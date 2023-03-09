using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace EHandles
{
    internal partial class AttributesHandler
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            new AttributesHandler();
        }

        private TargetsProvider m_TargetsProvider;
        private TargetScript[] m_Targets;

        private AttributesHandler()
        {
            m_TargetsProvider = new TargetsProvider();

            SceneView.duringSceneGui +=  OnSceneGUI;
            ScriptTracker.OnTargetScriptsChanged += OnTargetScriptsChanged;
        }

        private void OnTargetScriptsChanged()
        {
            m_TargetsProvider.FindTargets(ref m_Targets, ScriptTracker.targetScripts);
            Hotkeys.Clear();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!EHandlesSettings.enabled || !EHandlesSettings.showHandles) return;

            Hotkeys.UpdateHotkeys(Event.current);

            if (m_Targets == null) return;

            for (int i = 0; i < m_Targets.Length; i++)
            {
                m_Targets[i].OnSceneGUI();
            }
        }
    }
}