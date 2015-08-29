using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWin32.Api
{
    public static class Shell32
    {
        /// <summary>
        /// Creates, updates or deletes the taskbar icon.
        /// </summary>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern bool Shell_NotifyIcon(NotifyCommand cmd, [In] ref NotifyIconData data);

        [DllImport("shell32.dll")]
        public static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA data);

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, SHGetFileInfoFlags uFlags);

        [DllImport("shell32")]
        public static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

        [DllImport("shell32", CharSet = CharSet.Auto)]
        public static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);
    }
}
