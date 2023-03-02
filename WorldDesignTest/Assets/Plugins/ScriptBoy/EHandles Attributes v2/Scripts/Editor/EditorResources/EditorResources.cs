using UnityEditor;
using UnityEngine;

namespace EHandles
{
    [FilePath(@"Assets\ScriptBoy\EHandles Attributes v2\Scripts\Editor\EditorResources\Resources.asset")]
    internal class EditorResources : ScriptableSingleton<EditorResources>
    {
        [SerializeField] private Texture2D m_EHIcon;
        public static Texture2D EHIcon => instance.m_EHIcon;
    }
}
