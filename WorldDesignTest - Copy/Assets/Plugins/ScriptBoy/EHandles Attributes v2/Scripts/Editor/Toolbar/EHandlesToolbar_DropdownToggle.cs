#if SUPPORTS_SCENE_VIEW_OVERLAYS
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine.UIElements;
using UnityEditor.Overlays;

namespace EHandles
{
    internal partial class EHandlesToolbar
    {
        [EditorToolbarElement(id, typeof(SceneView))]
        private class DropdownToggle : EditorToolbarDropdownToggle
        {
            public const string id = "EHandles/DropdownToggle";
            public static DropdownToggle instance;

            public DropdownToggle()
            {
                instance = this;

                this.icon = EditorResources.EHIcon;
                this.value = EHandlesSettings.enabled;
                this.tooltip = "Toggle visibility of EHandles in the Scene view";
                this.RegisterValueChangedCallback(OnValueChanged);
                this.dropdownClicked += OnDropdownClicked;
            }

            private void OnDropdownClicked()
            {
                OptionsWindow.ShowAtPosition(this.worldBound);
            }

            private void OnValueChanged(ChangeEvent<bool> evt)
            {
                EHandlesSettings.enabled = evt.newValue;
            }
        }

        public static void Refresh()
        {
            var toggle = DropdownToggle.instance;
            if (toggle == null) return;
            toggle.value = EHandlesSettings.enabled;
        }
    }
}
#endif