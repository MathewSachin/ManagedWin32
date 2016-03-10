using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGBQuad
    {
        public byte Blue, Green, Red;
        
        byte rgbReserved;

        public void Set(byte r, byte g, byte b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }
    }
}