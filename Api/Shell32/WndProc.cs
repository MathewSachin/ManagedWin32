using System;

namespace ManagedWin32.Api
{
    public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}