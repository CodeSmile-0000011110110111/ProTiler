using UnityEngine;
using UnityEditor;

namespace SBG.Toolbelt.DebugTools.Editor
{
	[CustomEditor(typeof(HelpBox))]
	public class HelpBoxInspector : UnityEditor.Editor
	{
		private HelpBox _helpBox;
		private bool _editing = false;

		private Vector2 _scrollPos = Vector2.zero;
		private GUIStyle _boxStyle;


        private void OnEnable()
        {
			_helpBox = target as HelpBox;
		}

		private void BuildStyles()
        {
			_boxStyle = new GUIStyle(EditorStyles.helpBox);
			_boxStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
			_boxStyle.fontSize = 12;
		}

		public override void OnInspectorGUI()
		{
			if (_boxStyle == null) BuildStyles();

			EditorGUILayout.BeginHorizontal(_boxStyle);
			EditorGUILayout.LabelField(_helpBox.gameObject.name, EditorStyles.boldLabel);

			string buttonName = _editing ? "Apply" : "Edit";

			if (GUILayout.Button(buttonName))
			{
				_editing = !_editing;

				if (_editing == false)
                {
					EditorUtility.SetDirty(_helpBox);
                }
			}

			EditorGUILayout.EndHorizontal();

			if (!_editing)
            {
				string helpText = _helpBox.HelpText;

				if (string.IsNullOrEmpty(helpText))
                {
					helpText = "-";
                }

				_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MaxHeight(120));

				EditorGUILayout.LabelField(helpText, _boxStyle);

				EditorGUILayout.EndScrollView();
			}
			else
            {
				EditorStyles.textField.wordWrap = true;

				Undo.RecordObject(_helpBox, "Editing HelpBox");
				_helpBox.HelpText = EditorGUILayout.TextArea(_helpBox.HelpText, GUILayout.MinHeight(120));
            }
		}
	}
}