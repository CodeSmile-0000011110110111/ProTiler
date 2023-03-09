using System;

namespace EHandles
{
    public class DrawCubeAttribute : Attribute
    {
        public Color color;
        public float size = 0.1f;

        /// <summary>
        /// <para>[size = float]</para> 
        /// <para>[color = HandleColor]</para> 
        /// </summary>
        public DrawCubeAttribute()
        {

        }

        public DrawCubeAttribute(float size)
        {
            this.size = size;
        }

        public DrawCubeAttribute(float size, Color color)
        {
            this.size = size;
            this.color = color;
        }
    }
}