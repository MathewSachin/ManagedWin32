namespace ManagedWin32.Api
{
    public enum DeviceCaps
    {
        /// <summary>
        /// Device driver version
        /// </summary>
        DriverVersion = 0,

        /// <summary>
        /// Device classification
        /// </summary>
        Technology = 2,

        /// <summary>
        /// Horizontal size in millimeters
        /// </summary>
        HorizontalSize = 4,

        /// <summary>
        /// Vertical size in millimeters
        /// </summary>
        VerticalSize = 6,

        /// <summary>
        /// Horizontal width in pixels
        /// </summary>
        HorizontalResolution = 8,

        /// <summary>
        /// Vertical height in pixels
        /// </summary>
        VerticalResolution = 10,

        /// <summary>
        /// Number of bits per pixel
        /// </summary>
        BitsPerPixel = 12,

        /// <summary>
        /// Number of planes
        /// </summary>
        Planes = 14,

        /// <summary>
        /// Number of brushes the device has
        /// </summary>
        NumBrushes = 16,

        /// <summary>
        /// Number of pens the device has
        /// </summary>
        NumPens = 18,

        /// <summary>
        /// Number of markers the device has
        /// </summary>
        NumMarkers = 20,

        /// <summary>
        /// Number of fonts the device has
        /// </summary>
        NumFonts = 22,

        /// <summary>
        /// Number of colors the device supports
        /// </summary>
        NumColors = 24,

        /// <summary>
        /// Size required for device descriptor
        /// </summary>
        PDeviceSize = 26,

        /// <summary>
        /// Curve capabilities
        /// </summary>
        CurveCaps = 28,

        /// <summary>
        /// Line capabilities
        /// </summary>
        LineCaps = 30,

        /// <summary>
        /// Polygonal capabilities
        /// </summary>
        PolygonCaps = 32,

        /// <summary>
        /// Text capabilities
        /// </summary>
        TextCaps = 34,

        /// <summary>
        /// Clipping capabilities
        /// </summary>
        ClipCaps = 36,

        /// <summary>
        /// Bitblt capabilities
        /// </summary>
        RasterCaps = 38,

        /// <summary>
        /// Length of the X leg
        /// </summary>
        AspectX = 40,

        /// <summary>
        /// Length of the Y leg
        /// </summary>
        AspectY = 42,

        /// <summary>
        /// Length of the hypotenuse
        /// </summary>
        AspectXY = 44,

        /// <summary>
        /// Shading and Blending caps
        /// </summary>
        ShadeBlendCaps = 45,

        /// <summary>
        /// Logical pixels inch in X
        /// </summary>
        LogicPixelsX = 88,

        /// <summary>
        /// Logical pixels inch in Y
        /// </summary>
        LogicPixelsY = 90,

        /// <summary>
        /// Number of entries in physical palette
        /// </summary>
        SizePallette = 104,

        /// <summary>
        /// Number of reserved entries in palette
        /// </summary>
        NumReserved = 106,

        /// <summary>
        /// Actual color resolution
        /// </summary>
        ColorResolution = 108,

        // Printing related DeviceCaps. These replace the appropriate Escapes
        /// <summary>
        /// Physical Width in device units
        /// </summary>
        PhysicalWidth = 110,

        /// <summary>
        /// Physical Height in device units
        /// </summary>
        PhysicalHeight = 111,

        /// <summary>
        /// Physical Printable Area x margin
        /// </summary>
        PhysicalOffsetX = 112,

        /// <summary>
        /// Physical Printable Area y margin
        /// </summary>
        PhysicalOffsetY = 113,

        /// <summary>
        /// Scaling factor x
        /// </summary>
        ScalingFactorX = 114,

        /// <summary>
        /// Scaling factor y
        /// </summary>
        ScalingFactorY = 115,

        /// <summary>
        /// Current vertical refresh rate of the display device (for displays only) in Hz
        /// </summary>
        VerticalRefreshRate = 116,

        // TODO: Check Ambiguous
        /// <summary>
        /// Horizontal width of entire desktop in pixels
        /// </summary>
        DesktopVerticalResolution = 117,

        // TODO: Check Ambiguous
        /// <summary>
        /// Vertical height of entire desktop in pixels
        /// </summary>
        DesktopHorizontalResolution = 118,
        /// <summary>
        /// Preferred blt alignment
        /// </summary>
        BLTALIGNMENT = 119
    }
}