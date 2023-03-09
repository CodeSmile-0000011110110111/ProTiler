using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;


namespace EHandles
{
    internal partial class SceneButtonsHandler
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            new SceneButtonsHandler();
        }

        private TargetScript[] m_Targets;
        private TargetsProvider m_TargetsProvider;

        private SceneButtonsHandler()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            ScriptTracker.OnAllScriptsChanged += OnAllScriptsChanged;
            ScriptTracker.OnMarkedScriptsChanged += OnMarkedScriptsChanged;   
        }

        private void OnAllScriptsChanged()
        {
            if (EHandlesSettings.optimizeSceneButtons) return;
            RefreshTargets(ScriptTracker.allScripts);
        }


        private void OnMarkedScriptsChanged()
        {
            if (!EHandlesSettings.optimizeSceneButtons) return;
            RefreshTargets(ScriptTracker.markedScripts);
        }

        private void RefreshTargets(MonoBehaviour[] scripts)
        {
            if (m_TargetsProvider)
            {
                m_TargetsProvider.Dispose();
            }

            m_TargetsProvider = new TargetsProvider(scripts);
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!EHandlesSettings.showSceneButtons) return;

            if (m_TargetsProvider && m_TargetsProvider.isDone)
            {
                m_Targets = m_TargetsProvider.results;
                m_TargetsProvider.Dispose();
            }

            if (m_Targets == null) return;

            DrawButtons();
        }

        private void DrawButtons()
        {
            Handles.BeginGUI();
            for (int i = 0; i < m_Targets.Length; i++)
            {
                var target = m_Targets[i];
                if (target == null) continue;
                target.DrawButtons(Styles.buttonContent, Styles.buttonStyle);
            }
            Handles.EndGUI();
        }
    }
}