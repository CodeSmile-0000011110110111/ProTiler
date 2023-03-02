using System.Collections.Generic;

namespace EHandles
{
    public struct AttributeSettings
    {
        public bool useLocalSpace;
        public bool useArrayHotkeys;

        public UseLocalSpaceAttribute localSpaceAttr;

        public AttributeSettings(IEnumerable<SettingsAttribute> attributes)
        {
            useLocalSpace = false;
            useArrayHotkeys = false;
            localSpaceAttr = null;

            foreach (var attribute in attributes)
            {
                if (attribute is UseLocalSpaceAttribute)
                {
                    useLocalSpace = true;
                    localSpaceAttr = (UseLocalSpaceAttribute)attribute;
                }

                if (attribute is UseArrayHotkeysAttribute)
                {
                    useArrayHotkeys = true;
                }
            }
        }
    }
}
