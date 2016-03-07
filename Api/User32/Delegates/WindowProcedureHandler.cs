using System;

namespace ManagedWin32.Api
{
    /// <summary>
    /// Callback delegate which is used by the Windows API to submit window messages.
    /// </summary>
    public delegate IntPtr WindowProcedureHandler(IntPtr hwnd, uint uMsg, IntPtr wparam, IntPtr lparam);
}