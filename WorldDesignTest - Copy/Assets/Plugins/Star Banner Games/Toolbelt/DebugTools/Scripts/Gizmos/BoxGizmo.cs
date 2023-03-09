using UnityEngine;

namespace SBG.Toolbelt.DebugTools
{
    public class BoxGizmo : GizmoDisplay
    {
#if UNITY_EDITOR
        [SerializeField] protected Vector3 size = Vector3.one;

        protected override void DisplayGizmo()
        {
            ToolbeltGizmos.DrawBox(position, gizmoRotation, size, displayMode, color);
        }
#endif
    }
}