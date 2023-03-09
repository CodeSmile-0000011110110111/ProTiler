using UnityEngine;

namespace SBG.Toolbelt.DebugTools
{
    public class CircleGizmo : GizmoDisplay
    {
#if UNITY_EDITOR
        [Min(0)]
        [SerializeField] private float radius = 0.5f;
        [Range(3,100)]
        [SerializeField] private int segments = 30;


        protected override void DisplayGizmo()
        {
            ToolbeltGizmos.DrawCircle(position, gizmoRotation, radius, displayMode, color, segments);
        }
#endif
    }
}