using System;

namespace EHandles
{
    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = true)]
    public class DrawLineAttribute : Attribute
    {
        public Color color;
        public string endPointField;

        [Obsolete("The endPointSourceField is deprecated, please use endPointField instead.")]
        public string endPointSourceField;

        /// <summary>
        /// <para>[endPointSourceField = string]</para> 
        /// <para>[color = HandleColor]</para> 
        /// </summary>
        public DrawLineAttribute()// Draw a line from Vector3.zero
        {

        }

        public DrawLineAttribute(string endPointSourceField)
        {
            this.endPointField = endPointSourceField;
        }

        public DrawLineAttribute(string endPointSourceField, Color color)
        {
            this.endPointField = endPointSourceField;
            this.color = color;
        }
    }
}