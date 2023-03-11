using UnityEngine;

namespace Kamgam.MeshExtractor
{
    public struct RayCastTriangleResult
    {
        public bool Success;
        public Transform Transform;
        public Mesh Mesh;
        public Component Component;
        public int SubMeshIndex;
        public Vector3Int TriangleIndices;
        public Vector3 VertexLocal0;
        public Vector3 VertexLocal1;
        public Vector3 VertexLocal2;
        public bool IsBackFacing;
        public float Distance;

        public RayCastTriangleResult(bool success,
            Transform transform, Mesh mesh, Component component, int subMeshIndex,
            Vector3Int triangleIndices, Vector3 vertexLocal0, Vector3 vertexLocal1, Vector3 vertexLocal2,
            bool isBackFacing, float distance)
        {
            Success = success;
            Transform = transform;
            Mesh = mesh;
            Component = component;
            SubMeshIndex = subMeshIndex;
            TriangleIndices = triangleIndices;
            VertexLocal0 = vertexLocal0;
            VertexLocal1 = vertexLocal1;
            VertexLocal2 = vertexLocal2;
            IsBackFacing = isBackFacing;
            Distance = distance;
        }

        public RayCastTriangleResult(bool success)
        {
            Success = success;
            Transform = null;
            Mesh = null;
            Component = null;
            SubMeshIndex = -1;
            TriangleIndices = new Vector3Int(-1, -1, -1);
            VertexLocal0 = default;
            VertexLocal1 = default;
            VertexLocal2 = default;
            IsBackFacing = default;
            Distance = default;
        }
    }
}
