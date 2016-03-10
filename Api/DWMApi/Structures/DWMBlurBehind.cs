using System;
using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DWMBlurbehind
    {
        /// <summary>
        /// Flags that indicates the given parameters
        /// </summary>
        public DWMBlurbehindFlags Flags;
        
        /// <summary>
        /// True if the transparency is enabled
        /// </summary>
        public bool Enable;
        
        /// <summary>
        /// Region to blur
        /// </summary>
        public IntPtr Region;
        
        public bool TransitionOnMaximized;
    }
}