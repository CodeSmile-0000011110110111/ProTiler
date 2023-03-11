using System;
using UnityEngine;

namespace Kamgam.MeshExtractor
{
    public class SelectedTriangle : IEquatable<SelectedTriangle>
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
        public Vector3 VertexGlobal0;
        public Vector3 VertexGlobal1;
        public Vector3 VertexGlobal2;

        public SelectedTriangle()
        { }

        public SelectedTriangle(RayCastTriangleResult triangleResult)
        {
            Success         = triangleResult.Success;
            Transform       = triangleResult.Transform;
            Mesh            = triangleResult.Mesh;
            Component       = triangleResult.Component;
            SubMeshIndex    = triangleResult.SubMeshIndex;
            TriangleIndices = triangleResult.TriangleIndices;
            VertexLocal0    = triangleResult.VertexLocal0;
            VertexLocal1    = triangleResult.VertexLocal1;
            VertexLocal2    = triangleResult.VertexLocal2;

            if (Transform != null)
            {
                VertexGlobal0 = Transform.TransformPoint(VertexLocal0);
                VertexGlobal1 = Transform.TransformPoint(VertexLocal1);
                VertexGlobal2 = Transform.TransformPoint(VertexLocal2);
            }
        }

        public void UpdateWorldPos()
        {
            VertexGlobal0 = Transform.TransformPoint(VertexLocal0);
            VertexGlobal1 = Transform.TransformPoint(VertexLocal1);
            VertexGlobal2 = Transform.TransformPoint(VertexLocal2);
        }

        public bool Equals(SelectedTriangle obj)
        {
            return obj != null && Mesh == obj.Mesh && SubMeshIndex == obj.SubMeshIndex && TriangleIndices == obj.TriangleIndices;
        }

        public override int GetHashCode()
        {
            return Mesh.GetHashCode() ^ SubMeshIndex.GetHashCode() ^ TriangleIndices.GetHashCode();
        }

        public SelectedTriangle Copy()
        {
            var copy = new SelectedTriangle();

            copy.Success         = Success;
            copy.Transform       = Transform;
            copy.Mesh            = Mesh;
            copy.Component       = Component;
            copy.SubMeshIndex    = SubMeshIndex;
            copy.TriangleIndices = TriangleIndices;
            copy.VertexLocal0    = VertexLocal0;
            copy.VertexLocal1    = VertexLocal1;
            copy.VertexLocal2    = VertexLocal2;
            copy.VertexGlobal0   = VertexGlobal0;
            copy.VertexGlobal1   = VertexGlobal1;
            copy.VertexGlobal2   = VertexGlobal2;

            return copy;
        }
    }
}
