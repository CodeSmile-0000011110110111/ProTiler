using System;

namespace EHandles
{
    public class DrawSphereAttribute : Attribute
    {
        public Color color;
        public float size;

        /// <summary>
        /// <para>[size = float]</para> 
        /// <para>[color = HandleColor]</para> 
        /// </summary>
        public DrawSphereAttribute()
        {

        }

        public DrawSphereAttribute(float size)
        {
            this.size = size;
        }

        public DrawSphereAttribute(float size, Color color)
        {
            this.size = size;
            this.color = color;
        }
    }
}