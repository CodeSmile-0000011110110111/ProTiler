using System;

namespace EHandles
{
    public class FreeMoveHandleAttribute : Attribute
    {
        public Color color;
        public float size;

        /// <summary>
        /// <para>[size = float]</para> 
        /// <para>[color = HandleColor]</para> 
        /// </summary>
        public FreeMoveHandleAttribute()
        {

        }

        public FreeMoveHandleAttribute(float size)
        {
            this.size = size;
        }

        public FreeMoveHandleAttribute(float size, Color color)
        {
            this.size = size;
            this.color = color;
        }
    }
}