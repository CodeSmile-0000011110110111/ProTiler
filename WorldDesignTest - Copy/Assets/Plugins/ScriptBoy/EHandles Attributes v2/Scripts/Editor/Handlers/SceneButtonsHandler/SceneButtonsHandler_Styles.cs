using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;


namespace EHandles
{
    internal partial class SceneButtonsHandler
    {
        private static class Styles
        {
            public static GUIStyle buttonStyle = GUI.skin.button;
            public static GUIContent buttonContent = new GUIContent();
        }
    }
}