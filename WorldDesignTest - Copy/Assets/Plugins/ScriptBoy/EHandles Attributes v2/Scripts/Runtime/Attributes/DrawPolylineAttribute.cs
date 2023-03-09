using System;

namespace EHandles
{
    public class DrawPolylineAttribute : Attribute
    {
        public Color color;
        public bool loop;

        /// <summary>
        /// <para>[loop = bool]</para> 
        /// <para>[color = EColor]</para> 
        /// </summary>
        public DrawPolylineAttribute()
        {

        }

        public DrawPolylineAttribute(bool loop)
        {
            this.loop = loop;
        }

        public DrawPolylineAttribute(bool loop, Color color)
        {

            this.loop = loop;
            this.color = color;
        }
    }
}