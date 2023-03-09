using UnityEngine;
using UnityEditor;

namespace SBG.Toolbelt.Editor
{
	[CustomEditor(typeof(Trigger3D))]
	public class Trigger3DInspector : TriggerInspector
	{
		private GameObject triggerObject;

        private bool _hasRb;


        protected override void OnEnable()
        {
			base.OnEnable();

			triggerObject = (target as Trigger).gameObject;
            _hasRb = triggerObject.GetComponent<Rigidbody>() != null;
        }

        public override void OnInspectorGUI()
		{
			if (!_hasRb)
            {
                EditorGUILayout.HelpBox("This Trigger does not have a Rigidbody assigned. It will only work when an object with an active Rigidbody collides with it.", MessageType.Info);
            }

			base.OnInspectorGUI();
		}
	}
}