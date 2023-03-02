using UnityEngine;

namespace SBG.Toolbelt.DebugTools
{
	public abstract class GizmoDisplay : MonoBehaviour
	{
#if UNITY_EDITOR

        [SerializeField] protected bool alwaysDisplay = false;
        [SerializeField] protected ToolbeltGizmos.WireframeMode displayMode = ToolbeltGizmos.WireframeMode.Wireframe;

        [SerializeField] protected Color color = new Color(1,1,1,0.5f);
        [SerializeField] protected Transform origin;
        [SerializeField] protected Vector3 offset = Vector3.zero;

        [SerializeField] protected bool applyTransformRotation = false;
        [SerializeField] protected Vector3 rotationOffset = Vector3.zero;

        protected Vector3 position
        {
            get
            {
                if (origin == null) origin = transform;

                return origin.position + offset;
            }
        }

        protected Quaternion gizmoRotation
        {
            get
            {
                Quaternion rotation = applyTransformRotation ? transform.rotation : Quaternion.identity;
                return THelper.AddRotations(rotation, rotationOffset);
            }
        }

        protected abstract void DisplayGizmo();

        private void OnDrawGizmos()
        {
            if (alwaysDisplay) DisplayGizmo();
        }

        private void OnDrawGizmosSelected()
        {
            if (!alwaysDisplay) DisplayGizmo();
        }
#endif
    }
}