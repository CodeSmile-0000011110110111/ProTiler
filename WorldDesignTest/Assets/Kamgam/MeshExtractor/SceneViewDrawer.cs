using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
#endif

namespace Kamgam.MeshExtractor
{
#if !UNITY_EDITOR
    public class SceneViewDrawer : MonoBehaviour {}
#else

    /// <summary>
    /// Used to enable GL.* drawing method in BuiltIn and URP/HDRP
    /// render pipelines.
    /// </summary>
    [ExecuteInEditMode]
    public class SceneViewDrawer : MonoBehaviour
    {
        static SceneViewDrawer _instance;

        const string Name = "MeshExtractor.SceneViewDrawer";

        public static SceneViewDrawer Instance()
        {
            if (_instance != null && !_instance.Destroyed)
            {
                return _instance;
            }

            var go = new GameObject(Name);
            go.hideFlags = HideFlags.HideAndDontSave;
            var drawer = go.AddComponent<SceneViewDrawer>();
            _instance = drawer;

            CompilationPipeline.compilationStarted -= onCompilationStarted;
            CompilationPipeline.compilationStarted += onCompilationStarted;

            return drawer;
        }

        private static void onCompilationStarted(object obj)
        {
            if (_instance != null)
            {
                _instance.Kill();
                _instance = null;
            }
        }


        public bool Destroyed = false;

        public void Kill()
        {
            Destroyed = true;
            GameObject.DestroyImmediate(this.gameObject);
        }

        public event System.Action OnRender;

        void OnRenderObject()
        {
            if (Destroyed)
                return;

            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            OnRender?.Invoke();
        }
    }
}
#endif

