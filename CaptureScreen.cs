using System;
using System.Drawing;
using ManagedWin32.Api;

namespace ManagedWin32
{
    public static class ScreenCapture
    {
        public static Bitmap CaptureDesktop()
        {
            IntPtr hDC = User32.GetWindowDC(IntPtr.Zero),
                   hMemDC = Gdi32.CreateCompatibleDC(hDC),
                   hBitmap = Gdi32.CreateCompatibleBitmap(hDC, SystemParams.ScreenWidth, SystemParams.ScreenHeight);

            if (hBitmap == IntPtr.Zero)
                return null;

            var hOld = Gdi32.SelectObject(hMemDC, hBitmap);

            Gdi32.BitBlt(hMemDC, 0, 0, SystemParams.ScreenWidth, SystemParams.ScreenHeight, hDC, 0, 0, CopyPixelOperation.SourceCopy);

            Gdi32.SelectObject(hMemDC, hOld);

            return Bitmap.FromHbitmap(hBitmap);
        }

        public static Bitmap CaptureCursor(ref int x, ref int y)
        {
            CursorInfo ci;

            if (!User32.GetCursorInfo(out ci))
                return null;

            if (ci.flags != User32.CURSOR_SHOWING)
                return null;

            var hicon = User32.CopyIcon(ci.hCursor);

            IconInfo icInfo;

            if (!User32.GetIconInfo(hicon, out icInfo))
                return null;

            x = ci.ptScreenPos.X - icInfo.xHotspot;
            y = ci.ptScreenPos.Y - icInfo.yHotspot;

            return Icon.FromHandle(hicon).ToBitmap();
        }

        public static Bitmap CaptureDesktopWithCursor()
        {
            int cursorX = 0, cursorY = 0;

            var desktopBMP = CaptureDesktop();
            var cursorBMP = CaptureCursor(ref cursorX, ref cursorY);

            if (desktopBMP == null)
                return null;

            if (cursorBMP == null)
                return desktopBMP;

            var r = new Rectangle(cursorX, cursorY, cursorBMP.Width, cursorBMP.Height);
            var g = Graphics.FromImage(desktopBMP);
            g.DrawImage(cursorBMP, r);
            g.Flush();

            return desktopBMP;
        }

        public static Bitmap Capture(IntPtr Window)
        {
            IntPtr SourceDC = User32.GetWindowDC(Window),
                   MemoryDC = Gdi32.CreateCompatibleDC(SourceDC);

            var rect = new RECT();
            User32.GetWindowRect(Window, ref rect);

            int Width = rect.Right - rect.Left,
                Height = rect.Bottom - rect.Top;

            // Create a bitmap we can copy it to
            var hBmp = Gdi32.CreateCompatibleBitmap(SourceDC, Width, Height);

            try
            {
                // select the bitmap object
                var hOld = Gdi32.SelectObject(MemoryDC, hBmp);

                // bitblt over
                Gdi32.BitBlt(MemoryDC, 0, 0, Width, Height, SourceDC, 0, 0, CopyPixelOperation.SourceCopy);

                // restore selection
                Gdi32.SelectObject(MemoryDC, hOld);

                // get a .NET image object for it
                return Bitmap.FromHbitmap(hBmp);
            }
            finally { Gdi32.DeleteObject(hBmp); }
        }

        public static Bitmap CaptureScreen()
        {
            var bitmap = new Bitmap(SystemParams.ScreenWidth, SystemParams.ScreenHeight);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, new Size(SystemParams.ScreenWidth, SystemParams.ScreenHeight));
                return bitmap;
            }
        }
    }
}
