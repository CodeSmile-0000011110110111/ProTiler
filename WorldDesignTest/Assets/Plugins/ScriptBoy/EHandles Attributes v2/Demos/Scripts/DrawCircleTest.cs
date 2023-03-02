using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace EHandles.Tests
{
    public class DrawCircleTest : MonoBehaviour
    {
        [EHandles.DrawCircle(EHandles.Color.red, rotation = "90 0 0")]
        public float radius;
    }
}