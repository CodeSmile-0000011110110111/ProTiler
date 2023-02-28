using UnityEngine;

[ExecuteInEditMode]
public class AutoDisablePickability : MonoBehaviour
{
#if UNITY_EDITOR
	private void OnEnable() => UnityEditor.SceneVisibilityManager.instance.DisablePicking(gameObject, true);
#endif
}
