using System;

// TODO: Split this file
namespace ManagedWin32.Api
{
    [Flags]
    public enum AnimateWindowFlags
    {
        HOR_POSITIVE = 0x00000001,
        HOR_NEGATIVE = 0x00000002,
        VER_POSITIVE = 0x00000004,
        VER_NEGATIVE = 0x00000008,
        CENTER = 0x00000010,
        HIDE = 0x00010000,
        ACTIVATE = 0x00020000,
        SLIDE = 0x00040000,
        BLEND = 0x00080000
    }

    [Flags]
    public enum WindowPlacementFlags : uint
    {
        // The coordinates of the minimized window may be specified.
        // This flag must be specified if the coordinates are set in the ptMinPosition member.
        WPF_SETMINPOSITION = 0x0001,
        // If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
        WPF_ASYNCWINDOWPLACEMENT = 0x0004,
        // The restored window will be maximized, regardless of whether it was maximized before it was minimized. This setting is only valid the next time the window is restored. It does not change the default restoration behavior.
        // This flag is only valid when the SW_SHOWMINIMIZED value is specified for the showCmd member.
        WPF_RESTORETOMAXIMIZED = 0x0002
    }
    
    public enum ShowWindowFlags : uint
    {
        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        Hide = 0,
        /// <summary>
        /// Activates and displays a window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when displaying the window
        /// for the first time.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        ShowMinimized = 2,
        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        Maximize = 3, // is this the right value?
        /// <summary>
        /// Activates the window and displays it as a maximized window.
        /// </summary>
        ShowMaximized = 3,
        /// <summary>
        /// Displays a window in its most recent size and position. This value
        /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except
        /// the window is not actived.
        /// </summary>
        ShowNoActivate = 4,
        /// <summary>
        /// Activates the window and displays it in its current size and position.
        /// </summary>
        Show = 5,
        /// <summary>
        /// Minimizes the specified window and activates the next top-level
        /// window in the Z order.
        /// </summary>
        Minimize = 6,
        /// <summary>
        /// Displays the window as a minimized window. This value is similar to
        /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the
        /// window is not activated.
        /// </summary>
        ShowMinNoActive = 7,
        /// <summary>
        /// Displays the window in its current size and position. This value is
        /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the
        /// window is not activated.
        /// </summary>
        ShowNA = 8,
        /// <summary>
        /// Activates and displays the window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when restoring a minimized window.
        /// </summary>
        Restore = 9,
        /// <summary>
        /// Sets the show state based on the SW_* value specified in the
        /// STARTUPINFO structure passed to the CreateProcess function by the
        /// program that started the application.
        /// </summary>
        ShowDefault = 10,
        /// <summary>
        ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
        /// that owns the window is not responding. This flag should only be
        /// used when minimizing windows from a different thread.
        /// </summary>
        ForceMinimize = 11
    }

    public enum EventObjects : int
    {
        OBJID_ALERT = -10,
        OBJID_CARET = -8,
        OBJID_CLIENT = -4,
        OBJID_CURSOR = -9,
        OBJID_HSCROLL = -6,
        OBJID_MENU = -3,
        OBJID_SIZEGRIP = -7,
        OBJID_SOUND = -11,
        OBJID_SYSMENU = -1,
        OBJID_TITLEBAR = -2,
        OBJID_VSCROLL = -5,
        OBJID_WINDOW = 0
    }

    public enum WinEventHookFlags : int
    {
        WINEVENT_SKIPOWNTHREAD = 1,
        WINEVENT_SKIPOWNPROCESS = 2,
        WINEVENT_OUTOFCONTEXT = 0,
        WINEVENT_INCONTEXT = 4
    }

    public enum WinEvent : uint
    {
        EVENT_OBJECT_ACCELERATORCHANGE = 32786,
        EVENT_OBJECT_CREATE = 32768,
        EVENT_OBJECT_DESTROY = 32769,
        EVENT_OBJECT_DEFACTIONCHANGE = 32785,
        EVENT_OBJECT_DESCRIPTIONCHANGE = 32781,
        EVENT_OBJECT_FOCUS = 32773,
        EVENT_OBJECT_HELPCHANGE = 32784,
        EVENT_OBJECT_SHOW = 32770,
        EVENT_OBJECT_HIDE = 32771,
        EVENT_OBJECT_LOCATIONCHANGE = 32779,
        EVENT_OBJECT_NAMECHANGE = 32780,
        EVENT_OBJECT_PARENTCHANGE = 32783,
        EVENT_OBJECT_REORDER = 32772,
        EVENT_OBJECT_SELECTION = 32774,
        EVENT_OBJECT_SELECTIONADD = 32775,
        EVENT_OBJECT_SELECTIONREMOVE = 32776,
        EVENT_OBJECT_SELECTIONWITHIN = 32777,
        EVENT_OBJECT_STATECHANGE = 32778,
        EVENT_OBJECT_VALUECHANGE = 32782,
        EVENT_SYSTEM_ALERT = 2,
        EVENT_SYSTEM_CAPTUREEND = 9,
        EVENT_SYSTEM_CAPTURESTART = 8,
        EVENT_SYSTEM_CONTEXTHELPEND = 13,
        EVENT_SYSTEM_CONTEXTHELPSTART = 12,
        EVENT_SYSTEM_DIALOGEND = 17,
        EVENT_SYSTEM_DIALOGSTART = 16,
        EVENT_SYSTEM_DRAGDROPEND = 15,
        EVENT_SYSTEM_DRAGDROPSTART = 14,
        EVENT_SYSTEM_FOREGROUND = 3,
        EVENT_SYSTEM_MENUEND = 5,
        EVENT_SYSTEM_MENUPOPUPEND = 7,
        EVENT_SYSTEM_MENUPOPUPSTART = 6,
        EVENT_SYSTEM_MENUSTART = 4,
        EVENT_SYSTEM_MINIMIZEEND = 23,
        EVENT_SYSTEM_MINIMIZESTART = 22,
        EVENT_SYSTEM_MOVESIZEEND = 11,
        EVENT_SYSTEM_MOVESIZESTART = 10,
        EVENT_SYSTEM_SCROLLINGEND = 19,
        EVENT_SYSTEM_SCROLLINGSTART = 18,
        EVENT_SYSTEM_SOUND = 1,
        EVENT_SYSTEM_SWITCHEND = 21,
        EVENT_SYSTEM_SWITCHSTART = 20
    }

    public enum Orientation
    {
        Horizontal = 0,
        Vertical = 1
    }

    public enum ScrollBarDirection : int
    {
        SB_HORZ = 0,
        SB_VERT = 1,
        SB_CTL = 2,
        SB_BOTH = 3
    }

    public enum RegionResult
    {
        REGION_ERROR = 0,
        REGION_NULLREGION = 1,
        REGION_SIMPLEREGION = 2,
        REGION_COMPLEXREGION = 3
    }

    [Flags]
    public enum SendMessageTimeoutFlags : uint
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8
    }

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

    public enum GetWindowLongValue
    {
        /// <summary>Retrieves the address of the window procedure, or a handle representing the address of the window procedure.</summary>
        GWL_WNDPROC = -4,
        /// <summary>Retrieves a handle to the application instance.</summary>
        GWL_HINSTANCE = -6,
        /// <summary>Retrieves a handle to the parent window, if any.</summary>
        GWL_HWNDPARENT = -8,
        /// <summary>Retrieves the window styles.</summary>
        GWL_STYLE = -16,
        /// <summary>Retrieves the extended window styles.</summary>
        GWL_EXSTYLE = -20,
        /// <summary>
        /// Retrieves the user data associated with the window.
        /// This data is intended for use by the application that created the window. Its value is initially zero.
        /// </summary>
        GWL_USERDATA = -21,
        /// <summary>Retrieves the identifier of the window.</summary>
        GWL_ID = -12
    }

    public enum SystemInfoParamsAction
    {
        GetScreenSaverActive = 16,
        SetScreenSaverActive = 17,
        GetScreenSaverTimeout = 14,
        SetScreenSaverTimeout = 15,
        GetScreenSaverRunning = 114,

        GETFONTSMOOTHING = 74,

        GETDROPSHADOW = 0x1024,

        GETFLATMENU = 0x1022,

        GETFONTSMOOTHINGTYPE = 0x200a,

        GETFONTSMOOTHINGCONTRAST = 0x200c,

        ICONHORIZONTALSPACING = 13,

        ICONVERTICALSPACING = 24,

        GETICONTITLEWRAP = 25,

        GETICONTITLELOGFONT = 31,

        GETKEYBOARDCUES = 0x100a,

        GETKEYBOARDDELAY = 22,

        GETKEYBOARDPREF = 68,

        GETKEYBOARDSPEED = 10,

        GETMOUSEHOVERWIDTH = 98,

        GETMOUSEHOVERHEIGHT = 100,

        GETMOUSEHOVERTIME = 102,

        GETMOUSESPEED = 112,

        GETMENUDROPALIGNMENT = 27,

        GETMENUFADE = 0x1012,

        GETMENUSHOWDELAY = 106,

        GETCOMBOBOXANIMATION = 0x1004,

        GETGRADIENTCAPTIONS = 0x1008,

        GETHOTTRACKING = 0x100e,

        GETLISTBOXSMOOTHSCROLLING = 0x1006,

        GETMENUANIMATION = 0x1002,

        GETSELECTIONFADE = 0x1014,

        GETTOOLTIPANIMATION = 0x1016,

        GETUIEFFECTS = 0x103e,

        GETACTIVEWINDOWTRACKING = 0x1000,

        GETACTIVEWNDTRKTIMEOUT = 0x2002,

        GETANIMATION = 72,

        GETBORDER = 5,

        GETCARETWIDTH = 0x2006,

        GETDRAGFULLWINDOWS = 38,

        GETNONCLIENTMETRICS = 41,

        GETWORKAREA = 48,

        GETHIGHCONTRAST = 66,

        GETDEFAULTINPUTLANG = 89,

        GETSNAPTODEFBUTTON = 95,

        GETWHEELSCROLLLINES = 104
    }
    
    /// <summary>Specifies the boot mode in which the system was started.</summary>
    public enum BootMode
    {
        /// <summary>The computer was started in the standard boot mode. This mode uses the normal drivers and settings for the system.</summary>
        Normal,
        /// <summary>The computer was started in safe mode without network support. This mode uses a limited drivers and settings profile.</summary>
        FailSafe,
        /// <summary>The computer was started in safe mode with network support. This mode uses a limited drivers and settings profile, and loads the services needed to start networking.</summary>
        FailSafeWithNetwork
    }

    public enum BeepType
    {
        Beep = 0,
        Hand = 16,
        Question = 32,
        Exclamation = 48,
        Asterisk = 64
    }

    public enum SystemMetrics
    {
        CXSCREEN = 0,
        CYSCREEN = 1,

        CXVSCROLL = 2,
        CYHSCROLL = 3,

        CYCAPTION = 4,

        CXBORDER = 5,
        CYBORDER = 6,

        CYVTHUMB = 9,
        CXHTHUMB = 10,

        CXICON = 11,
        CYICON = 12,

        CXCURSOR = 13,
        CYCURSOR = 14,

        CYMENU = 15,

        CYKANJIWINDOW = 18,

        MOUSEPRESENT = 19,

        CYVSCROLL = 20,
        CXHSCROLL = 21,

        DEBUG = 22,

        SWAPBUTTON = 23,

        CXMIN = 28,

        CYMIN = 29,

        CXSIZE = 30,
        CYSIZE = 31,

        CXFRAME = 32,
        CYFRAME = 33,

        CYFOCUSBORDER = 84,
        CXFOCUSBORDER = 83,

        CYSIZEFRAME = 33,
        CXSIZEFRAME = 32,

        CXMINTRACK = 34,
        CYMINTRACK = 35,

        CXDOUBLECLK = 36,
        CYDOUBLECLK = 37,

        CXICONSPACING = 38,

        CYICONSPACING = 39,

        MENUDROPALIGNMENT = 40,

        PENWINDOWS = 41,

        DBCSENABLED = 42,

        CMOUSEBUTTONS = 43,

        CXFIXEDFRAME = 7,

        CYFIXEDFRAME = 8,

        SECURE = 44,

        CXEDGE = 45,
        CYEDGE = 46,

        CXMINSPACING = 47,
        CYMINSPACING = 48,

        CXSMICON = 49,
        CYSMICON = 50,

        CYSMCAPTION = 51,

        CXSMSIZE = 52,

        CYSMSIZE = 53,

        CXMENUSIZE = 54,

        CYMENUSIZE = 55,

        ARRANGE = 56,

        CXMINIMIZED = 57,

        CYMINIMIZED = 58,

        CXMAXTRACK = 59,

        CYMAXTRACK = 60,

        CXMAXIMIZED = 61,

        CYMAXIMIZED = 62,

        NETWORK = 63,

        CLEANBOOT = 67,

        CXDRAG = 68,
        CYDRAG = 69,

        SHOWSOUNDS = 70,

        CXMENUCHECK = 71,
        CYMENUCHECK = 72,

        MIDEASTENABLED = 74,

        MOUSEWHEELPRESENT = 75,

        XVIRTUALSCREEN = 76,
        YVIRTUALSCREEN = 77,

        CXVIRTUALSCREEN = 78,
        CYVIRTUALSCREEN = 79,

        CMONITORS = 80,

        SAMEDISPLAYFORMAT = 81,

        REMOTESESSION = 0x1000
    }

    public enum DesktopAccessRights
    {
        CreateWindow = 0x0002,
        Enumerate = 0x0040,
        WriteObjects = 0x0080,
        SwitchDesktop = 0x0100,
        CreateMenu = 0x0004,
        HookControl = 0x0008,
        ReadObjects = 0x0001,
        JournalRecord = 0x0010,
        JournalPlayback = 0x0020,
        AllRights = JournalRecord | JournalPlayback | CreateWindow
            | Enumerate | WriteObjects | SwitchDesktop
            | CreateMenu | HookControl | ReadObjects
    }

    /// <summary>
    /// Modifier Key Codes
    /// </summary>
    [Flags]
    public enum ModifierKeyCodes : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }
    
    public enum KeyCode
    {
        /// <summary>
        /// Left mouse button
        /// </summary>
        LBUTTON = 0x01,

        /// <summary>
        /// Right mouse button
        /// </summary>
        RBUTTON = 0x02,

        /// <summary>
        /// Control-break processing
        /// </summary>
        CANCEL = 0x03,

        /// <summary>
        /// Middle mouse button (three-button mouse) - NOT contiguous with LBUTTON and RBUTTON
        /// </summary>
        MBUTTON = 0x04,

        /// <summary>
        /// Windows 2000/XP: X1 mouse button - NOT contiguous with LBUTTON and RBUTTON
        /// </summary>
        XBUTTON1 = 0x05,

        /// <summary>
        /// Windows 2000/XP: X2 mouse button - NOT contiguous with LBUTTON and RBUTTON
        /// </summary>
        XBUTTON2 = 0x06,

        // 0x07 : Undefined

        /// <summary>
        /// BACKSPACE key
        /// </summary>
        BACK = 0x08,

        /// <summary>
        /// TAB key
        /// </summary>
        TAB = 0x09,

        // 0x0A - 0x0B : Reserved

        /// <summary>
        /// CLEAR key
        /// </summary>
        CLEAR = 0x0C,

        /// <summary>
        /// ENTER key
        /// </summary>
        RETURN = 0x0D,

        // 0x0E - 0x0F : Undefined

        /// <summary>
        /// SHIFT key
        /// </summary>
        SHIFT = 0x10,

        /// <summary>
        /// CTRL key
        /// </summary>
        CONTROL = 0x11,

        /// <summary>
        /// ALT key
        /// </summary>
        MENU = 0x12,

        /// <summary>
        /// PAUSE key
        /// </summary>
        PAUSE = 0x13,

        /// <summary>
        /// CAPS LOCK key
        /// </summary>
        CAPITAL = 0x14,

        /// <summary>
        /// Input Method Editor (IME) Kana mode
        /// </summary>
        KANA = 0x15,

        /// <summary>
        /// IME Hanguel mode (maintained for compatibility; use HANGUL)
        /// </summary>
        HANGEUL = 0x15,

        /// <summary>
        /// IME Hangul mode
        /// </summary>
        HANGUL = 0x15,

        // 0x16 : Undefined

        /// <summary>
        /// IME Junja mode
        /// </summary>
        JUNJA = 0x17,

        /// <summary>
        /// IME final mode
        /// </summary>
        FINAL = 0x18,

        /// <summary>
        /// IME Hanja mode
        /// </summary>
        HANJA = 0x19,

        /// <summary>
        /// IME Kanji mode
        /// </summary>
        KANJI = 0x19,

        // 0x1A : Undefined

        /// <summary>
        /// ESC key
        /// </summary>
        ESCAPE = 0x1B,

        /// <summary>
        /// IME convert
        /// </summary>
        CONVERT = 0x1C,

        /// <summary>
        /// IME nonconvert
        /// </summary>
        NONCONVERT = 0x1D,

        /// <summary>
        /// IME accept
        /// </summary>
        ACCEPT = 0x1E,

        /// <summary>
        /// IME mode change request
        /// </summary>
        MODECHANGE = 0x1F,

        /// <summary>
        /// SPACEBAR
        /// </summary>
        SPACE = 0x20,

        /// <summary>
        /// PAGE UP key
        /// </summary>
        PRIOR = 0x21,

        /// <summary>
        /// PAGE DOWN key
        /// </summary>
        NEXT = 0x22,

        /// <summary>
        /// END key
        /// </summary>
        END = 0x23,

        /// <summary>
        /// HOME key
        /// </summary>
        HOME = 0x24,

        /// <summary>
        /// LEFT ARROW key
        /// </summary>
        LEFT = 0x25,

        /// <summary>
        /// UP ARROW key
        /// </summary>
        UP = 0x26,

        /// <summary>
        /// RIGHT ARROW key
        /// </summary>
        RIGHT = 0x27,

        /// <summary>
        /// DOWN ARROW key
        /// </summary>
        DOWN = 0x28,

        /// <summary>
        /// SELECT key
        /// </summary>
        SELECT = 0x29,

        /// <summary>
        /// PRINT key
        /// </summary>
        PRINT = 0x2A,

        /// <summary>
        /// EXECUTE key
        /// </summary>
        EXECUTE = 0x2B,

        /// <summary>
        /// PRINT SCREEN key
        /// </summary>
        SNAPSHOT = 0x2C,

        /// <summary>
        /// INS key
        /// </summary>
        INSERT = 0x2D,

        /// <summary>
        /// DEL key
        /// </summary>
        DELETE = 0x2E,

        /// <summary>
        /// HELP key
        /// </summary>
        HELP = 0x2F,

        /// <summary>
        /// 0 key
        /// </summary>
        VK_0 = 0x30,

        /// <summary>
        /// 1 key
        /// </summary>
        VK_1 = 0x31,

        /// <summary>
        /// 2 key
        /// </summary>
        VK_2 = 0x32,

        /// <summary>
        /// 3 key
        /// </summary>
        VK_3 = 0x33,

        /// <summary>
        /// 4 key
        /// </summary>
        VK_4 = 0x34,

        /// <summary>
        /// 5 key
        /// </summary>
        VK_5 = 0x35,

        /// <summary>
        /// 6 key
        /// </summary>
        VK_6 = 0x36,

        /// <summary>
        /// 7 key
        /// </summary>
        VK_7 = 0x37,

        /// <summary>
        /// 8 key
        /// </summary>
        VK_8 = 0x38,

        /// <summary>
        /// 9 key
        /// </summary>
        VK_9 = 0x39,

        //
        // 0x3A - 0x40 : Udefined
        //

        /// <summary>
        /// A key
        /// </summary>
        VK_A = 0x41,

        /// <summary>
        /// B key
        /// </summary>
        VK_B = 0x42,

        /// <summary>
        /// C key
        /// </summary>
        VK_C = 0x43,

        /// <summary>
        /// D key
        /// </summary>
        VK_D = 0x44,

        /// <summary>
        /// E key
        /// </summary>
        VK_E = 0x45,

        /// <summary>
        /// F key
        /// </summary>
        VK_F = 0x46,

        /// <summary>
        /// G key
        /// </summary>
        VK_G = 0x47,

        /// <summary>
        /// H key
        /// </summary>
        VK_H = 0x48,

        /// <summary>
        /// I key
        /// </summary>
        VK_I = 0x49,

        /// <summary>
        /// J key
        /// </summary>
        VK_J = 0x4A,

        /// <summary>
        /// K key
        /// </summary>
        VK_K = 0x4B,

        /// <summary>
        /// L key
        /// </summary>
        VK_L = 0x4C,

        /// <summary>
        /// M key
        /// </summary>
        VK_M = 0x4D,

        /// <summary>
        /// N key
        /// </summary>
        VK_N = 0x4E,

        /// <summary>
        /// O key
        /// </summary>
        VK_O = 0x4F,

        /// <summary>
        /// P key
        /// </summary>
        VK_P = 0x50,

        /// <summary>
        /// Q key
        /// </summary>
        VK_Q = 0x51,

        /// <summary>
        /// R key
        /// </summary>
        VK_R = 0x52,

        /// <summary>
        /// S key
        /// </summary>
        VK_S = 0x53,

        /// <summary>
        /// T key
        /// </summary>
        VK_T = 0x54,

        /// <summary>
        /// U key
        /// </summary>
        VK_U = 0x55,

        /// <summary>
        /// V key
        /// </summary>
        VK_V = 0x56,

        /// <summary>
        /// W key
        /// </summary>
        VK_W = 0x57,

        /// <summary>
        /// X key
        /// </summary>
        VK_X = 0x58,

        /// <summary>
        /// Y key
        /// </summary>
        VK_Y = 0x59,

        /// <summary>
        /// Z key
        /// </summary>
        VK_Z = 0x5A,

        /// <summary>
        /// Left Windows key (Microsoft Natural keyboard)
        /// </summary>
        LWIN = 0x5B,

        /// <summary>
        /// Right Windows key (Natural keyboard)
        /// </summary>
        RWIN = 0x5C,

        /// <summary>
        /// Applications key (Natural keyboard)
        /// </summary>
        APPS = 0x5D,

        // 0x5E : reserved

        /// <summary>
        /// Computer Sleep key
        /// </summary>
        SLEEP = 0x5F,

        /// <summary>
        /// Numeric keypad 0 key
        /// </summary>
        NUMPAD0 = 0x60,

        /// <summary>
        /// Numeric keypad 1 key
        /// </summary>
        NUMPAD1 = 0x61,

        /// <summary>
        /// Numeric keypad 2 key
        /// </summary>
        NUMPAD2 = 0x62,

        /// <summary>
        /// Numeric keypad 3 key
        /// </summary>
        NUMPAD3 = 0x63,

        /// <summary>
        /// Numeric keypad 4 key
        /// </summary>
        NUMPAD4 = 0x64,

        /// <summary>
        /// Numeric keypad 5 key
        /// </summary>
        NUMPAD5 = 0x65,

        /// <summary>
        /// Numeric keypad 6 key
        /// </summary>
        NUMPAD6 = 0x66,

        /// <summary>
        /// Numeric keypad 7 key
        /// </summary>
        NUMPAD7 = 0x67,

        /// <summary>
        /// Numeric keypad 8 key
        /// </summary>
        NUMPAD8 = 0x68,

        /// <summary>
        /// Numeric keypad 9 key
        /// </summary>
        NUMPAD9 = 0x69,

        /// <summary>
        /// Multiply key
        /// </summary>
        MULTIPLY = 0x6A,

        /// <summary>
        /// Add key
        /// </summary>
        ADD = 0x6B,

        /// <summary>
        /// Separator key
        /// </summary>
        SEPARATOR = 0x6C,

        /// <summary>
        /// Subtract key
        /// </summary>
        SUBTRACT = 0x6D,

        /// <summary>
        /// Decimal key
        /// </summary>
        DECIMAL = 0x6E,

        /// <summary>
        /// Divide key
        /// </summary>
        DIVIDE = 0x6F,

        /// <summary>
        /// F1 key
        /// </summary>
        F1 = 0x70,

        /// <summary>
        /// F2 key
        /// </summary>
        F2 = 0x71,

        /// <summary>
        /// F3 key
        /// </summary>
        F3 = 0x72,

        /// <summary>
        /// F4 key
        /// </summary>
        F4 = 0x73,

        /// <summary>
        /// F5 key
        /// </summary>
        F5 = 0x74,

        /// <summary>
        /// F6 key
        /// </summary>
        F6 = 0x75,

        /// <summary>
        /// F7 key
        /// </summary>
        F7 = 0x76,

        /// <summary>
        /// F8 key
        /// </summary>
        F8 = 0x77,

        /// <summary>
        /// F9 key
        /// </summary>
        F9 = 0x78,

        /// <summary>
        /// F10 key
        /// </summary>
        F10 = 0x79,

        /// <summary>
        /// F11 key
        /// </summary>
        F11 = 0x7A,

        /// <summary>
        /// F12 key
        /// </summary>
        F12 = 0x7B,

        /// <summary>
        /// F13 key
        /// </summary>
        F13 = 0x7C,

        /// <summary>
        /// F14 key
        /// </summary>
        F14 = 0x7D,

        /// <summary>
        /// F15 key
        /// </summary>
        F15 = 0x7E,

        /// <summary>
        /// F16 key
        /// </summary>
        F16 = 0x7F,

        /// <summary>
        /// F17 key
        /// </summary>
        F17 = 0x80,

        /// <summary>
        /// F18 key
        /// </summary>
        F18 = 0x81,

        /// <summary>
        /// F19 key
        /// </summary>
        F19 = 0x82,

        /// <summary>
        /// F20 key
        /// </summary>
        F20 = 0x83,

        /// <summary>
        /// F21 key
        /// </summary>
        F21 = 0x84,

        /// <summary>
        /// F22 key
        /// </summary>
        F22 = 0x85,

        /// <summary>
        /// F23 key
        /// </summary>
        F23 = 0x86,

        /// <summary>
        /// F24 key
        /// </summary>
        F24 = 0x87,

        //
        // 0x88 - 0x8F : Unassigned
        //

        /// <summary>
        /// NUM LOCK key
        /// </summary>
        NUMLOCK = 0x90,

        /// <summary>
        /// SCROLL LOCK key
        /// </summary>
        SCROLL = 0x91,

        // 0x92 - 0x96 : OEM Specific

        // 0x97 - 0x9F : Unassigned

        //
        // L* & R* - left and right Alt, Ctrl and Shift virtual keys.
        // Used only as parameters to GetAsyncKeyState() and GetKeyState().
        // No other API or message will distinguish left and right keys in this way.
        //

        /// <summary>
        /// Left SHIFT key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        LSHIFT = 0xA0,

        /// <summary>
        /// Right SHIFT key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        RSHIFT = 0xA1,

        /// <summary>
        /// Left CONTROL key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        LCONTROL = 0xA2,

        /// <summary>
        /// Right CONTROL key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        RCONTROL = 0xA3,

        /// <summary>
        /// Left MENU key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        LMENU = 0xA4,

        /// <summary>
        /// Right MENU key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
        /// </summary>
        RMENU = 0xA5,

        /// <summary>
        /// Windows 2000/XP: Browser Back key
        /// </summary>
        BROWSER_BACK = 0xA6,

        /// <summary>
        /// Windows 2000/XP: Browser Forward key
        /// </summary>
        BROWSER_FORWARD = 0xA7,

        /// <summary>
        /// Windows 2000/XP: Browser Refresh key
        /// </summary>
        BROWSER_REFRESH = 0xA8,

        /// <summary>
        /// Windows 2000/XP: Browser Stop key
        /// </summary>
        BROWSER_STOP = 0xA9,

        /// <summary>
        /// Windows 2000/XP: Browser Search key
        /// </summary>
        BROWSER_SEARCH = 0xAA,

        /// <summary>
        /// Windows 2000/XP: Browser Favorites key
        /// </summary>
        BROWSER_FAVORITES = 0xAB,

        /// <summary>
        /// Windows 2000/XP: Browser Start and Home key
        /// </summary>
        BROWSER_HOME = 0xAC,

        /// <summary>
        /// Windows 2000/XP: Volume Mute key
        /// </summary>
        VOLUME_MUTE = 0xAD,

        /// <summary>
        /// Windows 2000/XP: Volume Down key
        /// </summary>
        VOLUME_DOWN = 0xAE,

        /// <summary>
        /// Windows 2000/XP: Volume Up key
        /// </summary>
        VOLUME_UP = 0xAF,

        /// <summary>
        /// Windows 2000/XP: Next Track key
        /// </summary>
        MEDIA_NEXT_TRACK = 0xB0,

        /// <summary>
        /// Windows 2000/XP: Previous Track key
        /// </summary>
        MEDIA_PREV_TRACK = 0xB1,

        /// <summary>
        /// Windows 2000/XP: Stop Media key
        /// </summary>
        MEDIA_STOP = 0xB2,

        /// <summary>
        /// Windows 2000/XP: Play/Pause Media key
        /// </summary>
        MEDIA_PLAY_PAUSE = 0xB3,

        /// <summary>
        /// Windows 2000/XP: Start Mail key
        /// </summary>
        LAUNCH_MAIL = 0xB4,

        /// <summary>
        /// Windows 2000/XP: Select Media key
        /// </summary>
        LAUNCH_MEDIA_SELECT = 0xB5,

        /// <summary>
        /// Windows 2000/XP: Start Application 1 key
        /// </summary>
        LAUNCH_APP1 = 0xB6,

        /// <summary>
        /// Windows 2000/XP: Start Application 2 key
        /// </summary>
        LAUNCH_APP2 = 0xB7,

        //
        // 0xB8 - 0xB9 : Reserved
        //

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the ';:' key 
        /// </summary>
        OEM_1 = 0xBA,

        /// <summary>
        /// Windows 2000/XP: For any country/region, the '+' key
        /// </summary>
        OEM_PLUS = 0xBB,

        /// <summary>
        /// Windows 2000/XP: For any country/region, the ',' key
        /// </summary>
        OEM_COMMA = 0xBC,

        /// <summary>
        /// Windows 2000/XP: For any country/region, the '-' key
        /// </summary>
        OEM_MINUS = 0xBD,

        /// <summary>
        /// Windows 2000/XP: For any country/region, the '.' key
        /// </summary>
        OEM_PERIOD = 0xBE,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '/?' key 
        /// </summary>
        OEM_2 = 0xBF,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '`~' key 
        /// </summary>
        OEM_3 = 0xC0,

        //
        // 0xC1 - 0xD7 : Reserved
        //

        //
        // 0xD8 - 0xDA : Unassigned
        //

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '[{' key
        /// </summary>
        OEM_4 = 0xDB,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '\|' key
        /// </summary>
        OEM_5 = 0xDC,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the ']}' key
        /// </summary>
        OEM_6 = 0xDD,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the 'single-quote/double-quote' key
        /// </summary>
        OEM_7 = 0xDE,

        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
        OEM_8 = 0xDF,

        //
        // 0xE0 : Reserved
        //

        //
        // 0xE1 : OEM Specific
        //

        /// <summary>
        /// Windows 2000/XP: Either the angle bracket key or the backslash key on the RT 102-key keyboard
        /// </summary>
        OEM_102 = 0xE2,

        //
        // (0xE3-E4) : OEM specific
        //

        /// <summary>
        /// Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
        /// </summary>
        PROCESSKEY = 0xE5,

        //
        // 0xE6 : OEM specific
        //

        /// <summary>
        /// Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes. The PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
        /// </summary>
        PACKET = 0xE7,

        //
        // 0xE8 : Unassigned
        //

        //
        // 0xE9-F5 : OEM specific
        //

        /// <summary>
        /// Attn key
        /// </summary>
        ATTN = 0xF6,

        /// <summary>
        /// CrSel key
        /// </summary>
        CRSEL = 0xF7,

        /// <summary>
        /// ExSel key
        /// </summary>
        EXSEL = 0xF8,

        /// <summary>
        /// Erase EOF key
        /// </summary>
        EREOF = 0xF9,

        /// <summary>
        /// Play key
        /// </summary>
        PLAY = 0xFA,

        /// <summary>
        /// Zoom key
        /// </summary>
        ZOOM = 0xFB,

        /// <summary>
        /// Reserved
        /// </summary>
        NONAME = 0xFC,

        /// <summary>
        /// PA1 key
        /// </summary>
        PA1 = 0xFD,

        /// <summary>
        /// Clear key
        /// </summary>
        OEM_CLEAR = 0xFE,
    }

    [Flags]
    public enum ShutdownFlags
    {
        //Shuts down all processes running in the logon session of the process that called the ExitWindowsEx function. 
        //Then it logs the user off.
        //This flag can be used only by processes running in an interactive user's logon session.
        LogOff = 0x00000000,

        //Shuts down the system to a point at which it is safe to turn off the power. 
        //All file buffers have been flushed to disk, and all running processes have stopped. 
        Shutdown = 0x00000001,

        //Shuts down the system and then restarts the system. 
        Reboot = 0x00000002,

        //This flag has no effect if terminal services is enabled. 
        //Otherwise, the system does not send the WM_QUERYENDSESSION message. 
        //This can cause applications to lose data. 
        //Therefore, you should only use this flag in an emergency.
        Force = 0x00000004,

        //Shuts down the system and turns off the power. The system must support the power-off feature. 
        PowerOff = 0x00000008,

        //Forces processes to terminate if they do not respond to the WM_QUERYENDSESSION or WM_ENDSESSION message within the timeout interval.
        ForceIfHung = 0x00000010,

        //Shuts down the system and then restarts it, 
        //as well as any applications that have been registered for restart 
        //using the RegisterApplicationRestart function.
        //These application receive the WM_QUERYENDSESSION message with lParam 
        //set to the ENDSESSION_CLOSEAPP value.
        RestartApps = 0x00000040,

        //Beginning with Windows 8:  You can prepare the system for a faster startup 
        //by combining the EWX_HYBRID_SHUTDOWN flag with the EWX_SHUTDOWN flag. 
        HybridShutdown = 0x00400000
    }

    public enum GetWindowEnum
    {
        First = 0,
        Last = 1,
        Next = 2,
        Previous = 3,
        Owner = 4,
        Child = 5,
        EnabledPopup = 6
    }

    public enum SetWindowPositionFlags
    {
        NoMove = 0x2,
        NoSize = 1,
        NoZOrder = 0x4,
        ShowWindow = 0x400,
        SWP_ASYNCWINDOWPOS = 0x4000,	// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
        SWP_DEFERERASE = 0x2000,	// Prevents generation of the WM_SYNCPAINT message.
        SWP_DRAWFRAME = 0x0020,	 // Draws a frame (defined in the window's class description) around the window.
        SWP_FRAMECHANGED = 0x0020, //Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
        SWP_HIDEWINDOW = 0x0080,	// Hides the window.
        SWP_NOACTIVATE = 0x0010,	// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
        SWP_NOCOPYBITS = 0x0100,	// Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
        SWP_NOOWNERZORDER = 0x0200,	//Does not change the owner window's position in the Z order.
        SWP_NOREDRAW = 0x0008,	//Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
        SWP_NOREPOSITION = 0x0200,	// Same as the SWP_NOOWNERZORDER flag.
        SWP_NOSENDCHANGING = 0x0400	//Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
    }

    public enum WindowsMessage
    {
        NULL = 0,
        CREATE = 1,
        DESTROY = 2,
        MOVE = 3,
        SIZE = 5,
        ACTIVATE = 6,
        SETFOCUS = 7,
        KILLFOCUS = 8,
        ENABLE = 10,
        SETREDRAW = 11,
        SETTEXT = 12,
        GETTEXT = 13,
        GETTEXTLENGTH = 14,
        PAINT = 15,
        Close = 16,
        QUERYENDSESSION = 17,
        QUIT = 18,
        QUERYOPEN = 19,
        ERASEBKGND = 20,
        SYSCOLORCHANGE = 21,
        ENDSESSION = 22,
        SHOWWINDOW = 24,
        CTLCOLOR = 25,
        SETTINGCHANGE = 26,
        WININICHANGE = 26,
        DEVMODECHANGE = 27,
        ACTIVATEAPP = 28,
        FONTCHANGE = 29,
        TIMECHANGE = 30,
        CANCELMODE = 31,
        SETCURSOR = 32,
        TABLET_MAXOFFSET = 32,
        MOUSEACTIVATE = 33,
        CHILDACTIVATE = 34,
        QUEUESYNC = 35,
        GETMINMAXINFO = 36,
        PAINTICON = 38,
        ICONERASEBKGND = 39,
        NEXTDLGCTL = 40,
        SPOOLERSTATUS = 42,
        DRAWITEM = 43,
        MEASUREITEM = 44,
        DELETEITEM = 45,
        VKEYTOITEM = 46,
        CHARTOITEM = 47,
        SETFONT = 48,
        GETFONT = 49,
        SETHOTKEY = 50,
        GETHOTKEY = 51,
        QUERYDRAGICON = 55,
        COMPAREITEM = 57,
        GETOBJECT = 61,
        COMPACTING = 65,
        COMMNOTIFY = 68,
        WINDOWPOSCHANGING = 70,
        WINDOWPOSCHANGED = 71,
        POWER = 72,
        COPYDATA = 74,
        CANCELJOURNAL = 75,
        NOTIFY = 78,
        INPUTLANGCHANGEREQUEST = 80,
        INPUTLANGCHANGE = 81,
        TCARD = 82,
        HELP = 83,
        USERCHANGED = 84,
        NOTIFYFORMAT = 85,
        CONTEXTMENU = 123,
        STYLECHANGING = 124,
        STYLECHANGED = 125,
        DISPLAYCHANGE = 126,
        GETICON = 127,
        SETICON = 128,
        NCCREATE = 129,
        NCDESTROY = 130,
        NCCALCSIZE = 131,
        NCHITTEST = 132,
        NCPAINT = 133,
        NCACTIVATE = 134,
        GETDLGCODE = 135,
        SYNCPAINT = 136,
        MOUSEQUERY = 155,
        NCMOUSEMOVE = 160,
        NCLBUTTONDOWN = 161,
        NCLBUTTONUP = 162,
        NCLBUTTONDBLCLK = 163,
        NCRBUTTONDOWN = 164,
        NCRBUTTONUP = 165,
        NCRBUTTONDBLCLK = 166,
        NCMBUTTONDOWN = 167,
        NCMBUTTONUP = 168,
        NCMBUTTONDBLCLK = 169,
        NCXBUTTONDOWN = 171,
        NCXBUTTONUP = 172,
        NCXBUTTONDBLCLK = 173,
        INPUT = 255,
        KEYDOWN = 256,
        KEYFIRST = 256,
        KEYUP = 257,
        CHAR = 258,
        DEADCHAR = 259,
        SYSKEYDOWN = 260,
        SYSKEYUP = 261,
        SYSCHAR = 262,
        SYSDEADCHAR = 263,
        KEYLAST = 264,
        IME_STARTCOMPOSITION = 269,
        IME_ENDCOMPOSITION = 270,
        IME_COMPOSITION = 271,
        IME_KEYLAST = 271,
        INITDIALOG = 272,
        COMMAND = 273,
        SYSCOMMAND = 274,
        TIMER = 275,
        HSCROLL = 276,
        VSCROLL = 277,
        INITMENU = 278,
        INITMENUPOPUP = 279,
        MENUSELECT = 287,
        MENUCHAR = 288,
        ENTERIDLE = 289,
        UNINITMENUPOPUP = 293,
        CHANGEUISTATE = 295,
        UPDATEUISTATE = 296,
        QUERYUISTATE = 297,
        CTLCOLORMSGBOX = 306,
        CTLCOLOREDIT = 307,
        CTLCOLORLISTBOX = 308,
        CTLCOLORBTN = 309,
        CTLCOLORDLG = 310,
        CTLCOLORSCROLLBAR = 311,
        CTLCOLORSTATIC = 312,
        MOUSEFIRST = 512,
        MOUSEMOVE = 512,
        LBUTTONDOWN = 513,
        LBUTTONUP = 514,
        LBUTTONDBLCLK = 515,
        RBUTTONDOWN = 516,
        RBUTTONUP = 517,
        RBUTTONDBLCLK = 518,
        MBUTTONDOWN = 519,
        MBUTTONUP = 520,
        MBUTTONDBLCLK = 521,
        MOUSEWHEEL = 522,
        XBUTTONDOWN = 523,
        XBUTTONUP = 524,
        XBUTTONDBLCLK = 525,
        MOUSEHWHEEL = 526,
        MOUSELAST = 526,
        PARENTNOTIFY = 528,
        ENTERMENULOOP = 529,
        EXITMENULOOP = 530,
        NEXTMENU = 531,
        SIZING = 532,
        CAPTURECHANGED = 533,
        MOVING = 534,
        POWERBROADCAST = 536,
        DEVICECHANGE = 537,
        MDICREATE = 544,
        MDIDESTROY = 545,
        MDIACTIVATE = 546,
        MDIRESTORE = 547,
        MDINEXT = 548,
        MDIMAXIMIZE = 549,
        MDITILE = 550,
        MDICASCADE = 551,
        MDIICONARRANGE = 552,
        MDIGETACTIVE = 553,
        MDISETMENU = 560,
        ENTERSIZEMOVE = 561,
        EXITSIZEMOVE = 562,
        DROPFILES = 563,
        MDIREFRESHMENU = 564,
        IME_SETCONTEXT = 641,
        IME_NOTIFY = 642,
        IME_CONTROL = 643,
        IME_COMPOSITIONFULL = 644,
        IME_SELECT = 645,
        IME_CHAR = 646,
        IME_REQUEST = 648,
        IME_KEYDOWN = 656,
        IME_KEYUP = 657,
        MOUSEHOVER = 673,
        NCMOUSELEAVE = 674,
        MOUSELEAVE = 675,
        WTSSESSION_CHANGE = 689,
        TABLET_DEFBASE = 704,
        TABLET_ADDED = 712,
        TABLET_DELETED = 713,
        TABLET_FLICK = 715,
        TABLET_QUERYSYSTEMGESTURESTATUS = 716,
        CUT = 768,
        COPY = 769,
        PASTE = 770,
        CLEAR = 771,
        UNDO = 772,
        RENDERFORMAT = 773,
        RENDERALLFORMATS = 774,
        DESTROYCLIPBOARD = 775,
        DRAWCLIPBOARD = 776,
        PAINTCLIPBOARD = 777,
        VSCROLLCLIPBOARD = 778,
        SIZECLIPBOARD = 779,
        ASKCBFORMATNAME = 780,
        CHANGECBCHAIN = 781,
        HSCROLLCLIPBOARD = 782,
        QUERYNEWPALETTE = 783,
        PALETTEISCHANGING = 784,
        PALETTECHANGED = 785,
        HOTKEY = 786,
        PRINT = 791,
        PRINTCLIENT = 792,
        APPCOMMAND = 793,
        THEMECHANGED = 794,
        DWMCOMPOSITIONCHANGED = 798,
        DWMNCRENDERINGCHANGED = 799,
        DWMCOLORIZATIONCOLORCHANGED = 800,
        DWMWINDOWMAXIMIZEDCHANGE = 801,
        DWMSENDICONICTHUMBNAIL = 803,
        DWMSENDICONICLIVEPREVIEWBITMAP = 806,
        HANDHELDFIRST = 856,
        HANDHELDLAST = 863,
        AFXFIRST = 864,
        AFXLAST = 895,
        PENWINFIRST = 896,
        PENWINLAST = 911,
        USER = 1024,
        APP = 32768
    }
}
