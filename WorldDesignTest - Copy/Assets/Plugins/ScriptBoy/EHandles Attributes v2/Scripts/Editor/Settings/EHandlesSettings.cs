using UnityEditor;
using UnityEngine;

namespace EHandles
{
    [FilePath(@"Assets\ScriptBoy\EHandles Attributes v2\Scripts\Editor\Settings\Settings.asset")]
    public class EHandlesSettings : ScriptableSingleton<EHandlesSettings>
    {
        [MenuItem("Tools/ScriptBoy/EHandles/Settings",priority = 0)]
        private static void Select()
        {
            Selection.activeObject = instance;
        }

        public static bool IsValidProperty(SerializedProperty property)
        {
            if (!instance.m_CheckPropertyType) return true;

            var propertyType = property.propertyType;

            foreach (var type in instance.m_AllowedPropertyTypes)
            {
                if (propertyType == type) return true;
            }

            return false;
        }


        public static bool enabled
        {
            get => instance.m_IsEnabled;
            set
            {
                instance.m_IsEnabled = value;
                instance.OnValidate();
            }
        }

        public static bool showHandles => instance.m_IsEnabled && instance.m_ShowHandles;
        public static bool showSceneButtons => instance.m_IsEnabled && instance.m_ShowSceneButtons;
        public static bool optimizeSceneButtons => instance.m_OptimizeSceneButtons;

        private void OnValidate()
        {
#if UNITY_2021_2_OR_NEWER
            EHandlesToolbar.Refresh();
#endif
        }

        [SerializeField] private bool m_IsEnabled = true;
        [SerializeField] private bool m_ShowHandles = true;
        [SerializeField] private bool m_ShowSceneButtons = true;
        [SerializeField] private bool m_OptimizeSceneButtons = false;

        [SerializeField] private bool m_CheckPropertyType = true;

        [SerializeField]
        private SerializedPropertyType[] m_AllowedPropertyTypes =
        {
            SerializedPropertyType.Vector3,
            SerializedPropertyType.Vector2,
            SerializedPropertyType.Vector3Int,
            SerializedPropertyType.Vector2Int,
            SerializedPropertyType.ObjectReference,
            SerializedPropertyType.Generic
        };
    }
}