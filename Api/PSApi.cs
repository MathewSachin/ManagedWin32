using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWin32.Api
{
    public static class PSApi
    {
        const string DllName = "psapi.dll";

        [DllImport(DllName)]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, uint nSize);

        [DllImport(DllName)]
        public static extern uint GetProcessImageFileName(IntPtr hProcess, StringBuilder lpImageFileName, uint nSize);

        [DllImport(DllName)]
        public static extern int EmptyWorkingSet(IntPtr hwProc);
    }
}
