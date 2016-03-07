using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DWMThumbnailProperties
    {
        public DWMThumbnailFlags Flags;
        public RECT Destination;
        public RECT Source;
        public byte Opacity;
        public bool Visible;
        public bool SourceClientAreaOnly;
    }
}