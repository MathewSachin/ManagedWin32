using System;
using ManagedWin32.Api;

namespace ManagedWin32
{
    public static class ExitWindows
    {
        public static void Exit(ShutdownFlags Flags = ShutdownFlags.Shutdown, int Reason = 0)
        {
            TokPriv1Luid tp = new TokPriv1Luid(1, 0, AdvApi32.SE_PRIVILEGE_ENABLED);
            IntPtr hproc = Kernel32.CurrentProcess, htok = IntPtr.Zero;

            AdvApi32.OpenProcessToken(hproc, AdvApi32.TOKEN_ADJUST_PRIVILEGES | AdvApi32.TOKEN_QUERY, ref htok);
            AdvApi32.LookupPrivilegeValue(null, "SeShutdownPrivilege", ref tp.Luid);
            AdvApi32.AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);

            User32.ExitWindows(Flags, 0);
        }
    }
}