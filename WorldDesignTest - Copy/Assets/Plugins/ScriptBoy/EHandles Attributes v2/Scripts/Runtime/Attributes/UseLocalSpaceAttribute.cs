using System;

namespace EHandles
{
    [Obsolete("The LocalSpace is deprecated, please use UseLocalSpace instead.")]
    public class LocalSpaceAttribute : System.Attribute
    {

    }

    public class UseLocalSpaceAttribute : SettingsAttribute
    {
        public string transformField;

        public UseLocalSpaceAttribute()
        {

        }

        public UseLocalSpaceAttribute(string parent)
        {
            this.transformField = parent;
        }
    }
}