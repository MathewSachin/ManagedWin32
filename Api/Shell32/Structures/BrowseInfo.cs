using System;
using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BrowseInfo
    {
        /// <summary>
        ///     Handle to the owner window for the dialog box.
        /// </summary>
        public IntPtr HwndOwner;

        /// <summary>
        ///     Pointer to an item identifier list (PIDL) specifying the
        ///     location of the root folder from which to start browsing.
        /// </summary>
        public IntPtr Root;

        /// <summary>
        ///     Address of a buffer to receive the display name of the
        ///     folder selected by the user.
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string DisplayName;

        /// <summary>
        ///     Address of a null-terminated string that is displayed
        ///     above the tree view control in the dialog box.
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string Title;

        /// <summary>
        ///     Flags specifying the options for the dialog box.
        /// </summary>
        public FolderBrowserOptions Flags;

        /// <summary>
        ///     Address of an application-defined function that the
        ///     dialog box calls when an event occurs.
        /// </summary>
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WndProc Callback;

        /// <summary>
        ///     Application-defined value that the dialog box passes to
        ///     the callback function
        /// </summary>
        public int LParam;

        /// <summary>
        ///     Variable to receive the image associated with the selected folder.
        /// </summary>
        public int Image;
    }
}