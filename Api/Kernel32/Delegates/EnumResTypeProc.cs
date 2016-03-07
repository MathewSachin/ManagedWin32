using System;

namespace ManagedWin32.Api
{
    public delegate int EnumResTypeProc(IntPtr hModule, IntPtr lpszType, IntPtr lParam);
}
