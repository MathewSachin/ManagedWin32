using System;

namespace ManagedWin32.Api
{
    public delegate bool EnumResNameProc(IntPtr hModule, ResourceType lpszType, IntPtr lpszName, IntPtr lParam);
}
