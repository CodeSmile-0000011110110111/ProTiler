using System;

namespace EHandles
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SceneButtonAttribute : System.Attribute
    {
        public string text;
        public SceneButtonAttribute()
        {

        }

        public SceneButtonAttribute(string text)
        {
            this.text = text;
        }
    }
}