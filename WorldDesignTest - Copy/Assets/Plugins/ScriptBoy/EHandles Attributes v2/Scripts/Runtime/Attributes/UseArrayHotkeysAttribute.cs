using System;

namespace EHandles
{
    /// <summary>
    /// Menu: Tools/ScriptBoy/Ehandles/Hotkeys
    /// </summary>
    public class UseArrayHotkeysAttribute : SettingsAttribute
    {

    }

    [Obsolete("PathEditor is deprecated, please use FreeMoveHandle + UseArrayHotkeys instead.")]
    public class PathEditorAttribute : Attribute
    {

    }
}
