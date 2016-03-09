using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ManagedWin32.Api
{
    public static class Gdi32
    {
        const string DllName = "gdi32.dll";

        [DllImport(DllName)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport(DllName)]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport(DllName)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport(DllName)]
        public static extern bool MaskBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth,
           int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, IntPtr hbmMask, int xMask,
           int yMask, uint dwRop);

        [DllImport(DllName)]
        public static extern bool StretchBlt(SafeHandle hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, SafeHandle hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, CopyPixelOperation dwRop);

        [DllImport(DllName)]
        public static extern int SetBkColor(IntPtr hDC, uint crColor);

        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, CopyPixelOperation dwRop);

        [DllImport(DllName, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BitmapInfo pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport(DllName)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport(DllName)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport(DllName)]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport(DllName)]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport(DllName)]
        public static extern int GetDeviceCaps(SafeHandle hdc, DeviceCaps nIndex);

        [DllImport(DllName)]
        public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, int lpInitData);

        [DllImport(DllName)]
        public static extern int SaveDC(IntPtr hdc);
    }
}
