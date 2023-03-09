using System;
using System.ComponentModel;
using UnityEngine;

namespace EHandles
{

    [AttributeUsage(validOn: AttributeTargets.Field, AllowMultiple = true)]
    public class DrawCircleAttribute : Attribute
    {
        public Color color;

        public float radius = 0.1f;
        public string radiusField;
        /// <summary>
        /// "x,y,z" or "x y z"
        /// </summary>
        public string rotation;


        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("The radiusSourceField is deprecated, please use radiusField instead.")]
        public string radiusSourceField;

        /// <summary>
        /// <para>[radius = float] or [radiusSourceField = string]</para> 
        /// <para>[color = HandleColor]</para> 
        /// </summary>
        public DrawCircleAttribute()
        {

        }

        public DrawCircleAttribute(float radius)
        {
            this.radius = radius;
        }

        public DrawCircleAttribute(float radius, Color color)
        {
            this.radius = radius;
            this.color = color;
        }

        public DrawCircleAttribute(Color color)
        {
            this.color = color;
        }

        public DrawCircleAttribute(string radiusSourceField)
        {
            this.radiusField = radiusSourceField;
        }

        public DrawCircleAttribute(string radiusSourceField, Color color)
        {
            this.radiusField = radiusSourceField;
            this.color = color;
        }
    }
}