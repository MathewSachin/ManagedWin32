using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ManagedWin32.Api
{    
    #region AdvApi
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TokPriv1Luid
    {
        public int Count;
        public long Luid;
        public int Attr;

        public TokPriv1Luid(int c, long l, int a)
        {
            Count = c;
            Luid = l;
            Attr = a;
        }
    }
    #endregion

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int Left, int Top, int Right, int Bottom)
        {
            this = new RECT()
            {
                Left = Left,
                Top = Top,
                Right = Right,
                Bottom = Bottom
            };
        }
    }

    #region User32
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SCROLLINFO
    {
        public int cbSize;
        public int fMask;
        public int nMin;
        public int nMax;
        public int nPage;
        public int nPos;
        public int nTrackPos;
    }

    [StructLayout(LayoutKind.Sequential), Serializable()]
    public struct WindowInfo
    {
        public uint cbSize;
        public RECT rcWindow;
        public RECT rcClient;
        public uint dwStyle;
        public uint dwExStyle;
        public uint dwWindowStatus;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public ushort atomWindowType;
        public ushort wCreatorVersion;

        // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
        public WindowInfo(Boolean? filler)
            : this()
        {
            cbSize = (UInt32)(Marshal.SizeOf(typeof(WindowInfo)));
        }
    }

    /// <summary>
    /// Contains information about the placement of a window on the screen.
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable()]
    public struct WindowPlacement
    {
        /// <summary>
        /// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
        /// <para>
        /// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
        /// </para>
        /// </summary>
        public int Length;

        /// <summary>
        /// Specifies flags that control the position of the minimized window and the method by which the window is restored.
        /// </summary>
        public WindowPlacementFlags Flags;

        /// <summary>
        /// The current show state of the window.
        /// </summary>
        public ShowWindowFlags ShowCmd;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is minimized.
        /// </summary>
        public Point MinPosition;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is maximized.
        /// </summary>
        public Point MaxPosition;

        /// <summary>
        /// The window's coordinates when the window is in the restored position.
        /// </summary>
        public RECT NormalPosition;

        /// <summary>
        /// Gets the default (empty) value.
        /// </summary>
        public static WindowPlacement Default
        {
            get
            {
                WindowPlacement result = new WindowPlacement();
                result.Length = Marshal.SizeOf(result);
                return result;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IconInfo
    {
        public bool fIcon;         // Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies 
        public int xHotspot;     // Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot 
        public int yHotspot;     // Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot 
        public IntPtr hbmMask;     // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon, 
        public IntPtr hbmColor;    // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this 
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CursorInfo
    {
        public int cbSize;        // Specifies the size, in bytes, of the structure. 
        public int flags;         // Specifies the cursor state. This parameter can be one of the following values:
        public IntPtr hCursor;          // Handle to the cursor. 
        public Point ptScreenPos;       // A Point structure that receives the screen coordinates of the cursor. 
    }

    /// <summary>
    /// Win API WNDCLASS struct - represents a single window.
    /// Used to receive window messages.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowClass
    {
        public uint style;
        public WindowProcedureHandler lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }

    public enum BitmapImageCompression : int
    {
        RGB = 0,
        RLE8 = 1,
        RLE4 = 2,
        BITFIELDS = 3,
        JPEG = 4,
        BMP = 0,
        PNG = 5,
        UNKNOWN = 255
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BitmapInfoHeader
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public BitmapImageCompression biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;

        public BitmapInfoHeader(Stream stream)
        {
            this = new BitmapInfoHeader();
            this.Read(stream);
        }

        public unsafe void Read(Stream stream)
        {
            byte[] array = new byte[sizeof(BitmapInfoHeader)];
            stream.Read(array, 0, array.Length);
            fixed (byte* pData = array)
                this = *(BitmapInfoHeader*)pData;
        }

        public unsafe void Write(Stream stream)
        {
            byte[] array = new byte[sizeof(BitmapInfoHeader)];
            fixed (BitmapInfoHeader* ptr = &this)
                Marshal.Copy((IntPtr)ptr, array, 0, sizeof(BitmapInfoHeader));
            stream.Write(array, 0, sizeof(BitmapInfoHeader));
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct BitmapInfo
    {
        public BitmapInfoHeader bmiHeader;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public RGBQuad[] bmiColors;
    }

    /// <summary>
    /// The INPUT structure is used by SendInput to store information for synthesizing input events such as keystrokes, mouse movement, and mouse clicks. (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    /// <remarks>
    /// This structure contains information identical to that used in the parameter list of the keybd_event or mouse_event function.
    /// Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard input methods, such as handwriting recognition or voice recognition, as if it were text input by using the KEYEVENTF_UNICODE flag. For more information, see the remarks section of KEYBDINPUT.
    /// </remarks>
    public struct INPUT
    {
        /// <summary>
        /// Specifies the type of the input event. This member can be one of the following values. 
        /// <see cref="InputType.Mouse"/> - The event is a mouse event. Use the mi structure of the union.
        /// <see cref="InputType.Keyboard"/> - The event is a keyboard event. Use the ki structure of the union.
        /// <see cref="InputType.Hardware"/> - Windows 95/98/Me: The event is from input hardware other than a keyboard or mouse. Use the hi structure of the union.
        /// </summary>
        public uint Type;

        /// <summary>
        /// The data structure that contains information about the simulated Mouse, Keyboard or Hardware event.
        /// </summary>
        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    /// <summary>
    /// The combined/overlayed structure that includes Mouse, Keyboard and Hardware Input message data (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MOUSEKEYBDHARDWAREINPUT
    {
        /// <summary>
        /// The <see cref="MOUSEINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;

        /// <summary>
        /// The <see cref="KEYBDINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;

        /// <summary>
        /// The <see cref="HARDWAREINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
    }

    /// <summary>
    /// The MOUSEINPUT structure contains information about a simulated mouse event.
    /// </summary>
    /// <remarks>
    /// If the mouse has moved, indicated by MOUSEEVENTF_MOVE, dx and dy specify information about that movement. The information is specified as absolute or relative integer values. 
    /// If MOUSEEVENTF_ABSOLUTE value is specified, dx and dy contain normalized absolute coordinates between 0 and 65,535. The event procedure maps these coordinates onto the display surface. Coordinate (0,0) maps onto the upper-left corner of the display surface; coordinate (65535,65535) maps onto the lower-right corner. In a multimonitor system, the coordinates map to the primary monitor. 
    /// Windows 2000/XP: If MOUSEEVENTF_VIRTUALDESK is specified, the coordinates map to the entire virtual desktop.
    /// If the MOUSEEVENTF_ABSOLUTE value is not specified, dx and dy specify movement relative to the previous mouse event (the last reported position). Positive values mean the mouse moved right (or down); negative values mean the mouse moved left (or up). 
    /// Relative mouse motion is subject to the effects of the mouse speed and the two-mouse threshold values. A user sets these three values with the Pointer Speed slider of the Control Panel's Mouse Properties sheet. You can obtain and set these values using the SystemParametersInfo function. 
    /// The system applies two tests to the specified relative mouse movement. If the specified distance along either the x or y axis is greater than the first mouse threshold value, and the mouse speed is not zero, the system doubles the distance. If the specified distance along either the x or y axis is greater than the second mouse threshold value, and the mouse speed is equal to two, the system doubles the distance that resulted from applying the first threshold test. It is thus possible for the system to multiply specified relative mouse movement along the x or y axis by up to four times.
    /// </remarks>
    public struct MOUSEINPUT
    {
        /// <summary>
        /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the x coordinate of the mouse; relative data is specified as the number of pixels moved. 
        /// </summary>
        public Int32 X;

        /// <summary>
        /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the y coordinate of the mouse; relative data is specified as the number of pixels moved. 
        /// </summary>
        public Int32 Y;

        /// <summary>
        /// If dwFlags contains MOUSEEVENTF_WHEEL, then mouseData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user. One wheel click is defined as WHEEL_DELTA, which is 120. 
        /// Windows Vista: If dwFlags contains MOUSEEVENTF_HWHEEL, then dwData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated to the right; a negative value indicates that the wheel was rotated to the left. One wheel click is defined as WHEEL_DELTA, which is 120.
        /// Windows 2000/XP: IfdwFlags does not contain MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then mouseData should be zero. 
        /// If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were pressed or released. This value may be any combination of the following flags. 
        /// </summary>
        public UInt32 MouseData;

        /// <summary>
        /// A set of bit flags that specify various aspects of mouse motion and button clicks. The bits in this member can be any reasonable combination of the following values. 
        /// The bit flags that specify mouse button status are set to indicate changes in status, not ongoing conditions. For example, if the left mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN is set when the left button is first pressed, but not for subsequent motions. Similarly, MOUSEEVENTF_LEFTUP is set only when the button is first released. 
        /// You cannot specify both the MOUSEEVENTF_WHEEL flag and either MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP flags simultaneously in the dwFlags parameter, because they both require use of the mouseData field. 
        /// </summary>
        public UInt32 Flags;

        /// <summary>
        /// Time stamp for the event, in milliseconds. If this parameter is 0, the system will provide its own time stamp. 
        /// </summary>
        public UInt32 Time;

        /// <summary>
        /// Specifies an additional value associated with the mouse event. An application calls GetMessageExtraInfo to obtain this extra information. 
        /// </summary>
        public IntPtr ExtraInfo;
    }

    /// <summary>
    /// The KEYBDINPUT structure contains information about a simulated keyboard event.  (see: http://msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    /// <remarks>
    /// Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard-input methodssuch as handwriting recognition or voice recognitionas if it were text input by using the KEYEVENTF_UNICODE flag. If KEYEVENTF_UNICODE is specified, SendInput sends a WM_KEYDOWN or WM_KEYUP message to the foreground thread's message queue with wParam equal to VK_PACKET. Once GetMessage or PeekMessage obtains this message, passing the message to TranslateMessage posts a WM_CHAR message with the Unicode character originally specified by wScan. This Unicode character will automatically be converted to the appropriate ANSI value if it is posted to an ANSI window.
    /// Windows 2000/XP: Set the KEYEVENTF_SCANCODE flag to define keyboard input in terms of the scan code. This is useful to simulate a physical keystroke regardless of which keyboard is currently being used. The virtual key value of a key may alter depending on the current keyboard layout or what other keys were pressed, but the scan code will always be the same.
    /// </remarks>
    public struct KEYBDINPUT
    {
        /// <summary>
        /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. The Winuser.h header file provides macro definitions (VK_*) for each value. If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0. 
        /// </summary>
        public UInt16 KeyCode;

        /// <summary>
        /// Specifies a hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application. 
        /// </summary>
        public UInt16 Scan;

        /// <summary>
        /// Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
        /// KEYEVENTF_EXTENDEDKEY - If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).
        /// KEYEVENTF_KEYUP - If specified, the key is being released. If not specified, the key is being pressed.
        /// KEYEVENTF_SCANCODE - If specified, wScan identifies the key and wVk is ignored. 
        /// KEYEVENTF_UNICODE - Windows 2000/XP: If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section. 
        /// </summary>
        public UInt32 Flags;

        /// <summary>
        /// Time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp. 
        /// </summary>
        public UInt32 Time;

        /// <summary>
        /// Specifies an additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information. 
        /// </summary>
        public IntPtr ExtraInfo;
    }

    /// <summary>
    /// The HARDWAREINPUT structure contains information about a simulated message generated by an input device other than a keyboard or mouse.  (see: http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    public struct HARDWAREINPUT
    {
        /// <summary>
        /// Value specifying the message generated by the input hardware. 
        /// </summary>
        public UInt32 Msg;

        /// <summary>
        /// Specifies the low-order word of the lParam parameter for uMsg. 
        /// </summary>
        public UInt16 ParamL;

        /// <summary>
        /// Specifies the high-order word of the lParam parameter for uMsg. 
        /// </summary>
        public UInt16 ParamH;
    }
    #endregion

    #region PowrProf
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct SystemPowerCapablities
    {
        public byte PowerButtonPresent,
            SleepButtonPresent,
            LidPresent,
            SystemS1,
            SystemS2,
            SystemS3,
            SystemS4,
            SystemS5,
            HiberFilePresent,
            FullWake,
            VideoDimPresent,
            ApmPresent,
            UpsPresent,
            ThermalControl,
            ProcessorThrottle,
            ProcessorMinThrottle,
            ProcessorMaxThrottle,
            FastSystemS4;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
        public byte[] spare2;

        public byte DiskSpinDown;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
        public byte[] spare3;

        public byte SystemBatteriesPresent,
            BatteriesAreShortTerm;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.Struct)]
        public BatteryReportingScale[] BatteryScale;

        public SystemPowerState AcOnLineWake,
            SoftLidWake,
            RtcWake,
            MinDeviceWakeState,
            DefaultLowLatencyWake;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct BatteryReportingScale { public int Granularity, Capacity; }
    #endregion

    #region Kernel32
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STARTUPINFO
    {
        public int cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public int dwX;
        public int dwY;
        public int dwXSize;
        public int dwYSize;
        public int dwXCountChars;
        public int dwYCountChars;
        public int dwFillAttribute;
        public int dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBQuad
    {
        public byte rgbBlue, rgbGreen, rgbRed, rgbReserved;

        public void Set(byte r, byte g, byte b)
        {
            rgbRed = r;
            rgbGreen = g;
            rgbBlue = b;
        }
    }
    #endregion

    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public RECT rc;
        public int lParam;
    }

    /// <summary>
    /// A struct that is submitted in order to configure
    /// the taskbar icon. Provides various members that
    /// can be configured partially, according to the
    /// values of the <see cref="IconDataMembers"/>
    /// that were defined.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NotifyIconData
    {
        /// <summary>
        /// Size of this structure, in bytes.
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// Handle to the window that receives notification messages associated with an icon in the
        /// taskbar status area. The Shell uses hWnd and uID to identify which icon to operate on
        /// when Shell_NotifyIcon is invoked.
        /// </summary>
        public IntPtr WindowHandle;

        /// <summary>
        /// Application-defined identifier of the taskbar icon. The Shell uses hWnd and uID to identify
        /// which icon to operate on when Shell_NotifyIcon is invoked. You can have multiple icons
        /// associated with a single hWnd by assigning each a different uID. This feature, however
        /// is currently not used.
        /// </summary>
        public uint TaskbarIconId;

        /// <summary>
        /// Flags that indicate which of the other members contain valid data. This member can be
        /// a combination of the NIF_XXX constants.
        /// </summary>
        public IconDataMembers ValidMembers;

        /// <summary>
        /// Application-defined message identifier. The system uses this identifier to send
        /// notifications to the window identified in hWnd.
        /// </summary>
        public uint CallbackMessageId;

        /// <summary>
        /// A handle to the icon that should be displayed. Just
        /// <c>Icon.Handle</c>.
        /// </summary>
        public IntPtr IconHandle;

        /// <summary>
        /// String with the text for a standard ToolTip. It can have a maximum of 64 characters including
        /// the terminating NULL. For Version 5.0 and later, szTip can have a maximum of
        /// 128 characters, including the terminating NULL.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string ToolTipText;

        /// <summary>
        /// State of the icon. Remember to also set the <see cref="StateMask"/>.
        /// </summary>
        public IconState IconState;

        /// <summary>
        /// A value that specifies which bits of the state member are retrieved or modified.
        /// For example, setting this member to <see cref="Interop.IconState.Hidden"/>
        /// causes only the item's hidden
        /// state to be retrieved.
        /// </summary>
        public IconState StateMask;

        /// <summary>
        /// String with the text for a balloon ToolTip. It can have a maximum of 255 characters.
        /// To remove the ToolTip, set the NIF_INFO flag in uFlags and set szInfo to an empty string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string BalloonText;

        /// <summary>
        /// Mainly used to set the version when <see cref="WinApi.Shell_NotifyIcon"/> is invoked
        /// with <see cref="NotifyCommand.SetVersion"/>. However, for legacy operations,
        /// the same member is also used to set timouts for balloon ToolTips.
        /// </summary>
        public uint VersionOrTimeout;

        /// <summary>
        /// String containing a title for a balloon ToolTip. This title appears in boldface
        /// above the text. It can have a maximum of 63 characters.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string BalloonTitle;

        /// <summary>
        /// Adds an icon to a balloon ToolTip, which is placed to the left of the title. If the
        /// <see cref="BalloonTitle"/> member is zero-length, the icon is not shown.
        /// </summary>
        public BalloonFlags BalloonFlags;

        /// <summary>
        /// Windows XP (Shell32.dll version 6.0) and later.<br/>
        /// - Windows 7 and later: A registered GUID that identifies the icon.
        ///   This value overrides uID and is the recommended method of identifying the icon.<br/>
        /// - Windows XP through Windows Vista: Reserved.
        /// </summary>
        public Guid TaskbarIconGuid;

        /// <summary>
        /// Windows Vista (Shell32.dll version 6.0.6) and later. The handle of a customized
        /// balloon icon provided by the application that should be used independently
        /// of the tray icon. If this member is non-NULL and the <see cref="Interop.BalloonFlags.User"/>
        /// flag is set, this icon is used as the balloon icon.<br/>
        /// If this member is NULL, the legacy behavior is carried out.
        /// </summary>
        public IntPtr CustomBalloonIconHandle;

        /// <summary>
        /// Creates a default data structure that provides
        /// a hidden taskbar icon without the icon being set.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static NotifyIconData CreateDefault(IntPtr handle)
        {
            var data = new NotifyIconData();

            if (Environment.OSVersion.Version.Major >= 6)
            {
                //use the current size
                data.cbSize = (uint)Marshal.SizeOf(data);
            }
            else
            {
                //we need to set another size on xp/2003- otherwise certain
                //features (e.g. balloon tooltips) don't work.
                data.cbSize = 952; // NOTIFYICONDATAW_V3_SIZE

                //set to fixed timeout
                data.VersionOrTimeout = 10;
            }

            const int CallbackMessageId = 0x400;

            data.WindowHandle = handle;
            data.TaskbarIconId = 0x0;
            data.CallbackMessageId = CallbackMessageId;
            data.VersionOrTimeout = (uint)NotifyIconVersion.Win95;

            data.IconHandle = IntPtr.Zero;

            //hide initially
            data.IconState = IconState.Hidden;
            data.StateMask = IconState.Hidden;

            //set flags
            data.ValidMembers = IconDataMembers.Message | IconDataMembers.Icon | IconDataMembers.Tip;

            //reset strings
            data.ToolTipText = data.BalloonText = data.BalloonTitle = String.Empty;

            return data;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    /// <summary>
    /// Presents an Icon Directory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 6)]
    public struct IconDir
    {
        public short Reserved;   // Reserved (must be 0)
        public short Type;       // Resource Type (1 for icons)
        public short Count;      // How many images?

        /// <summary>
        /// Converts the current NIco.IconDir into NIco.GroupIconDir.
        /// </summary>
        /// <returns>NIco.GroupIconDir</returns>
        public GroupIconDir ToGroupIconDir()
        {
            GroupIconDir grpDir = new GroupIconDir();
            grpDir.Reserved = this.Reserved;
            grpDir.Type = this.Type;
            grpDir.Count = this.Count;
            return grpDir;
        }
    }

    /// <summary>
    /// Presents an Icon Directory Entry.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct IconDirEntry
    {
        public byte Width;          // Width, in pixels, of the image
        public byte Height;         // Height, in pixels, of the image
        public byte ColorCount;     // Number of colors in image (0 if >=8bpp)
        public byte Reserved;       // Reserved ( must be 0)
        public short Planes;         // Color Planes
        public short BitCount;       // Bits per pixel
        public int BytesInRes;     // How many bytes in this resource?
        public int ImageOffset;    // Where in the file is this image?

        /// <summary>
        /// Converts the current NIco.IconDirEntry into NIco.GroupIconDirEntry.
        /// </summary>
        /// <param name="id">The resource identifier.</param>
        /// <returns>NIco.GroupIconDirEntry</returns>
        public GroupIconDirEntry ToGroupIconDirEntry(int id)
        {
            GroupIconDirEntry grpEntry = new GroupIconDirEntry();
            grpEntry.Width = this.Width;
            grpEntry.Height = this.Height;
            grpEntry.ColorCount = this.ColorCount;
            grpEntry.Reserved = this.Reserved;
            grpEntry.Planes = this.Planes;
            grpEntry.BitCount = this.BitCount;
            grpEntry.BytesInRes = this.BytesInRes;
            grpEntry.ID = (short)id;
            return grpEntry;
        }
    }

    /// <summary>
    /// Presents a Group Icon Directory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 6)]
    public struct GroupIconDir
    {
        public short Reserved;   // Reserved (must be 0)
        public short Type;       // Resource Type (1 for icons)
        public short Count;      // How many images?

        /// <summary>
        /// Converts the current NIco.GroupIconDir into NIco.IconDir.
        /// </summary>
        /// <returns>NIco.IconDir</returns>
        public IconDir ToIconDir()
        {
            IconDir dir = new IconDir();
            dir.Reserved = this.Reserved;
            dir.Type = this.Type;
            dir.Count = this.Count;
            return dir;
        }
    }

    /// <summary>
    /// Presents a Group Icon Directory Entry.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 14)]
    public struct GroupIconDirEntry
    {
        public byte Width;          // Width, in pixels, of the image
        public byte Height;         // Height, in pixels, of the image
        public byte ColorCount;     // Number of colors in image (0 if >=8bpp)
        public byte Reserved;       // Reserved ( must be 0)
        public short Planes;         // Color Planes
        public short BitCount;       // Bits per pixel
        public int BytesInRes;     // How many bytes in this resource?
        public short ID;             // the ID

        /// <summary>
        /// Converts the current NIco.GroupIconDirEntry into NIco.IconDirEntry.
        /// </summary>
        /// <param name="id">The resource identifier.</param>
        /// <returns>NIco.IconDirEntry</returns>
        public IconDirEntry ToIconDirEntry(int imageOffiset)
        {
            IconDirEntry entry = new IconDirEntry();
            entry.Width = this.Width;
            entry.Height = this.Height;
            entry.ColorCount = this.ColorCount;
            entry.Reserved = this.Reserved;
            entry.Planes = this.Planes;
            entry.BitCount = this.BitCount;
            entry.BytesInRes = this.BytesInRes;
            entry.ImageOffset = imageOffiset;
            return entry;
        }
    }
}