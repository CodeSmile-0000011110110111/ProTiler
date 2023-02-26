using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.XR;

namespace CodeSmile.Scripts
{
	[CustomEditor(typeof(ExampleScript))]
	public class ExampleEditor : Editor
	{
		public void OnSceneGUI()
		{
			var t = target as ExampleScript;
			var tr = t.transform;
			var pos = tr.position + Vector3.down;

			Handles.color = Color.magenta;
			Handles.DrawWireDisc(pos, tr.up, t.value);
			t.value = Handles.ScaleSlider(t.value, pos, Vector3.forward, Quaternion.identity, HandleUtility.GetHandleSize(pos), 1f);
			//t.value = Handles.RadiusHandle(Quaternion.identity, pos, t.value);
			//Handles.Disc(Quaternion.identity, pos, Vector3.up, t.value, false, 1f);
			//Handles.ScaleValueHandle(t.value, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos), (id, position, rotation, size, type) => {}, 1f);
			
			//Handles.DrawCamera(new Rect(pos, new Vector2(300,200)), Camera.main);
			//Handles.DrawSelectionFrame();

			GUI.color = Color.magenta;
			Handles.Label(pos, t.value.ToString("F1"));
		}
	}
}