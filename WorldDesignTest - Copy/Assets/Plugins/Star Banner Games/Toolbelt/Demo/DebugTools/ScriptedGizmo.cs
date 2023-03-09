using UnityEngine;
using SBG.Toolbelt.DebugTools;

namespace SBG.Toolbelt.Demo
{
	public class ScriptedGizmo : MonoBehaviour
	{
		[GizmoColor] private Color _color = new Color(1, 1, 1, 0.25f);
		[GizmoRotationOffset] private Vector3 _rotationOffset = Vector3.zero;

        [Button("Reset Values")]
        private void ResetValues()
        {
			_color = new Color(1, 1, 1, 0.25f);
			_rotationOffset = Vector3.zero;
		}

        [Button("Randomize Color")]
		private void RandomizeColor()
        {
			float hue = Random.Range(0f, 1f);

			_color = Color.HSVToRGB(hue, 1, 1);
			_color.a = 0.25f;
        }

		[Button("Randomize Rotation Offset")]
		private void RandomizeRotation()
		{
			_rotationOffset = new Vector3(Random.Range(0f, 360f),
										  Random.Range(0f, 360f),
										  Random.Range(0f, 360f));
		}
	}
}