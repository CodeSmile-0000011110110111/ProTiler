using UnityEngine;
using UnityEditor;

namespace EHandles
{
    public static class EditorGridUtility
    {
        public static Vector3 SnapToGrid(Vector3 v)
        {
            float d = OrderOfMagnitude(HandleUtility.GetHandleSize(v) / 0.7f);

            return new Vector3(Round(v.x, d), Round(v.y, d), Round(v.z, d));
        }

        public static Vector3 SnapToGrid2D(Vector3 v)
        {
            float d = OrderOfMagnitude(HandleUtility.GetHandleSize(v) / 0.7f);

            return new Vector3(Round(v.x, d), Round(v.y, d), v.z);
        }

        private static float OrderOfMagnitude(float v)
        {
            return Mathf.Pow(10, Mathf.Floor(Mathf.Log10(v)));
        }

        private static float Round(float v, float d)
        {
            return Mathf.Round(v / d) * d;
        }
    }
}