using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    public static class DWMApi
    {
        const string DllName = "dwmapi.dll";

        [DllImport(DllName)]
        public static extern int DwmUpdateThumbnailProperties(IntPtr HThumbnail, ref DWMThumbnailProperties props);

        [DllImport(DllName)]
        public static extern int DwmQueryThumbnailSourceSize(IntPtr HThumbnail, out Size size);

        [DllImport(DllName)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref RECT margins);

        [DllImport(DllName)]
        public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr source, out IntPtr HThumbnail);

        [DllImport(DllName)]
        public static extern int DwmUnregisterThumbnail(IntPtr HThumbnail);

        [DllImport(DllName)]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWMBlurbehind blurBehind);
        
        [DllImport(DllName)]
        public static extern int DwmGetWindowAttribute(IntPtr hWnd, DWMWindowAttribute dWAttribute, ref RECT pvAttribute, int cbAttribute);

        [DllImport(DllName)]
        public static extern int DwmGetColorizationColor(ref int color, [MarshalAs(UnmanagedType.Bool)] ref bool opaque);

        #region IsEnabled
        [DllImport(DllName)]
        static extern uint DwmEnableComposition(int uCompositionAction);

        [DllImport(DllName)]
        static extern bool DwmIsCompositionEnabled();

        public static bool IsEnabled
        {
            get { return DwmIsCompositionEnabled(); }
            set { DwmEnableComposition(value ? 1 : 0); }
        }
        #endregion

        public static bool IsTransparency
        {
            get
            {
                int color = 0;
                bool opaque = true;

                DwmGetColorizationColor(ref color, ref opaque);

                return !opaque;
            }
        }

        public static Color ColorizationColor
        {
            get
            {
                int color = 0;
                bool opaque = true;

                DwmGetColorizationColor(ref color, ref opaque);

                return Color.FromArgb(color);
            }
        }
    }
}
