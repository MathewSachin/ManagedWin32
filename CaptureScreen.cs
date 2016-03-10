using System;
using System.Drawing;
using System.Runtime.InteropServices;
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

            if (hBitmap != IntPtr.Zero)
            {
                var hOld = Gdi32.SelectObject(hMemDC, hBitmap);

                Gdi32.BitBlt(hMemDC, 0, 0, SystemParams.ScreenWidth, SystemParams.ScreenHeight, hDC, 0, 0, CopyPixelOperation.SourceCopy);

                Gdi32.SelectObject(hMemDC, hOld);

                return Bitmap.FromHbitmap(hBitmap);
            }
            return null;
        }

        public static Bitmap CaptureCursor(ref int x, ref int y)
        {
            IntPtr hicon;

            var ci = new CursorInfo() { cbSize = Marshal.SizeOf(typeof(CursorInfo)) };

            IconInfo icInfo;

            if (User32.GetCursorInfo(out ci))
            {
                if (ci.flags == User32.CURSOR_SHOWING)
                {
                    hicon = User32.CopyIcon(ci.hCursor);

                    if (User32.GetIconInfo(hicon, out icInfo))
                    {
                        x = ci.ptScreenPos.X - ((int)icInfo.xHotspot);
                        y = ci.ptScreenPos.Y - ((int)icInfo.yHotspot);

                        return Icon.FromHandle(hicon).ToBitmap();
                    }
                }
            }

            return null;
        }

        public static Bitmap CaptureDesktopWithCursor()
        {
            int cursorX = 0, cursorY = 0;
            Bitmap desktopBMP, cursorBMP;
            Graphics g;
            Rectangle r;

            desktopBMP = CaptureDesktop();
            cursorBMP = CaptureCursor(ref cursorX, ref cursorY);
            if (desktopBMP != null)
            {
                if (cursorBMP != null)
                {
                    r = new Rectangle(cursorX, cursorY, cursorBMP.Width, cursorBMP.Height);
                    g = Graphics.FromImage(desktopBMP);
                    g.DrawImage(cursorBMP, r);
                    g.Flush();

                    return desktopBMP;
                }
                else return desktopBMP;
            }

            return null;
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
            IntPtr hBmp = Gdi32.CreateCompatibleBitmap(SourceDC, Width, Height);

            if (hBmp != null)
            {
                try
                {
                    // select the bitmap object
                    IntPtr hOld = Gdi32.SelectObject(MemoryDC, hBmp);

                    // bitblt over
                    Gdi32.BitBlt(MemoryDC, 0, 0, Width, Height, SourceDC, 0, 0, CopyPixelOperation.SourceCopy);

                    // restore selection
                    Gdi32.SelectObject(MemoryDC, hOld);

                    // get a .NET image object for it
                    return Bitmap.FromHbitmap(hBmp);
                }
                finally { Gdi32.DeleteObject(hBmp); }
            }

            return null;
        }

        public static Bitmap CaptureScreen()
        {
            var bitmap = new Bitmap(SystemParams.ScreenWidth, SystemParams.ScreenHeight);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(SystemParams.ScreenWidth, SystemParams.ScreenHeight));
                return bitmap;
            }
        }
    }
}
