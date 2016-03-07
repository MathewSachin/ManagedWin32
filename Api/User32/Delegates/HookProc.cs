using System;

namespace ManagedWin32.Api
{
    public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
}