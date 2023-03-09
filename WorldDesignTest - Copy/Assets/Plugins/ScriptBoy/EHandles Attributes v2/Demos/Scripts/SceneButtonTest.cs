using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EHandles.Tests
{
    public class SceneButtonTest : MonoBehaviour
    {
        public string text;

        [EHandles.SceneButton]
        public void Print()
        {
            Debug.Log(text);
        }
    }
}