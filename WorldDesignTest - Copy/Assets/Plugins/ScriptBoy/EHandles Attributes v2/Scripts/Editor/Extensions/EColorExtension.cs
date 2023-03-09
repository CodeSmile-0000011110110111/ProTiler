using UnityEngine;

namespace EHandles
{
    public static class EColorExtension
    {
        public static UnityEngine.Color ToUnityColor(this Color color)
        {
            switch (color)
            {
                case Color.black: return UnityEngine.Color.black;
                case Color.blue: return UnityEngine.Color.blue;
                case Color.green: return UnityEngine.Color.green;
                case Color.red: return UnityEngine.Color.red;
                case Color.white: return UnityEngine.Color.white;
                case Color.yellow: return UnityEngine.Color.yellow;
            }
            return UnityEngine.Color.white;
        }
    }
}