using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Kamgam.MeshExtractor
{
    /// <summary>
    /// Converts 3D triangles to 2D triangles viewed from the current Editor camera position (viewport).
    /// By doing that we can frontload many of the ray cast calculations and cache the results.
    /// <br /><br />
    /// Backface culling can be done.<br />
    /// Occlusion culling is a problem for objects with many triangles.<br />
    /// </summary>
    public static class TriangleCache
    {
        static Dictionary<Component, Mesh> _cachedMeshes = new Dictionary<Component, Mesh>();
        static Dictionary<Component, List<Vector3>> _cachedVerticesInWorldSpaceCache = new Dictionary<Component, List<Vector3>>();
        static Dictionary<Component, List<Vector3>> _cachedVerticesInViewport = new Dictionary<Component, List<Vector3>>();
        static Dictionary<Component, Dictionary<int, List<int>>> _cachedTrianglesCache = new Dictionary<Component, Dictionary<int, List<int>>>();

        // One entry per triangle
        // true = the triangle is facing front, false = back.
        static Dictionary<Component, Dictionary<int, List<bool>>> _cachedTrisFrontFacing = new Dictionary<Component, Dictionary<int, List<bool>>>();

        // true = the triangle center is inside the camera frustum, false = it's outside.
        static Dictionary<Component, Dictionary<int, List<bool>>> _cachedTrisInFrustum = new Dictionary<Component, Dictionary<int, List<bool>>>();

        // One entry per triangle
        static Dictionary<Component, Dictionary<int, List<Vector3>>> _cachedTrisCenterInViewport = new Dictionary<Component, Dictionary<int, List<Vector3>>>();

        // One entry per triangle
        // The max distance from the center of the triangle to the vertices (max(center-a, center-b, center-c)).
        // We use this as a 2d bouding circle test to exclude triangles from detail checks quickly.
        static Dictionary<Component, Dictionary<int, List<float>>> _cachedTrisCenterMaxDistanceInViewport = new Dictionary<Component, Dictionary<int, List<float>>>();

        // Baked meshes are cached until the cache is cleared manually.
        // This is important so we return the SAME baked mesh for all selected triangles.
        // We use this to bake each Skinned Mesh only once. Otherwise it would bake multiple times and return a new mesh every time CacheTriangles() was called.
        static Dictionary<Component, Mesh> _bakedMeshesCache = new Dictionary<Component, Mesh>();

        public static void RebuildBakedMeshesCache(IList<GameObject> objects)
        {
            _bakedMeshesCache.Clear();

            SkinnedMeshRenderer[] skinnedMeshRenderers;
            getMeshObjects(objects, out skinnedMeshRenderers, out _);

            if (skinnedMeshRenderers != null)
            {
                foreach (var meshRenderer in skinnedMeshRenderers)
                {
                    if (!isLayerInLayers(meshRenderer.gameObject.layer, Tools.visibleLayers))
                        continue;

                    if (meshRenderer.sharedMesh == null)
                        continue;

                    bakeMeshIfNeeded(meshRenderer);
                }
            }
        }

        private static void bakeMeshIfNeeded(SkinnedMeshRenderer meshRenderer)
        {
            if (!_bakedMeshesCache.ContainsKey(meshRenderer) || _bakedMeshesCache[meshRenderer] == null)
            {
                // bake vertices
                Mesh bakedMesh = new Mesh();
                meshRenderer.BakeMesh(bakedMesh, useScale: true);
                bakedMesh.RecalculateBounds();

                _bakedMeshesCache.Add(meshRenderer, bakedMesh);
            }
        }

        [MenuItem("Tools/Mesh Extractor/Debug/Cache Triangles", priority = 301)]
        public static void CacheTriangles()
        {
            CacheTriangles(SceneView.lastActiveSceneView.camera, null);
        }

        public static void Clear()
        {
            _cachedMeshes.Clear();
            _cachedVerticesInWorldSpaceCache.Clear();
            _cachedTrianglesCache.Clear();
            _cachedTrisFrontFacing.Clear();
            _cachedTrisInFrustum.Clear();

            _cachedVerticesInViewport.Clear();
            _cachedTrisCenterInViewport.Clear();
            _cachedTrisCenterMaxDistanceInViewport.Clear();
        }

        public static void CacheTriangles(Camera cam, IList<GameObject> objects)
        {
            Clear();

            // Get meshes

            SkinnedMeshRenderer[] skinnedMeshRenderers;
            MeshFilter[] meshFilters;
            getMeshObjects(objects, out skinnedMeshRenderers, out meshFilters);

            // Cache meshes
            if (meshFilters != null)
            {
                foreach (var meshFilter in meshFilters)
                {
                    if (!isLayerInLayers(meshFilter.gameObject.layer, Tools.visibleLayers))
                        continue;

                    if (meshFilter.sharedMesh == null)
                        continue;

                    cacheMesh(cam, meshFilter, meshFilter.sharedMesh);
                }
            }

            if (skinnedMeshRenderers != null)
            {
                foreach (var meshRenderer in skinnedMeshRenderers)
                {
                    if (!isLayerInLayers(meshRenderer.gameObject.layer, Tools.visibleLayers))
                        continue;

                    if (meshRenderer.sharedMesh == null)
                        continue;

                    bakeMeshIfNeeded(meshRenderer);
                    cacheMesh(cam, meshRenderer, _bakedMeshesCache[meshRenderer]);
                }
            }
        }

        private static void getMeshObjects(IList<GameObject> objects, out SkinnedMeshRenderer[] skinnedMeshRenderers, out MeshFilter[] meshFilters)
        {
            skinnedMeshRenderers = null;
            meshFilters = null;
            if (objects == null)
            {
                // Get objects from prefab stage or scene
                if (UtilsEditor.IsInPrefabStage())
                {
                    var root = UtilsEditor.GetPrefabStageRoot();

                    // Find the real root of the prefab stage.
                    var rootParent = root.transform.parent; // A root can have a parent? Apparently yes!
                    if (rootParent != null)
                        root = rootParent.gameObject;

                    if (root.gameObject != null)
                    {
                        skinnedMeshRenderers = root.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: false);
                        meshFilters = root.GetComponentsInChildren<MeshFilter>(includeInactive: false);
                    }
                }
                else
                {
                    // Find components in all loaded scenes.
                    skinnedMeshRenderers = Transform.FindObjectsOfType<SkinnedMeshRenderer>();
                    meshFilters = Transform.FindObjectsOfType<MeshFilter>();
                }
            }
            else
            {
                // Use given objects
                var tmpSkinnedMeshRenderers = new List<SkinnedMeshRenderer>();
                var tmpMeshFilters = new List<MeshFilter>();
                if (objects != null)
                {
                    foreach (var root in objects)
                    {
                        if (root == null)
                            continue;

                        tmpSkinnedMeshRenderers.AddRange(root.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: false));
                        tmpMeshFilters.AddRange(root.GetComponentsInChildren<MeshFilter>(includeInactive: false));
                    }
                }
                skinnedMeshRenderers = tmpSkinnedMeshRenderers.ToArray();
                meshFilters = tmpMeshFilters.ToArray();
            }
        }

        private static void cacheMesh(Camera cam, Component component, Mesh mesh)
        {
            if (_cachedMeshes.ContainsKey(component))
            {
                _cachedMeshes.Remove(component);
                _cachedVerticesInWorldSpaceCache.Remove(component);
                _cachedVerticesInViewport.Remove(component);
                _cachedTrianglesCache.Remove(component);
                _cachedTrisFrontFacing.Remove(component);
                _cachedTrisInFrustum.Remove(component);
                _cachedTrisCenterInViewport.Remove(component);
                _cachedTrisCenterMaxDistanceInViewport.Remove(component);
            }
            _cachedMeshes.Add(component, mesh);

            var verticesInWorldSpace = new List<Vector3>();
            mesh.GetVertices(verticesInWorldSpace);
            // Convert to world space
            int vertexCount = verticesInWorldSpace.Count;
            for (int i = 0; i < vertexCount; i++)
            {
                verticesInWorldSpace[i] = component.transform.TransformPoint(verticesInWorldSpace[i]);
            }
            _cachedVerticesInWorldSpaceCache.Add(component, verticesInWorldSpace);
            // Convert to camera space
            var verticesInViewport = new List<Vector3>();
            for (int i = 0; i < vertexCount; i++)
            {
                var pInView = cam.WorldToViewportPoint(verticesInWorldSpace[i]);
                // Correct (flip) points behind the camera.
                // Sadly this does not suffice as it is not correct (also points near z=0 tend to infinity and we get a lot of inaccuracy).
                // We still do it as it give us rougly corret points (important for triangle center tests).
                if (pInView.z < 0)
                {
                    pInView.x *= -1;
                    pInView.y *= -1;
                }
                verticesInViewport.Add(pInView);
            }
            _cachedVerticesInViewport.Add(component, verticesInViewport);

            var triangles = new Dictionary<int, List<int>>();
            var frontFacingPerSubMesh = new Dictionary<int, List<bool>>();
            var inFrustumPerSubMesh = new Dictionary<int, List<bool>>();
            var centerPointsPerSubMesh = new Dictionary<int, List<Vector3>>();
            var centerMaxDistancesPerSubMesh = new Dictionary<int, List<float>>();

            bool outsideCameraFrustum;
            float camNearDistance = cam.nearClipPlane;

            int subMeshCount = mesh.subMeshCount;
            for (int m = 0; m < subMeshCount; m++)
            {
                // Triangles
                var subMeshTriangles = new List<int>();
                mesh.GetTriangles(subMeshTriangles, m);
                triangles.Add(m, subMeshTriangles);

                // Per triangle caches:

                // Generate one entry per triangle per cache.
                var frontFacingInfos = new List<bool>();
                var inFrustumInfos = new List<bool>();
                var centerPoints = new List<Vector3>();
                var centerMaxDistances = new List<float>();
                int triCount = subMeshTriangles.Count;
                for (int i = 0; i < triCount; i += 3)
                {
                    var a = subMeshTriangles[i];
                    var b = subMeshTriangles[i + 1];
                    var c = subMeshTriangles[i + 2];

                    // Center point
                    var center = (verticesInViewport[a] + verticesInViewport[b] + verticesInViewport[c]) / 3f;
                    centerPoints.Add(center);
                    // Max distance is the maximumt distance from center to A B or C.
                    float maxDistance = Mathf.Sqrt(Mathf.Max(
                        (center - verticesInViewport[a]).sqrMagnitude,
                        (center - verticesInViewport[b]).sqrMagnitude,
                        (center - verticesInViewport[c]).sqrMagnitude
                    ));
                    centerMaxDistances.Add(maxDistance);

                    // Front or back?
                    // Calculate the face normal (vertex normals are not reliable as they may be "smoothed").
                    Vector3 ab = verticesInWorldSpace[b] - verticesInWorldSpace[a];
                    Vector3 ac = verticesInWorldSpace[c] - verticesInWorldSpace[a];
                    var faceNormal = Vector3.Cross(ab, ac);
                    /* (Skip normalization, it is not needed.)
                    // Normalize without clamping values below 1E-05f (Unitys normalize does that).
                    float magnitude = faceNormal.magnitude;
                    if (magnitude > 0) faceNormal /= magnitude;
                    */
                    Vector3 rayDirection = verticesInWorldSpace[a] - cam.transform.position;
                    bool isFrontFace = Vector3.Dot(rayDirection, faceNormal) < 0;
                    frontFacingInfos.Add(isFrontFace);

                    // Is the triangle inside or outside the camera frustum?
                    // Remember: Some triangles have no vertices inside the frustum yet still intersect it (big tris). That's why we check if all are above, below, behind, ...
                    outsideCameraFrustum =
                        // triangle is behind the camera plane?
                        (verticesInViewport[a].z < camNearDistance && verticesInViewport[b].z < camNearDistance && verticesInViewport[c].z < camNearDistance)
                        // left to the camera
                        || (verticesInViewport[a].x < 0 && verticesInViewport[b].x < 0 && verticesInViewport[c].x < 0)
                        // right to the camera
                        || (verticesInViewport[a].x > 1 && verticesInViewport[b].x > 1 && verticesInViewport[c].x > 1)
                        // below the camera
                        || (verticesInViewport[a].y < 0 && verticesInViewport[b].y < 0 && verticesInViewport[c].y < 0)
                        // above the camera
                        || (verticesInViewport[a].y > 1 && verticesInViewport[b].y > 1 && verticesInViewport[c].y > 1);
                    inFrustumInfos.Add(!outsideCameraFrustum);
                }
                frontFacingPerSubMesh.Add(m, frontFacingInfos);
                inFrustumPerSubMesh.Add(m, inFrustumInfos);
                centerPointsPerSubMesh.Add(m, centerPoints);
                centerMaxDistancesPerSubMesh.Add(m, centerMaxDistances);
            }

            _cachedTrianglesCache.Add(component, triangles);
            _cachedTrisFrontFacing.Add(component, frontFacingPerSubMesh);
            _cachedTrisInFrustum.Add(component, inFrustumPerSubMesh);
            _cachedTrisCenterInViewport.Add(component, centerPointsPerSubMesh);
            _cachedTrisCenterMaxDistanceInViewport.Add(component, centerMaxDistancesPerSubMesh);
        }

        static bool isLayerInLayers(int layer, int layers)
        {
            return ((1 << layer) & layers) != 0;
        }

        public static void GetTrianglesUnderMouse(Event evt, bool cullBack, float rayThickness, float rayDepth, IList<GameObject> objects, bool allowMultipleResults, IList<RayCastTriangleResult> results)
        {
            GetTrianglesUnderMouse(evt.mousePosition, cullBack, rayThickness, rayDepth, objects, allowMultipleResults, results);
        }

        static List<RayCastTriangleResult> _tmpResults = new List<RayCastTriangleResult>();

        public static void GetTrianglesUnderMouse(Vector2 mousePosition, bool cullBack, float rayThickness, float rayDepth, IList<GameObject> objects, bool allowMultipleResults, IList<RayCastTriangleResult> results)
        {
            _tmpResults.Clear();

            if (SceneView.lastActiveSceneView == null || SceneView.lastActiveSceneView.camera == null)
                return;

            var ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            var cam = SceneView.lastActiveSceneView.camera;
            var rayOriginInViewport = cam.WorldToViewportPoint(ray.origin);

            foreach (var obj in objects)
            {
                if (obj == null)
                    continue;

                // Mesh Renderers
                var meshRenderes = obj.GetComponentsInChildren<MeshRenderer>(includeInactive: false);
                foreach (var renderer in meshRenderes)
                {
                    // Bounding box check
                    if (!renderer.bounds.IntersectRay(ray))
                        continue;

                    // Mesh Filters
                    var meshFilter = renderer.gameObject.GetComponent<MeshFilter>();
                    if (meshFilter)
                    {
                        RayCastCached(
                            cam, rayOriginInViewport, meshFilter,
                            cullBack, rayThickness, _tmpResults
                            );
                    }
                }

                // Skinned Mesh Renderers
                var skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: false);
                foreach (var renderer in skinnedMeshRenderers)
                {
                    if (renderer == null)
                        continue;

                    if (!_cachedMeshes.ContainsKey(renderer))
                        continue;

                    var skinnedBakedMesh = _cachedMeshes[renderer];

                    // Skinned mesh renderer bounds are not reliable (the user may have messed them up).
                    var bounds = skinnedBakedMesh.bounds;
                    // Calc none aligned bounding box (based on baked mesh bounds).
                    Vector3[] points = new Vector3[8];
                    points[0] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
                    points[1] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
                    points[2] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);
                    points[3] = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);
                    points[4] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
                    points[5] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
                    points[6] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);
                    points[7] = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = renderer.transform.TransformPoint(points[i]);
                    }
                    // calc axis aligned bounding box (for ray intersect check)
                    var center = renderer.transform.TransformPoint(bounds.center);
                    Vector3 min = points[0];
                    Vector3 max = points[0];
                    for (int i = 1; i < points.Length; i++)
                    {
                        min = Vector3.Min(min, points[i]);
                        max = Vector3.Max(max, points[i]);
                    }
                    Vector3 size = max - min;
                    bounds = new Bounds(center, size);
                    // UtilsEditor.DrawBounds(bounds);

                    // Bounding box check
                    if (!bounds.IntersectRay(ray))
                        continue;

                    RayCastCached(
                        cam, rayOriginInViewport, renderer,
                        cullBack, rayThickness, _tmpResults
                        );
                }
            }

            // Find the result with the lowest distance triangle of all meshes.
            if (_tmpResults.Count > 0 && !allowMultipleResults)
            {
                RayCastTriangleResult finalResult = default;
                float minDistance = float.MaxValue;
                foreach (var result in _tmpResults)
                {
                    if (result.Distance < minDistance)
                    {
                        minDistance = result.Distance;
                        finalResult = result;
                    }
                }
                results.Add(finalResult);
            }
            else
            {
                float minDistance = float.MaxValue;
                foreach (var tmpResult in _tmpResults)
                {
                    if (tmpResult.Distance < minDistance)
                    {
                        minDistance = tmpResult.Distance;
                    }
                }
                foreach (var tmpResult in _tmpResults)
                {
                    if (tmpResult.Distance < minDistance + rayDepth)
                    {
                        results.Add(tmpResult);
                    }
                }
            }
        }

        /// <summary>
        /// Raycast a cached mesh.
        /// </summary>
        public static void RayCastCached(
            Camera cam, Vector2 pos, Component comp, bool cullBack, float rayThickness,
            IList<RayCastTriangleResult> results)
        {
            // Abort if not in cache.
            if (_cachedMeshes == null || !_cachedMeshes.ContainsKey(comp))
            {
                return;
            }

            var mesh = _cachedMeshes[comp];

            // Gather data from cache.
            var verticesCamera = _cachedVerticesInViewport[comp];
            var verticesWorld = _cachedVerticesInWorldSpaceCache[comp];
            var meshTriangles = _cachedTrianglesCache[comp];
            var frontFacingInfos = _cachedTrisFrontFacing[comp];
            var inFrustumInfos = _cachedTrisInFrustum[comp];
            var centerTrianglesViewport = _cachedTrisCenterInViewport[comp];
            var centerMaxDistances = _cachedTrisCenterMaxDistanceInViewport[comp];

            // Check each submesh.
            int subMeshCount = mesh.subMeshCount;
            for (int i = 0; i < subMeshCount; i++)
            {
                // get data for submesh
                var triangles = meshTriangles[i];
                var frontInfos = frontFacingInfos[i];
                var frustumInfos = inFrustumInfos[i];
                var centers = centerTrianglesViewport[i];
                var maxDistances = centerMaxDistances[i];

                rayCastCache(
                    cam, pos, comp.transform, mesh, comp, i,
                    verticesCamera, verticesWorld, triangles, frontInfos, frustumInfos, centers, maxDistances,
                    cullBack, rayThickness, results
                );
            }
        }

        static void rayCastCache(
            Camera cam, Vector2 posInViewport, Transform transform, Mesh mesh, Component comp, int subMeshIndex,
            List<Vector3> verticesViewport, List<Vector3> verticesWorld, List<int> triangles,
            List<bool> frontFacingInfos, List<bool> inFrustumInfos, List<Vector3> triangleCentersViewport, List<float> centerMaxDistances,
            bool cullBack, float rayThickness, IList<RayCastTriangleResult> results
            )
        {
            float camNearDistance = cam.nearClipPlane;
            Ray ray = cam.ViewportPointToRay(posInViewport);
            // The viewport is always square. To compensate we need to take the aspect ratio into account.
            float cameraAspect = cam.aspect;
            rayThickness /= cameraAspect;
            float sqrRayThickness = rayThickness * rayThickness;
            int a, b, c;
            int triangleCount = triangles.Count;
            int triangleIndex;
            bool isFrontFace;
            bool isVisible;
            Vector3 center;
            for (int i = 0; i < triangleCount; i = i + 3)
            {
                triangleIndex = i / 3;
                a = triangles[i];
                b = triangles[i + 1];
                c = triangles[i + 2];

                // Skip due to backface culling
                isFrontFace = frontFacingInfos[triangleIndex];
                if (!isFrontFace && cullBack)
                {
                    continue;
                }

                // Skip if the triangle is not in the camera frustum
                isVisible = inFrustumInfos[triangleIndex];
                if (!isVisible)
                {
                    continue;
                }

                center = triangleCentersViewport[triangleIndex];

                if (rayThickness > 0f)
                {
                    float dx = center.x - posInViewport.x;
                    float dy = (center.y - posInViewport.y) / cameraAspect;
                    float sqrDistance = dx * dx + dy * dy;

                    // Max distance check (bounding circle)
                    // We can skip a lot of triangles this way.
                    float maxDistance = centerMaxDistances[triangleIndex] + rayThickness;
                    if (sqrDistance > (maxDistance * maxDistance))
                        continue;

                    // Distance check on center
                    bool distanceCheck = sqrDistance < sqrRayThickness;

                    // If center check fails then try the three vertices instead.
                    if (!distanceCheck)
                    {
                        // a
                        dx = verticesViewport[a].x - posInViewport.x;
                        dy = verticesViewport[a].y - posInViewport.y;
                        sqrDistance = dx * dx + dy * dy;
                        distanceCheck |= sqrDistance < sqrRayThickness;
                        // b
                        if (!distanceCheck)
                        {
                            dx = verticesViewport[b].x - posInViewport.x;
                            dy = verticesViewport[b].y - posInViewport.y;
                            sqrDistance = dx * dx + dy * dy;
                            distanceCheck |= sqrDistance < sqrRayThickness;
                        }
                        // c
                        if (!distanceCheck)
                        {
                            dx = verticesViewport[c].x - posInViewport.x;
                            dy = verticesViewport[c].y - posInViewport.y;
                            sqrDistance = dx * dx + dy * dy;
                            distanceCheck |= sqrDistance < sqrRayThickness;
                        }
                    }

                    if (distanceCheck)
                    {
                        var tri = new RayCastTriangleResult(
                                true,
                                transform,
                                mesh,
                                comp,
                                subMeshIndex,
                                new Vector3Int(a, b, c),
                                transform.InverseTransformPoint(verticesWorld[a]),
                                transform.InverseTransformPoint(verticesWorld[b]),
                                transform.InverseTransformPoint(verticesWorld[c]),
                                !isFrontFace,
                                verticesViewport[a].z
                            );

                        results.Add(tri);

                        // Continue (Skips detailed checks below)
                        continue;
                    }
                }

                /*
                // Debug: Draw hit triangle
                bool isBehind = verticesViewport[a].z < 0 || verticesViewport[b].z < 0 || verticesViewport[c].z < 0;
                Vector2 v0 = verticesViewport[a];
                Vector2 v1 = verticesViewport[b];
                Vector2 v2 = verticesViewport[c];
                Debug.DrawLine(v0, v1, isBehind ? Color.yellow : Color.red, 0.1f);
                Debug.DrawLine(v1, v2, isBehind ? Color.yellow : Color.red, 0.1f);
                Debug.DrawLine(v2, v0, isBehind ? Color.yellow : Color.red, 0.1f);
                // Draw pos in viewport
                Debug.DrawLine(posInViewport, posInViewport + Vector2.up * 0.02f, Color.green, 0.1f);
                // draw center
                Debug.DrawLine((Vector2)center, (Vector2)center + Vector2.up * 0.02f, Color.blue, 0.1f);
                //*/

                // 2D Check (checking triangle intersection in ViewPort space)
                if (verticesViewport[a].z > camNearDistance && verticesViewport[b].z > camNearDistance && verticesViewport[c].z > camNearDistance)
                {
                    // Is posInViewport within the triangle?
                    Vector2 va = verticesViewport[a];
                    Vector2 vb = verticesViewport[b];
                    Vector2 vc = verticesViewport[c];
                    var ab = vb - va;
                    var bc = vc - vb;
                    var ca = va - vc;
                    var ap = posInViewport - va;
                    var bp = posInViewport - vb;
                    var cp = posInViewport - vc;
                    // cross
                    int c0 = ab.x * ap.y - ab.y * ap.x > 0 ? 1 : -1;
                    int c1 = bc.x * bp.y - bc.y * bp.x > 0 ? 1 : -1;
                    int c2 = ca.x * cp.y - ca.y * cp.x > 0 ? 1 : -1;
                    // Sum cross products.
                    int sum = c0 + c1 + c2;
                    // If the sum is 3 or -3 then the posInViewport point is
                    // on the same side of all lines and thus within the triangle.
                    if (sum == 3 || sum == -3)
                    {
                        // Slower but more accurate distance
                        float distance;
                        var planeNormal = Vector3.Cross(verticesViewport[c] - verticesViewport[b], verticesViewport[a] - verticesViewport[b]);
                        var plane = new Plane(planeNormal, verticesViewport[a]);
                        plane.Raycast(new Ray(new Vector3(posInViewport.x, posInViewport.y, 0f), Vector3.forward), out distance);
                        // float distance = center.z; // Faster but less accurate way (distance based on center.z vs based on actual mouse pos)

                        var tri = new RayCastTriangleResult(
                            true,
                            transform,
                            mesh,
                            comp,
                            subMeshIndex,
                            new Vector3Int(a, b, c),
                            transform.InverseTransformPoint(verticesWorld[a]),
                            transform.InverseTransformPoint(verticesWorld[b]),
                            transform.InverseTransformPoint(verticesWorld[c]),
                            !isFrontFace,
                            distance
                        );

                        results.Add(tri);
                    }
                }
                else
                {
                    // If the 2D check failed but the triangle is in front of the camera then fall back on the 3D check.
                    float distance = IntersectRayTriangle3D(ray, verticesWorld[a], verticesWorld[b], verticesWorld[c]);
                    if (!float.IsNaN(distance))
                    {
                        var tri = new RayCastTriangleResult(
                            true,
                            transform,
                            mesh,
                            comp,
                            subMeshIndex,
                            new Vector3Int(a, b, c),
                            transform.InverseTransformPoint(verticesWorld[a]),
                            transform.InverseTransformPoint(verticesWorld[b]),
                            transform.InverseTransformPoint(verticesWorld[c]),
                            !isFrontFace,
                            distance
                        );

                        results.Add(tri);
                    }
                }
            }
        }

        const float kEpsilon = 0.000001f;

        /// <summary>
        /// Thanks to: https://answers.unity.com/questions/861719/a-fast-triangle-triangle-intersection-algorithm-fo.html
        /// Ray-versus-triangle intersection test suitable for ray-tracing etc.
        /// Port of Möller–Trumbore algorithm c++ version from:
        /// https://en.wikipedia.org/wiki/Möller–Trumbore_intersection_algorithm
        /// </summary>
        /// <returns><c>The distance along the ray to the intersection</c> if one exists, <c>NaN</c> if one does not.</returns>
        /// <param name="ray">the ray</param>
        /// <param name="v0">A vertex 0 of the triangle.</param>
        /// <param name="v1">A vertex 1 of the triangle.</param>
        /// <param name="v2">A vertex 2 of the triangle.</param>
        public static float IntersectRayTriangle3D(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            // edges from v1 & v2 to v0.     
            Vector3 e1 = v1 - v0;
            Vector3 e2 = v2 - v0;

            Vector3 h = Vector3.Cross(ray.direction, e2);
            float a = Vector3.Dot(e1, h);
            if ((a > -kEpsilon) && (a < kEpsilon))
            {
                return float.NaN;
            }

            float f = 1.0f / a;

            Vector3 s = ray.origin - v0;
            float u = f * Vector3.Dot(s, h);
            if ((u < 0.0f) || (u > 1.0f))
            {
                return float.NaN;
            }

            Vector3 q = Vector3.Cross(s, e1);
            float v = f * Vector3.Dot(ray.direction, q);
            if ((v < 0.0f) || (u + v > 1.0f))
            {
                return float.NaN;
            }

            float t = f * Vector3.Dot(e2, q);
            if (t > kEpsilon)
            {
                return t;
            }
            else
            {
                return float.NaN;
            }
        }

        /// <summary>
        /// Tries to select all triangles which are connected to the given selected triangle. However if the mesh has a lot of tris it will
        /// stop at some point because the time needed grows exponentially.
        /// </summary>
        /// <param name="objects">The object to search (they need to be cached).</param>
        /// <param name="selectedTri">The triangle to start the link search from.</param>
        /// <param name="limitToSubMesh">Enable to limit selection to the sub mesh which the given triangle is part of.</param>
        /// <returns></returns>
        public static List<SelectedTriangle> AddLinked(IList<GameObject> objects, SelectedTriangle selectedTri, bool limitToSubMesh)
        {
            var results = new List<SelectedTriangle>() { };

            var components = new List<Component>();

            var verticesPerComp = new Dictionary<Component, Vector3[]>();
            var trisPerCompPerSubMesh = new Dictionary<Component, List<int[]>>();
            foreach (var obj in objects)
            {
                if (obj == null)
                    continue;

                var meshFilters = obj.GetComponentsInChildren<MeshFilter>(includeInactive: false);
                components.AddRange(meshFilters);

                var skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: false);
                components.AddRange(skinnedMeshRenderers);
            }

            // Cache vertices and tris. We'll need them often.
            foreach (var comp in components)
            {
                var mesh = _cachedMeshes[comp];

                if (!verticesPerComp.ContainsKey(comp))
                {
                    verticesPerComp.Add(comp, mesh.vertices);
                }

                if (!trisPerCompPerSubMesh.ContainsKey(comp))
                {
                    var trisPerSubMesh = new List<int[]>();
                    for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
                    {
                        trisPerSubMesh.Add(mesh.GetTriangles(subMeshIndex));
                    }
                    trisPerCompPerSubMesh.Add(comp, trisPerSubMesh);
                }
            }

            EditorUtility.DisplayProgressBar("Working", "Gathering linked polygons ..", 0.1f);
            try
            {

                foreach (var comp in components)
                {
                    var vertices = verticesPerComp[comp];
                    var trisPerSubMesh = trisPerCompPerSubMesh[comp];

                    var allConnectedVertices = new List<int>();
                    var lastConnectedVertices = new List<int>()
                    {
                        selectedTri.TriangleIndices.x, selectedTri.TriangleIndices.y, selectedTri.TriangleIndices.z
                    };
                    var newConnectedVertices = new List<int>();

                    if (selectedTri.Component == comp)
                    {
                        var watch = new System.Diagnostics.Stopwatch();
                        watch.Start();
                        bool completed = false;
                        int counter = 0;
                        int maxCounter = 100000000;
                        while (!completed && counter < maxCounter)
                        {
                            newConnectedVertices.Clear();
                            int count = lastConnectedVertices.Count;
                            for (int subMeshIndex = 0; subMeshIndex < trisPerSubMesh.Count; subMeshIndex++)
                            {
                                if (counter > maxCounter)
                                    break;

                                if (limitToSubMesh && selectedTri.SubMeshIndex != subMeshIndex)
                                {
                                    continue;
                                }

                                int triCount = trisPerSubMesh[subMeshIndex].Length;
                                for (int i = 0; i < triCount; i += 3)
                                {
                                    var idx0 = trisPerSubMesh[subMeshIndex][i];
                                    var idx1 = trisPerSubMesh[subMeshIndex][i + 1];
                                    var idx2 = trisPerSubMesh[subMeshIndex][i + 2];
                                    var v0 = vertices[idx0];
                                    var v1 = vertices[idx1];
                                    var v2 = vertices[idx2];

                                    if (counter > maxCounter)
                                        break;

                                    for (int v = 0; v < count; v++)
                                    {
                                        counter++;
                                        if (counter > maxCounter)
                                            break;

                                        // Does any of the vertices overlap?
                                        // N2H investigate why this "if (idx0 == lastConnectedVertices[v] || idx1 == lastConnectedVertices[v] || idx2 == lastConnectedVertices[v])"
                                        // does not always work. It would be faster.
                                        if (V3Equal(vertices[idx0], vertices[lastConnectedVertices[v]])
                                            || V3Equal(vertices[idx1], vertices[lastConnectedVertices[v]])
                                            || V3Equal(vertices[idx2], vertices[lastConnectedVertices[v]])
                                            )
                                        {
                                            var newTri = new SelectedTriangle();

                                            newTri.Success = true;
                                            newTri.Transform = comp.transform;
                                            newTri.Mesh = _cachedMeshes[comp];
                                            newTri.Component = comp;
                                            newTri.SubMeshIndex = subMeshIndex;
                                            newTri.TriangleIndices = new Vector3Int(idx0, idx1, idx2);
                                            newTri.VertexLocal0 = v0;
                                            newTri.VertexLocal1 = v1;
                                            newTri.VertexLocal2 = v2;

                                            results.Add(newTri);

                                            // update vertices
                                            if (!allConnectedVertices.Contains(idx0))
                                            {
                                                allConnectedVertices.Add(idx0);
                                                newConnectedVertices.Add(idx0);
                                            }
                                            if (!allConnectedVertices.Contains(idx1))
                                            {
                                                allConnectedVertices.Add(idx1);
                                                newConnectedVertices.Add(idx1);
                                            }
                                            if (!allConnectedVertices.Contains(idx2))
                                            {
                                                allConnectedVertices.Add(idx2);
                                                newConnectedVertices.Add(idx2);
                                            }
                                        }
                                    }
                                }
                            }
                            if (newConnectedVertices.Count == 0)
                            {
                                completed = true;
                            }

                            lastConnectedVertices.Clear();
                            lastConnectedVertices.AddRange(newConnectedVertices);
                        }

                        if (counter > maxCounter)
                        {
                            EditorApplication.delayCall += () =>
                            {
                                EditorUtility.DisplayDialog(
                                    "The mesh is too complex!",
                                    "Sorry, the mesh is has too many triangles for linked polygon search. It would take forever." +
                                    "\n\nThe search has been aborted.",
                                    "OK");
                            };
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            return results;
        }

        public static bool V3Equal(Vector3 a, Vector3 b)
        {
            return Vector3.SqrMagnitude(a - b) < 0.0000001f;
        }
    }
}

