using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal partial class AttributesHandler
    {
        private class TargetsProvider
        {
            public void FindTargets(ref TargetScript[] targets , MonoBehaviour[] scripts)
            {
                List<TargetScript> targetList = new List<TargetScript>();

                for (int i = 0; i < scripts.Length; i++)
                {
                    var script = scripts[i];
                    var serializedObject = new SerializedObject(script);
                    var property = serializedObject.GetIterator();

                    while (property.NextVisible(true))
                    {
                        if (EHandlesSettings.IsValidProperty(property))
                        {
                            var field = property.GetFieldInfo();
                            if (field == null)
                            {
                                continue;
                            }

                            if (field.GetCustomAttributes(typeof(Attribute), false).Length > 0)
                            {
                                var target = new TargetScript(script, serializedObject);
                                targetList.Add(target);
                                break;
                            }
                        }
                    }
                }

                targets = targetList.ToArray();
            }
        }
    }
}
