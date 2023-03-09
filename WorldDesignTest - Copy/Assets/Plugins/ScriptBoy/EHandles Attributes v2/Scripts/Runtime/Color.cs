using System;
using UnityEngine;

namespace EHandles
{
    public enum Color
    {
        white, black, yellow, red, blue, green
    }
}

[Obsolete("The EHandleColor is deprecated, please use EHandles.Color instead.\n(The EHandleColor will be removed in the next update)", true)]
public enum EHandleColor
{
    white, black, yellow, red, blue, green
}