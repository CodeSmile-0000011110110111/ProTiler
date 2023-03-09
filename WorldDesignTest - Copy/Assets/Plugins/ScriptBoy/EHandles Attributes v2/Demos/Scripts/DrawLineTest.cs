using UnityEngine;

namespace EHandles.Tests
{
    public class DrawLineTest : MonoBehaviour
    {
        [EHandles.DrawLine("b", Color.red)]
        [EHandles.PositionHandlePro]
        public Vector3 a;

        [EHandles.PositionHandlePro]
        public Vector3 b;

        [EHandles.Label(Color.red)]
        [EHandles.DrawAAPolyline(color = Color.red)]
        [EHandles.PositionHandlePro, EHandles.UseArrayHotkeys, EHandles.UseLocalSpace]
        public Vector3[] path;
    }
}