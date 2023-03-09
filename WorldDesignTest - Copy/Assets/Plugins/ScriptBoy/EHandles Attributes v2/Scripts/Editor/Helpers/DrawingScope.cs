using System;
using UnityEngine;
using UnityEditor;

namespace EHandles
{
    public struct DrawingScope : IDisposable
    {
        private UnityEngine.Color m_DefaultColor;

        public DrawingScope(Color color)
        {
            m_DefaultColor = Handles.color;
            Handles.color = color.ToUnityColor();
        }

        public void Dispose()
        {
            Handles.color = m_DefaultColor;
        }
    }
}