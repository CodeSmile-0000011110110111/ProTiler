using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandlesTest))]
public class HandlesTestEditor : Editor
{
	protected virtual void OnSceneGUI()
	{
		HandlesTest testObject = (HandlesTest)target;

		var pos = testObject.transform.position;
		var size = HandleUtility.GetHandleSize(pos) * 15f;
		//var snap = 1f;

		EditorGUI.BeginChangeCheck();
		//var newAmount = Handles.ScaleValueHandle(testObject.TestFloat, pos, Quaternion.identity, size, Handles.ArrowHandleCap, snap);
		var newPos = Handles.PositionHandle(pos + Vector3.up, Quaternion.identity) - pos;
		if (newPos + pos != pos)
		Debug.Log(newPos);
		
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(testObject, "Change float value");
			//testObject.TestFloat = newAmount;
		}
	}
}