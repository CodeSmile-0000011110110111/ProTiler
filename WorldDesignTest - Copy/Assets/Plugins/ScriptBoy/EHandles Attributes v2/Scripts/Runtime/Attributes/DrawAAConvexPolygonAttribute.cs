using System;

namespace EHandles
{
    public class DrawAAConvexPolygonAttribute : Attribute
    {
        public Color color;

        /// <summary>
        /// <para>[color = HandleColor]</para> 
        /// </summary>
        public DrawAAConvexPolygonAttribute()
        {

        }

        public DrawAAConvexPolygonAttribute(Color color)
        {
            this.color = color;
        }
    }
}