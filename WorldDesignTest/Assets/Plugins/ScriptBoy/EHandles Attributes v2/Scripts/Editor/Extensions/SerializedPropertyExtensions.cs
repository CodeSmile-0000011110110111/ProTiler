using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EHandles
{
    internal static class SerializedPropertyExtensions
    {
        public static int GetID(this SerializedProperty property)
        {
            return property.serializedObject.targetObject.GetInstanceID() ^ property.propertyPath.GetHashCode();
        }

        public static bool IsArrayElement(this SerializedProperty property)
        {
            return property.propertyPath.EndsWith("]");//xxx.Array.data[n]
        }

        public static int GetArrayElementIndex(this SerializedProperty property)
        {
            var path = property.propertyPath;//xxx.Array.data[n]
            int s = path.LastIndexOf('[');
            string n = path.Substring(s + 1, path.Length - s - 2);
            return int.Parse(n);
        }

        public static SerializedProperty GetNeighborProperty(this SerializedProperty property, string neighborName)
        {
            //a
            //a.Array.data[n]
            //a.b...n
            //a.b...n.Array.data[n]
            //a.b...n.Array.data[n].v

            string path = property.propertyPath;

            if (path.Contains("."))
            {
                if (path.EndsWith("]"))
                {
                    int cut = 0;
                    for (int i = path.Length - 1, dot = 0; i > -1; i--)
                    {
                        if (path[i] == '.')
                        {
                            dot++;
                        }

                        if (dot == 2)
                        {
                            cut = i;
                        }

                        if (dot == 3)
                        {
                            cut = i + 1;
                            break;
                        }
                    }

                    path = path.Substring(0, cut) + neighborName;
                    return property.serializedObject.FindProperty(path);
                }
                else
                {
                    for (int i = path.Length - 1; i > -1; i--)
                    {
                        if (path[i] == '.')
                        {
                            path = path.Substring(0, i + 1) + neighborName;
                            return property.serializedObject.FindProperty(path);
                        }
                    }
                }
            }

            return property.serializedObject.FindProperty(neighborName);
        }

        public static void Duplicate(this SerializedProperty property)
        {
            var element = property;
            if (element.TryGetParentProperty(out var parent))
            {
                if (!parent.isArray && parent.IsArrayElement())
                {
                    element = parent;
                    parent = parent.GetParentProperty();
                }

                if (parent.isArray)
                {
                    EditorApplication.delayCall += () =>
                    {
                        if (element.propertyType == SerializedPropertyType.ObjectReference && element == property)
                        {
                            var obj = element.objectReferenceValue;
                            if (obj != null)
                            {
                                element.DuplicateCommand();
                                obj = UnityEngine.Object.Instantiate(obj);
                                obj.name = obj.name.Replace("(Clone)","");
                                element.objectReferenceValue = obj;
                            }
                        }
                        else
                        {
                            element.DuplicateCommand();
                        }

                        parent.serializedObject.ApplyModifiedProperties();
                    };

                    return;
                }
            }

            Debug.LogWarning($"The '{property.propertyPath}' can't be duplicated.\nIt should be an array element or a property of an array element.");
        }

        public static void Delete(this SerializedProperty property)
        {
            var element = property;
            if (element.TryGetParentProperty(out var parent))
            {
                if (!parent.isArray && parent.IsArrayElement())
                {
                    element = parent;
                    parent = parent.GetParentProperty();
                }

                if (parent.isArray)
                {
                    EditorApplication.delayCall += () =>
                    {
                        if (element.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            var obj = element.objectReferenceValue;
                            if (obj != null)
                            {
                                if (obj is Component) obj = (obj as Component).gameObject;
                                UnityEngine.Object.DestroyImmediate(obj);
                            }
                        }

                        element.DeleteCommand();
                        parent.serializedObject.ApplyModifiedProperties();
                    };

                    return;
                }
            }

            Debug.LogWarning($"The '{property.propertyPath}' can't be deleted.\nIt should be an array element or a property of an array element.");
        }

        public static bool TryGetParentProperty(this SerializedProperty property,out SerializedProperty parent)
        {
            string path = property.propertyPath;

            if (!path.Contains("."))
            {
                parent = null;
                return false;
            }

            if (path[path.Length - 1] == ']')
            {
                //xxx.parent.Array.data[n] => xxx.parent
                path = path.Remove(path.LastIndexOf(".A"));
                parent = property.serializedObject.FindProperty(path);
            }
            else
            {
                //xxx.parent.v => xxx.parent
                path = path.Remove(path.LastIndexOf("."));
                parent = property.serializedObject.FindProperty(path);
            }

            return true;
        }

        public static SerializedProperty GetParentProperty(this SerializedProperty property)
        {
            string path = property.propertyPath;

            if (path[path.Length - 1] == ']')
            {
                //xxx.parent.Array.data[n] => xxx.parent
                path = path.Remove(path.LastIndexOf(".A"));
                return property.serializedObject.FindProperty(path);
            }

            //xxx.parent.v => xxx.parent
            path = path.Remove(path.LastIndexOf("."));
            return property.serializedObject.FindProperty(path);
        }

        public static FieldInfo GetFieldInfo(this SerializedProperty property)
        {
            string path = property.propertyPath;

            if (path.EndsWith("]")) return null;

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            Type type = property.serializedObject.targetObject.GetType();
            if (!path.Contains(".")) return type.GetField(path, bindingFlags);

            FieldInfo field = null;
            string[] paths = path.Split('.');
            for (int i = 0; i < paths.Length; i++)
            {
                if (type.IsArray)
                {
                    type = type.GetElementType();
                    field = null;
                    i++;//skip "data[n] or size" (xxx.Array.data[n].yyy) (xxx.Array.size)
                    continue;
                }

                if (type.IsGenericType)
                {
                    type = type.GetGenericArguments()[0];
                    field = null;
                    i++;
                    continue;
                }

                field = type.GetField(paths[i], bindingFlags);
                if (field == null) return null;
                type = field.FieldType;
            }

            return field;
        }

        public static bool TryGetNumber(this SerializedProperty property, ref float f)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float: { f = property.floatValue; return true; }
                case SerializedPropertyType.Integer: { f = property.intValue; return true; }
            }
            return false;
        }

        public static bool TryGetTransform(this SerializedProperty property, out Transform t)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue != null)
                {
                    if (property.type == "PPtr<$Transform>")
                    {
                        t = (Transform)property.objectReferenceValue;
                        return true;
                    }
                    else if (property.type == "PPtr<$GameObject>")
                    {
                        t = ((GameObject)property.objectReferenceValue).transform;
                        return true;
                    }
                }
            }

            t = null;
            return false;
        }
    }
}