using System;

namespace ManagedWin32.Api
{
    public delegate bool EnumResNameProc(IntPtr hModule, ResourceType ResType, IntPtr Name, IntPtr Param);
}
