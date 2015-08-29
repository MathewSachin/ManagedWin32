using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWin32.Api
{
    public static class PSApi
    {
        [DllImport("psapi", SetLastError = true)]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, uint nSize);

        [DllImport("psapi", SetLastError = true)]
        public static extern uint GetProcessImageFileName(IntPtr hProcess, StringBuilder lpImageFileName, uint nSize);

        [DllImport("psapi")]
        public static extern int EmptyWorkingSet(IntPtr hwProc);
    }
}
