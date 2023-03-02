using UnityEngine;

namespace SBG.Toolbelt.DebugTools
{
	public class RectGizmo : GizmoDisplay
	{
#if UNITY_EDITOR
        [SerializeField] private Vector2 size = Vector2.one;

        protected override void DisplayGizmo()
        {
            ToolbeltGizmos.DrawRect(position, gizmoRotation, size, displayMode, color);
        }
#endif
    }
}