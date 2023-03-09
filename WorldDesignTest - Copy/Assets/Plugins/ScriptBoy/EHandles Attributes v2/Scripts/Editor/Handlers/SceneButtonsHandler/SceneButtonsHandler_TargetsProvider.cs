using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;


namespace EHandles
{
    internal partial class SceneButtonsHandler
    {
        private class TargetsProvider
        {
            private MonoBehaviour[] m_Scripts;
            private Thread m_Thread;
            public TargetScript[] results;
            public bool isDone;
            public bool m_IsDisposed;

            public TargetsProvider(MonoBehaviour[] scripts)
            {
                m_Scripts = scripts;
                m_Thread = new Thread(FindTargets);
                m_Thread.Start();
            }

            private void FindTargets()
            {
                Thread.Sleep(500);

                if (m_IsDisposed) return;

                List<TargetScript> targetList = new List<TargetScript>();
                List<Button> buttonList = new List<Button>();

                BindingFlags flags = 
                    BindingFlags.InvokeMethod | 
                    BindingFlags.Public |
                    BindingFlags.NonPublic | 
                    BindingFlags.Static | 
                    BindingFlags.Instance;

                for (int i = 0; i < m_Scripts.Length; i++)
                {
                    if (m_IsDisposed) return;

                    var script = m_Scripts[i];
                    var type = script.GetType();
                    var methods = type.GetMethods(flags);
                    buttonList.Clear();

                    for (int j = 0; j < methods.Length; j++)
                    {
                        if (m_IsDisposed) return;

                        MethodInfo method = methods[j];
                        var attributes = method.GetCustomAttributes(false);

                        for (int w = 0; w < attributes.Length; w++)
                        {
                            if (m_IsDisposed) return;

                            var attribute = attributes[w];
                            var attributeType = attribute.GetType(); ;
                            if (attributeType == typeof(SceneButtonAttribute))
                            {
                                var button = new Button(script, method, (SceneButtonAttribute)attribute);
                                buttonList.Add(button);
                            }
                        }
                    }

                    if (buttonList.Count > 0)
                    {
                        var target = new TargetScript(m_Scripts[i], buttonList.ToArray());
                        targetList.Add(target);
                    }
                }

                results = targetList.ToArray();
                isDone = true;
            }

            public void Dispose()
            {
                m_IsDisposed = true;
                m_Thread.Abort();
            }

            public static implicit operator bool(TargetsProvider obj)
            {
                if (obj == null) return false;
                return !obj.m_IsDisposed;
            }
        }
    }
}
