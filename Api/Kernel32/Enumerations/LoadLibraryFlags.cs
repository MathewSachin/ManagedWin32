using System;

namespace ManagedWin32.Api
{
    [Flags]
    public enum LoadLibraryFlags
    {
        DontResolveDllReferences = 0x00000001,
        LoadAsDataFile = 0x00000002,
        LoadWithAlteredSearchPath = 0x00000008,
        IgnoreCodeAuthZLevel = 0x00000010
    }
}
