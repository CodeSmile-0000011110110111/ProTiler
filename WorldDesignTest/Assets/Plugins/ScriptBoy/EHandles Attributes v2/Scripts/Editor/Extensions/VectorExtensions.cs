using UnityEngine;

namespace EHandles
{
    internal static class VectorExtensions
    {
        public static Vector3Int Vector3Int(this Vector3 v)
        {
            return new Vector3Int((int)v.x, (int)v.y, (int)v.z);
        }

        public static Vector2Int Vector2Int(this Vector2 v)
        {
            return new Vector2Int((int)v.x, (int)v.y);
        }

        public static Vector2Int Vector2Int(this Vector3 v)
        {
            return new Vector2Int((int)v.x, (int)v.y);
        }
    }
}