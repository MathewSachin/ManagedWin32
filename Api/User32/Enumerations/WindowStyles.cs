namespace ManagedWin32.Api
{
    public enum WindowStyles : uint
    {
        /// <summary>Overlapped window. An overlapped window usually has a caption and a border.</summary>
        WS_OVERLAPPED = 0x00000000,
        /// <summary>Pop-up window. Cannot be used with the WS_CHILD style.</summary>
        WS_POPUP = 0x80000000,
        /// <summary>Child window. Cannot be used with the WS_POPUP style.</summary>
        WS_CHILD = 0x40000000,
        /// <summary>Window that is initially minimized. For use with the WS_OVERLAPPED style only.</summary>
        WS_MINIMIZE = 0x20000000,
        /// <summary>Window that is initially visible.</summary>
        WS_VISIBLE = 0x10000000,
        /// <summary>Window that is initially disabled.</summary>
        WS_DISABLED = 0x08000000,
        /// <summary>
        /// Clips child windows relative to each other;
        /// that is, when a particular child window receives a paint message, the WS_CLIPSIBLINGS style
        /// clips all other overlapped child windows out of the region of the child window to be updated.
        /// (If WS_CLIPSIBLINGS is not given and child windows overlap, when you draw within the client area
        /// of a child window, it is possible to draw within the client area of a neighboring child window.)
        /// For use with the WS_CHILD style only.
        /// </summary>
        WS_CLIPSIBLINGS = 0x04000000,
        /// <summary>
        /// Excludes the area occupied by child windows when you draw within the parent window. Used when you create the parent window.</summary>
        WS_CLIPCHILDREN = 0x02000000,
        /// <summary>Window of maximum size.</summary>
        WS_MAXIMIZE = 0x01000000,
        /// <summary>Window that has a border.</summary>
        WS_BORDER = 0x00800000,
        /// <summary>Window with a double border but no title.</summary>
        WS_DLGFRAME = 0x00400000,
        /// <summary>Window that has a vertical scroll bar.</summary>
        WS_VSCROLL = 0x00200000,
        /// <summary>Window that has a horizontal scroll bar.</summary>
        WS_HSCROLL = 0x00100000,
        /// <summary>Window that has a Control-menu box in its title bar. Used only for windows with title bars.</summary>
        WS_SYSMENU = 0x00080000,
        /// <summary>Window with a thick frame that can be used to size the window.</summary>
        WS_THICKFRAME = 0x00040000,
        /// <summary>
        /// Specifies the first control of a group of controls in which the user can move from one
        /// control to the next with the arrow keys. All controls defined with the WS_GROUP style
        /// FALSE after the first control belong to the same group. The next control with the
        /// WS_GROUP style starts the next group (that is, one group ends where the next begins).
        /// </summary>
        WS_GROUP = 0x00020000,
        /// <summary>
        /// Specifies one of any number of controls through which the user can move by using the TAB key.
        /// The TAB key moves the user to the next control specified by the WS_TABSTOP style.
        /// </summary>
        WS_TABSTOP = 0x00010000,

        /// <summary>Window that has a Minimize button.</summary>
        WS_MINIMIZEBOX = 0x00020000,
        /// <summary>Window that has a Maximize button.</summary>
        WS_MAXIMIZEBOX = 0x00010000,

        /// <summary>Window that has a title bar (implies the WS_BORDER style). Cannot be used with the WS_DLGFRAME style.</summary>
        WS_CAPTION = WS_BORDER | WS_DLGFRAME,
        /// <summary>Creates an overlapped window. An overlapped window has a title bar and a border. Same as the WS_OVERLAPPED style.</summary>
        WS_TILED = WS_OVERLAPPED,
        /// <summary>Window that is initially minimized. Same as the WS_MINIMIZE style.</summary>
        WS_ICONIC = WS_MINIMIZE,
        /// <summary>Window that has a sizing border. Same as the WS_THICKFRAME style.</summary>
        WS_SIZEBOX = WS_THICKFRAME,

        /// <summary>Same as the WS_CHILD style.</summary>
        WS_CHILDWINDOW = WS_CHILD,
        /// <summary>Overlapped window with the WS_OVERLAPPED, WS_CAPTION, WS_SYSMENU, WS_THICKFRAME, WS_MINIMIZEBOX, and WS_MAXIMIZEBOX styles.</summary>
        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        /// <summary>
        /// Overlapped window with the WS_OVERLAPPED, WS_CAPTION, WS_SYSMENU, WS_THICKFRAME, WS_MINIMIZEBOX, and WS_MAXIMIZEBOX styles.
        /// Same as the WS_OVERLAPPEDWINDOW style.
        /// </summary>
        WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
        /// <summary>
        /// Pop-up window with the WS_BORDER, WS_POPUP, and WS_SYSMENU styles.
        /// The WS_CAPTION style must be combined with the WS_POPUPWINDOW style to make the Control menu visible.
        /// </summary>
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

        #region Extended Window Styles
        // http://msdn2.microsoft.com/en-us/library/ms632680.aspx

        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_ACCEPTFILES = 0x00000010,
        WS_EX_TRANSPARENT = 0x00000020,

        //// Only with WINVER >= 0x400

        WS_EX_MDICHILD = 0x00000040,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_WINDOWEDGE = 0x00000100,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_CONTEXTHELP = 0x00000400,

        WS_EX_RIGHT = 0x00001000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,

        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_APPWINDOW = 0x00040000,

        WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
        WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),

        //// Only with WINVER >= 0x500
        /// <summary>Disable inheritence of mirroring by children</summary>
        WS_EX_NOINHERITLAYOUT = 0x00100000,
        /// <summary>Right to left mirroring</summary>
        WS_EX_LAYOUTRTL = 0x00400000,

        // Only with _WIN32_WINNT >= 0x500
        WS_EX_LAYERED = 0x00080000,
        WS_EX_COMPOSITED = 0x02000000,
        WS_EX_NOACTIVATE = 0x08000000
        #endregion
    }
}