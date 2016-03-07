using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
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
}