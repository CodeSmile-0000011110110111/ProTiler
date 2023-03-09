using UnityEngine;

namespace SBG.Toolbelt.DebugTools
{
    public class SphereGizmo : GizmoDisplay
    {
#if UNITY_EDITOR
        [Min(0)]
        [SerializeField] private float radius = 0.5f;

        protected override void DisplayGizmo()
        {
            ToolbeltGizmos.DrawSphere(position, radius, displayMode, color);
        }
#endif
    }
}