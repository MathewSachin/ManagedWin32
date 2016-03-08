using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CursorInfo
    {
        public int cbSize;        // Specifies the size, in bytes, of the structure. 
        public int flags;         // Specifies the cursor state. This parameter can be one of the following values:
        public IntPtr hCursor;          // Handle to the cursor. 
        public Point ptScreenPos;       // A Point structure that receives the screen coordinates of the cursor. 
    }
}