using UnityEditor;

namespace SBG.Toolbelt.Editor
{
	[CustomEditor(typeof(Trigger))]
	public class TriggerInspector : UnityEditor.Editor
	{
		private SerializedProperty _filterByTag;
		private SerializedProperty _tagFilters;
		private SerializedProperty _filterByLayer;
		private SerializedProperty _layerMask;

		private SerializedProperty _onTriggerEnterEvent;
		private SerializedProperty _onTriggerStayEvent;
		private SerializedProperty _onTriggerExitEvent;
								   
		private SerializedProperty _onCollisionEnterEvent;
		private SerializedProperty _onCollisionStayEvent;
		private SerializedProperty _onCollisionExitEvent;

		private bool _filterDropdown = false;
		private bool _triggerDropdown = false;
		private bool _collisionDropdown = false;


		protected virtual void OnEnable()
        {
			_filterByTag =		serializedObject.FindProperty("filterByTag");
			_tagFilters =		serializedObject.FindProperty("tagFilters");
			_filterByLayer =	serializedObject.FindProperty("filterByLayer");
			_layerMask =		serializedObject.FindProperty("layerMask");

			_onTriggerEnterEvent =	serializedObject.FindProperty("OnTriggerEnterEvent");
			_onTriggerStayEvent =	serializedObject.FindProperty("OnTriggerStayEvent");
			_onTriggerExitEvent =	serializedObject.FindProperty("OnTriggerExitEvent");

			_onCollisionEnterEvent =	serializedObject.FindProperty("OnCollisionEnterEvent");
			_onCollisionStayEvent =		serializedObject.FindProperty("OnCollisionStayEvent");
			_onCollisionExitEvent =		serializedObject.FindProperty("OnCollisionExitEvent");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			//FILTERS
			_filterDropdown = EditorGUILayout.Foldout(_filterDropdown, "Filters", EditorStyles.foldoutHeader);
			if (_filterDropdown)
            {
				EditorGUILayout.Space(7);
				EditorGUI.indentLevel++;

				//FILTER BY TAG
				EditorGUILayout.PropertyField(_filterByTag);
				if (_filterByTag.boolValue)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(_tagFilters);
					EditorGUI.indentLevel--;
				}

				//FILTER BY LAYER
				EditorGUILayout.PropertyField(_filterByLayer);
				if (_filterByLayer.boolValue)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(_layerMask);
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Separator();

			//TRIGGER EVENTS
			_triggerDropdown = EditorGUILayout.Foldout(_triggerDropdown, "Trigger Events", EditorStyles.foldoutHeader);
			if (_triggerDropdown)
			{
				EditorGUILayout.Space(7);
				EditorGUI.indentLevel++;

				EditorGUILayout.PropertyField(_onTriggerEnterEvent);
				EditorGUILayout.PropertyField(_onTriggerStayEvent);
				EditorGUILayout.PropertyField(_onTriggerExitEvent);

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Separator();

			//COLLISION EVENTS
			_collisionDropdown = EditorGUILayout.Foldout(_collisionDropdown, "Collision Events", EditorStyles.foldoutHeader);
			if (_collisionDropdown)
			{
				EditorGUILayout.Space(7);
				EditorGUI.indentLevel++;

				EditorGUILayout.PropertyField(_onCollisionEnterEvent);
				EditorGUILayout.PropertyField(_onCollisionStayEvent);
				EditorGUILayout.PropertyField(_onCollisionExitEvent);

				EditorGUI.indentLevel--;
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}