using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct BitmapInfo
    {
        public BitmapInfoHeader bmiHeader;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public RGBQuad[] bmiColors;
    }
}