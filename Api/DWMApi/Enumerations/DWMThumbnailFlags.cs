using System;

namespace ManagedWin32.Api
{
    [Flags]
    public enum DWMThumbnailFlags
    {
        RectDestination = 1,
        RectSource = 2,
        Opacity = 4,
        Visible = 8,
        SourceClientAreaOnly = 0x10
    }
}