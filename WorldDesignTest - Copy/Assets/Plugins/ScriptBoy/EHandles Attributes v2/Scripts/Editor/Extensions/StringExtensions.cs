using UnityEngine;

namespace EHandles
{
    public static class StringExtensions
    {
        public static Quaternion ToQuaternion(this string text)
        {
            if (text != null)
            {
                if (TryParseToVector3(text, out var v))
                {
                    return Quaternion.Euler(v);
                }

                Debug.LogWarning($"Can't parse ({text}) to Quaternion.\n Suported formats: x,y,z  or  x y z");
            }

            return Quaternion.identity;
        }

        public static Vector3 ToVector3(this string text)
        {
            if (text != null)
            {
                if (TryParseToVector3(text, out var v))
                {
                    return v;
                }

                Debug.LogWarning($"Can't parse ({text}) to Vector3.\n Suported formats: x,y,z  or  x y z");
            }

            return Vector3.zero;
        }

        private static bool TryParseToVector3(this string text, out Vector3 v)
        {
            var axis = text.Split(' ', ',');
            var result = axis.Length == 3;
            if (result)
            {
                result |= float.TryParse(axis[0], out v.x);
                result |= float.TryParse(axis[1], out v.y);
                result |= float.TryParse(axis[2], out v.z);
            }
            else
            {
                v = Vector3.zero;
            }

            return result;
        }
    }
}