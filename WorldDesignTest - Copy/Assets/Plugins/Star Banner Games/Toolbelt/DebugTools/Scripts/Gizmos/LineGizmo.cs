using UnityEngine;

namespace SBG.Toolbelt.DebugTools
{
    public class LineGizmo : GizmoDisplay
    {
#if UNITY_EDITOR
        [SerializeField] Transform targetTransform;
        [SerializeField] private Vector3 targetOffset = Vector3.zero;

        private Vector3 TargetPosition
        {
            get
            {
                if (targetTransform == null) targetTransform = transform;

                return targetTransform.position + targetOffset;
            }
        }

        protected override void DisplayGizmo()
        {
            ToolbeltGizmos.DrawLine(position, TargetPosition, color);
        }
#endif
    }
}