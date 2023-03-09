using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SBG.Toolbelt.DebugTools.Editor
{
    [CustomEditor(typeof(Object), true, isFallback = true)]
    [CanEditMultipleObjects]
    internal class ObjectEditor : UnityEditor.Editor
	{
        private struct FoldoutGroup
        {
            public readonly string Name;
            public bool Foldout;
            public SerializedProperty[] Properties;

            public FoldoutGroup(string name, bool foldout, SerializedProperty[] properties)
            {
                Name = name;
                Foldout = foldout;
                Properties = properties;
            }
        }

        private SerializedProperty[] _ungroupedProperties;
        private FoldoutGroup[] _foldouts;

        private bool _buttonFoldout = true;
        private InspectorButton[] _buttons;

        private bool _isDefaultEditor = true;


        private void OnEnable()
        {
            GetAllFoldouts();
            GetAllButtons();

            if (_foldouts != null && _foldouts.Length > 0 ||
                _buttons != null && _buttons.Length > 0)
            {
                _isDefaultEditor = false;
            }
            else
            {
                _ungroupedProperties = null;
                _foldouts = null;
                _buttons = null;
            }
        }

        private void OnDestroy()
        {
            //Remove Keys when Object is deleted
            if (target == null)
            {
                DeleteFoldoutKey("ButtonFoldout");

                if (_foldouts != null)
                {
                    for (int i = 0; i < _foldouts.Length; i++)
                    {
                        DeleteFoldoutKey($"Foldout{i}");
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (_isDefaultEditor)
            {
                DrawDefaultInspector();
                return;
            }

            serializedObject.Update();
            DrawFoldouts();
            DrawUngroupedProperties();
            serializedObject.ApplyModifiedProperties();
            
            DrawButtons();
        }

        private void DrawFoldouts()
        {
            if (_foldouts == null || _foldouts.Length == 0) return;

            for (int i = 0; i < _foldouts.Length; i++)
            {
                bool foldout = EditorGUILayout.BeginFoldoutHeaderGroup(_foldouts[i].Foldout, _foldouts[i].Name);

                //Save Foldout Value if it changed
                if (_foldouts[i].Foldout != foldout)
                {
                    _foldouts[i].Foldout = foldout;
                    SetFoldoutKey($"Foldout{i}", foldout);
                }

                //Draw all properties in the foldout with an indent
                if (_foldouts[i].Foldout)
                {
                    EditorGUI.indentLevel++;
                    foreach (SerializedProperty property in _foldouts[i].Properties)
                    {
                        EditorGUILayout.PropertyField(property);
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        private void DrawUngroupedProperties()
        {
            if (_ungroupedProperties != null)
            {
                foreach (SerializedProperty property in _ungroupedProperties)
                {
                    EditorGUILayout.PropertyField(property);
                }
            }
        }

        private void DrawButtons()
        {
            if (_buttons == null || _buttons.Length == 0) return;

            EditorGUILayout.Separator();

            bool foldout = EditorGUILayout.BeginFoldoutHeaderGroup(_buttonFoldout, "Buttons");

            //Save Foldout Value if it changed
            if (foldout != _buttonFoldout)
            {
                _buttonFoldout = foldout;
                SetFoldoutKey("ButtonFoldout", foldout);
            }

            //Draw all Buttons
            if (_buttonFoldout)
            {
                foreach (var button in _buttons)
                {
                    if (GUILayout.Button(button.Name, GUILayout.Height(button.Height)))
                    {
                        button.Method.Invoke(target, null);
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void GetAllFoldouts()
        {
            //Get Fields
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] fields = target.GetType().GetFields(flags);

            //Create Temp Lists
            List<FoldoutGroup> groups = new List<FoldoutGroup>();
            List<SerializedProperty> ungroupedProps = new List<SerializedProperty>();

            FoldoutGroup unnamedGroup = new FoldoutGroup("Unnamed", true, new SerializedProperty[0]);
            int lastGroup = -1;

            //Add each Field to its Group
            foreach (FieldInfo field in fields)
            {
                FoldoutAttribute foldout = field.GetCustomAttribute<FoldoutAttribute>();

                if (foldout == null) //Regular Property
                {
                    SerializedProperty prop = serializedObject.FindProperty(field.Name);
                    if (prop != null) ungroupedProps.Add(prop); //Returns null if private and not serialized
                    continue;
                }

                bool groupFound = false;

                if (foldout.FoldoutGroup != string.Empty) //Foldout Group was specified
                {
                    for (int i = 0; i < groups.Count; i++)
                    {
                        //If the Group was found, add the property to it.
                        if (groups[i].Name == foldout.FoldoutGroup)
                        {
                            groups[i] = AddFieldToGroup(groups[i], field);
                            lastGroup = i;
                            groupFound = true;
                            break;
                        }
                    }
                }
                else //Use last Foldout Group
                {
                    if (lastGroup == -1) unnamedGroup = AddFieldToGroup(unnamedGroup, field);
                    else groups[lastGroup] = AddFieldToGroup(groups[lastGroup], field);

                    groupFound = true;
                }
                
                //If a group wasn't found, create it and add the property!
                if (!groupFound)
                {
                    bool foldoutValue = GetFoldoutKey(foldout.FoldoutGroup);
                    SerializedProperty[] newProps = new SerializedProperty[] { serializedObject.FindProperty(field.Name) };
                    groups.Add(new FoldoutGroup(foldout.FoldoutGroup, foldoutValue, newProps));
                    lastGroup = groups.Count-1;
                }
            }

            //Append the Unnamed Group at the end if it was filled
            if (unnamedGroup.Properties.Length > 0) groups.Add(unnamedGroup);

            //Apply Lists
            _ungroupedProperties = ungroupedProps.ToArray();
            _foldouts = groups.ToArray();

            //Load Foldout Values
            for (int i = 0; i < _foldouts.Length; i++)
            {
                _foldouts[i].Foldout = GetFoldoutKey($"Foldout{i}");
            }
        }

        private FoldoutGroup AddFieldToGroup(FoldoutGroup group, FieldInfo field)
        {
            List<SerializedProperty> props = group.Properties.ToList();
            props.Add(serializedObject.FindProperty(field.Name));

            return new FoldoutGroup(group.Name, group.Foldout, props.ToArray());
        }

        private void GetAllButtons()
        {
            //Get Methods
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo[] methods = target.GetType().GetMethods(flags);

            List<InspectorButton> newButtons = new List<InspectorButton>();

            //Look for Button Attributes in all Methods
            foreach (MethodInfo method in methods)
            {
                ButtonAttribute btn = method.GetCustomAttribute<ButtonAttribute>();

                if (btn != null)
                {
                    var parameters = method.GetParameters();

                    if (parameters.Length > 0)
                    {
                        Debug.LogError("[ButtonAttribute] does not support parameters");
                        continue;
                    }

                    string text = btn.Text;

                    if (string.IsNullOrEmpty(text)) text = method.Name;
                    newButtons.Add(new InspectorButton(text, btn.Height, method));
                }
            }

            //Load Foldout Value
            if (newButtons.Count > 0)
            {
                _buttonFoldout = GetFoldoutKey("ButtonFoldout");
            }

            _buttons = newButtons.ToArray();
        }

        private bool GetFoldoutKey(string foldoutName)
        {
            return EditorPrefs.GetBool($"{target.GetInstanceID()}_{foldoutName}", true);
        }

        private void SetFoldoutKey(string foldoutName, bool value)
        {
            EditorPrefs.SetBool($"{target.GetInstanceID()}_{foldoutName}", value);
        }

        private void DeleteFoldoutKey(string foldoutName)
        {
            EditorPrefs.DeleteKey($"{target.GetInstanceID()}_{foldoutName}");
        }
    }
}