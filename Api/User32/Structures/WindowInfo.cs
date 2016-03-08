using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayout(LayoutKind.Sequential)]
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
        public WindowInfo(bool? filler)
            : this()
        {
            cbSize = (uint)Marshal.SizeOf(typeof(WindowInfo));
        }
    }
}