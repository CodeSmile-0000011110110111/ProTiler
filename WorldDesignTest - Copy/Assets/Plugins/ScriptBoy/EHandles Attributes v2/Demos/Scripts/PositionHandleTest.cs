using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EHandles.Tests
{
    public class PositionHandleTest : MonoBehaviour
    {
        [EHandles.PositionHandle]
        public Vector3Int position;
    }
}