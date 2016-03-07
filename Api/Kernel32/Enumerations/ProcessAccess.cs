using System;

namespace ManagedWin32.Api
{
    [Flags]
    public enum ProcessAccess
    {
        Terminate = 0x0001,
        CreateThread = 0x0002,
        SetSessionID = 0x0004,
        Operation = 0x0008,
        Read = 0x0010,
        Write = 0x0020,
        DupHandle = 0x0040,
        CreateProcess = 0x0080,
        SetQuota = 0x0100,
        SetInformation = 0x0200,
        QueryInformation = 0x0400,
        Synchronize = 0x00100000
    }
}
