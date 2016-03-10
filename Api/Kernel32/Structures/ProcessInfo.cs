using System;
using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessInfo
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int ProcessId;
        public int ThreadId;
    }
}