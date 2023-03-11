using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

namespace Kamgam.MeshExtractor
{
    partial class MeshExtractorTool
    {
        // Cursor position:
        // We cache one position per selection. This way the user does not have to
        // reposition the cursor every time the selection changes.
        protected Dictionary<string, Vector3> _cursorPositionCache = new Dictionary<string, Vector3>();
        protected StringBuilder _cursorCacheKeyBuilder = new StringBuilder();
        protected string _cursorPositionCacheKey;
        protected bool _pivotModified = false;

        /// <summary>
        /// Global cursor position.
        /// </summary>
        protected Vector3 _cursorPosition
        {
            get
            {
                initCursorCacheKeyIfneeded();
                var pos = getCursorPositionFromCache(_cursorPositionCacheKey);
                if (pos.HasValue)
                {
                    return pos.Value;
                }
                else
                {
                    return calculateDefaultHandlePivotPositionInWorld();
                }
            }

            set
            {
                initCursorCacheKeyIfneeded();
                setCursorPositionInCache(_cursorPositionCacheKey, value);
            }
        }

        /// <summary>
        /// Global cursor rotation.
        /// </summary>
        protected Quaternion _cursorRotation = Quaternion.identity;

        public void ClearCursorCache()
        {
            _cursorPositionCache.Clear();
        }

        protected void initCursorCacheKeyIfneeded()
        {
            if (_cursorPositionCacheKey == null)
            {
                _cursorPositionCacheKey = getCursorCacheKey(Selection.gameObjects);
            }
        }

        public void CenterPivot()
        {
            _cursorPosition = calculateDefaultHandlePivotPositionInWorld();
        }

        protected string getCursorCacheKey(IEnumerable<GameObject> gameObjects)
        {
            _cursorCacheKeyBuilder.Clear();
            foreach (var go in gameObjects)
            {
                _cursorCacheKeyBuilder.Append(go.GetInstanceID());
            }

            // In case we want to cache different cursor positions for mode/rotations.
            // objectsKeyBuilder.Append(Tools.pivotMode == PivotMode.Center ? "c" : "p");
            // objectsKeyBuilder.Append(Tools.pivotRotation == PivotRotation.Local ? "l" : "g");

            // avoid long keys by hashing if necessary
            if (_cursorCacheKeyBuilder.Length <= 40)
                return _cursorCacheKeyBuilder.ToString();
            else
                return UtilsHash.SHA1(_cursorCacheKeyBuilder.ToString());
        }

        protected Vector3? getCursorPositionFromCache(string key)
        {
            if (_cursorPositionCache.ContainsKey(key))
                return _cursorPositionCache[key];
            else
                return null;
        }

        protected void setCursorPositionInCache(string key, Vector3 position)
        {
            // store in cache
            if (_cursorPositionCache.ContainsKey(key))
                _cursorPositionCache[key] = position;
            else
                _cursorPositionCache.Add(key, position);
        }

        protected void removeCursorPositionInCache(string key)
        {
            if (_cursorPositionCache.ContainsKey(key))
                _cursorPositionCache.Remove(key);
        }

        /// <summary>
        /// Mirrors the default beviour of Unity pivots.
        /// </summary>
        /// <returns></returns>
        Vector3 calculateDefaultHandlePivotPositionInWorld()
        {
            Vector3 result = Vector3.zero;

            var gameObjects = Selection.gameObjects;
            if (gameObjects.Length >= 1)
            {
                // center of all selected tris
                Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

                foreach (var tri in _selectedTriangles)
                {
                    tri.UpdateWorldPos();

                    min.x = Mathf.Min(tri.VertexGlobal0.x, min.x);
                    min.y = Mathf.Min(tri.VertexGlobal0.y, min.y);
                    min.z = Mathf.Min(tri.VertexGlobal0.z, min.z);

                    min.x = Mathf.Min(tri.VertexGlobal1.x, min.x);
                    min.y = Mathf.Min(tri.VertexGlobal1.y, min.y);
                    min.z = Mathf.Min(tri.VertexGlobal1.z, min.z);

                    min.x = Mathf.Min(tri.VertexGlobal2.x, min.x);
                    min.y = Mathf.Min(tri.VertexGlobal2.y, min.y);
                    min.z = Mathf.Min(tri.VertexGlobal2.z, min.z);

                    max.x = Mathf.Max(tri.VertexGlobal0.x, max.x);
                    max.y = Mathf.Max(tri.VertexGlobal0.y, max.y);
                    max.z = Mathf.Max(tri.VertexGlobal0.z, max.z);

                    max.x = Mathf.Max(tri.VertexGlobal1.x, max.x);
                    max.y = Mathf.Max(tri.VertexGlobal1.y, max.y);
                    max.z = Mathf.Max(tri.VertexGlobal1.z, max.z);
                    
                    max.x = Mathf.Max(tri.VertexGlobal2.x, max.x);
                    max.y = Mathf.Max(tri.VertexGlobal2.y, max.y);
                    max.z = Mathf.Max(tri.VertexGlobal2.z, max.z);
                }

                result = min + (max - min) * 0.5f;
            }

            return result;
        }

        void onPivotCursorGUI(SceneView sceneView, bool snap)
        {
            if (_selectionChanged && _mouseIsInSceneView)
            {
                _cursorPositionCacheKey = null; 
            }

            // snap
            if (snap)
            {
                if (_selectedTriangles.Count > 0 && SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
                {
                    // Make sure the cached world space is in sync.
                    foreach (var tri in _selectedTriangles)
                    {
                        tri.UpdateWorldPos();
                    }

                    var mousePos = Event.current.mousePosition;
                    var ray = HandleUtility.GUIPointToWorldRay(mousePos);
                    var mousePosWorld = ray.origin + ray.direction * 10f;

                    Vector3 minPoint = Vector3.zero;
                    float minDistance = float.MaxValue;
                    foreach(var tri in _selectedTriangles)
                    {
                        snapFindClosestPoint(mousePosWorld, ref minPoint, ref minDistance, tri.VertexGlobal0);
                        snapFindClosestPoint(mousePosWorld, ref minPoint, ref minDistance, tri.VertexGlobal1);
                        snapFindClosestPoint(mousePosWorld, ref minPoint, ref minDistance, tri.VertexGlobal2);
                    }
                    _cursorPosition = minPoint;
                }
            }

            // cursor pos handle
            var newCursorPosition = Handles.PositionHandle(_cursorPosition, _cursorRotation) ;
            if (Vector3.SqrMagnitude(newCursorPosition - _cursorPosition) > 0.000001f)
            {
                _cursorPosition = newCursorPosition;
                _pivotModified = true;
            }
        }

        private static void snapFindClosestPoint(Vector3 mousePosWorld, ref Vector3 minPoint, ref float minDistance, Vector3 vertexWorld)
        {
            var screenPointMouse = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(mousePosWorld);
            var screenPoint = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(vertexWorld);
            
            var dx = screenPointMouse.x - screenPoint.x;
            var dy = screenPointMouse.y - screenPoint.y;
            float sqrDistance = (dx * dx) + (dy * dy);
            if (sqrDistance < minDistance)
            {
                minDistance = sqrDistance;
                minPoint = vertexWorld;
            }
        }
    }
}
