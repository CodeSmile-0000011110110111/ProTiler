using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EHandles.Tests
{
    public class PositionHandleProTest : MonoBehaviour
    {
        [EHandles.Label(Color.red)]
        [EHandles.PositionHandlePro]
        public Vector3 position;

        [EHandles.Label(Color.red)]
        [EHandles.PositionHandlePro(buttonColor = Color.green), EHandles.UseLocalSpace]
        public Vector3 localPosition;
    }
}