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
        public DWMBlurbehindFlags dwFlags;
        
        /// <summary>
        /// True if the transparency is enabled
        /// </summary>
        public bool fEnable;
        
        /// <summary>
        /// Region
        /// </summary>
        public IntPtr hRgnBlur;
        
        public bool fTransitionOnMaximized;
    }
}