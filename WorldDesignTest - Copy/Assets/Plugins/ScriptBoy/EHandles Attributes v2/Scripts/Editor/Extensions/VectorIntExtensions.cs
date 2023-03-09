using UnityEngine;

namespace EHandles
{
    internal static class VectorIntExtensions
    {
        public static Vector3 Vector3(this Vector3Int v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static Vector3 Vector3(this Vector2Int v)
        {
            return new Vector3(v.x, v.y);
        }

        public static Vector2 Vector2(this Vector2Int v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 Vector2(this Vector3Int v)
        {
            return new Vector2(v.x, v.y);
        }
    }
}