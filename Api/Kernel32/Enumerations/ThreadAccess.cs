using System;

namespace ManagedWin32.Api
{
    [Flags]
    public enum ThreadAccess
    {
        Terminate = 0x0001,
        SuspendResume = 0x0002,
        GetContext = 0x0008,
        SetContext = 0x0010,
        SetInfo = 0x0020,
        QueryInfo = 0x0040,
        SetThreadToken = 0x0080,
        Impersonate = 0x0100,
        DirectImpersonation = 0x0200
    }
}
