using System;

namespace EHandles
{
    public class LabelAttribute : Attribute
    {
        public string text;
        public Color color;

        /// <summary>
        /// <para>[text = string]</para> 
        /// <para>[color = EColor]</para> 
        /// </summary>
        public LabelAttribute()
        {

        }

        public LabelAttribute(string text)
        {
            this.text = text;
        }

        public LabelAttribute(string text, Color color)
        {
            this.text = text;
            this.color = color;
        }

        public LabelAttribute(Color color)
        {
            this.color = color;
        }
    }
}