using UnityEngine;
using UnityEditor;

namespace SBG.Toolbelt.Editor
{
	[CustomEditor(typeof(Trigger2D))]
	public class Trigger2DInspector : TriggerInspector
	{
		private GameObject triggerObject;

        private bool _hasRb2D;


		protected override void OnEnable()
		{
			base.OnEnable();

			triggerObject = (target as Trigger).gameObject;
            _hasRb2D = triggerObject.GetComponent<Rigidbody2D>() != null;
        }

        public override void OnInspectorGUI()
		{
			if (!_hasRb2D)
            {
                EditorGUILayout.HelpBox("This Trigger does not have a Rigidbody2D assigned. It will only work when an object with an active Rigidbody collides with it.", MessageType.Info);
            }

			base.OnInspectorGUI();
		}
	}
}