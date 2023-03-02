using System;

namespace EHandles
{
    public class DrawAAPolylineAttribute : Attribute
    {
        public Color color;
        public float width = 4;
        public bool loop;

        /// <summary>
        /// <para>[loop = bool]</para> 
        /// <para>[width = float]</para> 
        /// <para>[color = HandleColor]</para> 
        /// </summary>
        public DrawAAPolylineAttribute()
        {

        }

        public DrawAAPolylineAttribute(float width, bool loop)
        {
            this.width = width;
            this.loop = loop;
        }

        public DrawAAPolylineAttribute(float width, bool loop, Color color)
        {
            this.width = width;
            this.loop = loop;
            this.color = color;
        }
    }
}