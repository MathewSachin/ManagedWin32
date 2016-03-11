using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWin32.Api
{
    public static class Shell32
    {
        const string DllName = "shell32.dll";

        /// <summary>
        /// Creates, updates or deletes the taskbar icon.
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern bool Shell_NotifyIcon(NotifyCommand cmd, [In] ref NotifyIconData data);

        [DllImport(DllName, EntryPoint = "#62", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SHPickIconDialog(IntPtr hWnd, StringBuilder pszFilename, int cchFilenameMax, out int pnIconIndex);

        [DllImport(DllName)]
        public static extern uint SHAppBarMessage(uint dwMessage, ref AppBarData data);

        [DllImport(DllName)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFileInfo psfi, uint cbSizeFileInfo, SHGetFileInfoFlags uFlags);

        [DllImport(DllName)]
        public static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

        // TODO: Check HandleRef
        [Obsolete("Use System.Drawing.ExtractAssociatedIcon instead.")]
        [DllImport(DllName)]
        public static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);

        [DllImport(DllName)]
        public static extern int SHGetFolderLocation(IntPtr hwndOwner, Int32 nFolder, IntPtr hToken, uint dwReserved, out IntPtr ppidl);

        [DllImport(DllName)]
        public static extern int SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string pszName, IntPtr pbc,
            out IntPtr ppidl, uint sfgaoIn, out uint psfgaoOut);

        [DllImport(DllName)]
        public static extern IntPtr SHBrowseForFolder(ref BrowseInfo lbpi);

        [DllImport(DllName)]
        public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

        [DllImport(DllName)]
        public static extern int SHGetMalloc([Out, MarshalAs(UnmanagedType.LPArray)] IMalloc[] ppMalloc);
    }
}
