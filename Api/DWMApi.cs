using System;
using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.Win32;

namespace ManagedWin32.Api
{
    #region Enumerations
    [Flags]
    public enum DWMThumbnailFlags : uint
    {
        RectDestination = 1,
        RectSource = 2,
        Opacity = 4,
        Visible = 8,
        SourceClientAreaOnly = 0x10
    }

    [Flags]
    public enum DWMBlurbehindFlags : uint
    {
        /// <summary>Flag Transparency Enabled</summary>
        Enable = 1,
        /// <summary>Flag Region</summary>
        BlurRegion = 2,
        /// <summary>Flag Transition on maximized</summary>
        TransitionOnMaximized = 4
    }

    public enum DwmWindowAttribute
    {
        NonClientRenderingEnabled = 1,
        NonClientRenderingPolicy,
        TransitionsForceDisabled,
        AllowNonClientPaint,
        CaptionButtonBounds,
        NonClientRtlLayout,
        ForceIconicRepresentation,
        Flip3DPolicy,
        ExtendedFrameBounds,
        HasIconicBitmap,
        DisallowPeek,
        ExcludedFromPeek,
        Last
    }
    #endregion

    #region Structures
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

    [StructLayout(LayoutKind.Sequential)]
    public struct DWMBlurbehind
    {
        /// <summary>Flags that indicates the given parameters</summary>
        public DWMBlurbehindFlags dwFlags;
        /// <summary>True if the transparency is enabled</summary>
        public bool fEnable;
        /// <summary>Region</summary>
        public IntPtr hRgnBlur;
        public bool fTransitionOnMaximized;
    }
    #endregion

    public static class DWMApi
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmUpdateThumbnailProperties(IntPtr HThumbnail, ref DWMThumbnailProperties props);

        [DllImport("dwmapi.dll")]
        public static extern int DwmQueryThumbnailSourceSize(IntPtr HThumbnail, out Size size);

        [DllImport("dwmapi.dll")]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref RECT margins);

        [DllImport("dwmapi.dll")]
        public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr source, out IntPtr HThumbnail);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUnregisterThumbnail(IntPtr HThumbnail);

        [DllImport("dwmapi.dll")]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWMBlurbehind blurBehind);
        
        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hWnd, DwmWindowAttribute dWAttribute, ref RECT pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetColorizationColor(ref int color, [MarshalAs(UnmanagedType.Bool)] ref bool opaque);

        #region IsEnabled
        [DllImport("dwmapi.dll")]
        static extern uint DwmEnableComposition(uint uCompositionAction);

        [DllImport("dwmapi.dll")]
        static extern bool DwmIsCompositionEnabled();

        public static bool IsEnabled
        {
            get { return DwmIsCompositionEnabled(); }
            set { DwmEnableComposition(value ? 1u : 0); }
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
