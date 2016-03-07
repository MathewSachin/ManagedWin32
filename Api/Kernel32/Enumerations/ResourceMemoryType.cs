using System;

namespace ManagedWin32.Api
{
    [Flags]
    public enum ResourceMemoryType : ushort
    {
        None = 0,
        Moveable = 0x10,
        Pure = 0x20,
        PreLoad = 0x40,
        Unknown = 7168
    }
}
