using System;

namespace ManagedWin32.Api
{
    [Flags]
    public enum WinEventHookFlags
    {
        WINEVENT_SKIPOWNTHREAD = 1,
        WINEVENT_SKIPOWNPROCESS = 2,
        WINEVENT_OUTOFCONTEXT = 0,
        WINEVENT_INCONTEXT = 4
    }
}