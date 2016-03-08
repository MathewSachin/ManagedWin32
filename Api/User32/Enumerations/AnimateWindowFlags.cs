using System;

namespace ManagedWin32.Api
{
    [Flags]
    public enum AnimateWindowFlags
    {
        HOR_POSITIVE = 0x00000001,
        HOR_NEGATIVE = 0x00000002,
        VER_POSITIVE = 0x00000004,
        VER_NEGATIVE = 0x00000008,
        CENTER = 0x00000010,
        HIDE = 0x00010000,
        ACTIVATE = 0x00020000,
        SLIDE = 0x00040000,
        BLEND = 0x00080000
    }
}