#if SUPPORTS_SCENE_VIEW_OVERLAYS
using UnityEditor;
using UnityEditor.Overlays;

namespace EHandles
{
    [Overlay(typeof(SceneView), "EHandles")]
    internal partial class EHandlesToolbar : ToolbarOverlay
    {
        EHandlesToolbar() : base(DropdownToggle.id)
        {
            this.displayName = "EH";
            this.collapsedChanged += OnCollapsedChanged;
        }

        private void OnCollapsedChanged(bool value)
        {
            if (value) this.collapsed = false;
        }
    }
}
#endif