using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    /// <summary>
    /// Presents an Icon Directory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 6)]
    public struct IconDir
    {
        public short Reserved;   // Reserved (must be 0)
        public short Type;       // Resource Type (1 for icons)
        public short Count;      // How many images?

        /// <summary>
        /// Converts the current NIco.IconDir into NIco.GroupIconDir.
        /// </summary>
        /// <returns>NIco.GroupIconDir</returns>
        public GroupIconDir ToGroupIconDir()
        {
            return new GroupIconDir()
            {
                Reserved = Reserved,
                Type = Type,
                Count = Count
            };
        }
    }

    /// <summary>
    /// Presents an Icon Directory Entry.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct IconDirEntry
    {
        public byte Width;          // Width, in pixels, of the image
        public byte Height;         // Height, in pixels, of the image
        public byte ColorCount;     // Number of colors in image (0 if >=8bpp)
        public byte Reserved;       // Reserved ( must be 0)
        public short Planes;         // Color Planes
        public short BitCount;       // Bits per pixel
        public int BytesInRes;     // How many bytes in this resource?
        public int ImageOffset;    // Where in the file is this image?

        /// <summary>
        /// Converts the current NIco.IconDirEntry into NIco.GroupIconDirEntry.
        /// </summary>
        /// <param name="id">The resource identifier.</param>
        /// <returns>NIco.GroupIconDirEntry</returns>
        public GroupIconDirEntry ToGroupIconDirEntry(int id)
        {
            return new GroupIconDirEntry()
            {
                Width = Width,
                Height = Height,
                ColorCount = ColorCount,
                Reserved = Reserved,
                Planes = Planes,
                BitCount = BitCount,
                BytesInRes = BytesInRes,
                ID = (short)id
            };
        }
    }

    /// <summary>
    /// Presents a Group Icon Directory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 6)]
    public struct GroupIconDir
    {
        public short Reserved;   // Reserved (must be 0)
        public short Type;       // Resource Type (1 for icons)
        public short Count;      // How many images?

        /// <summary>
        /// Converts the current NIco.GroupIconDir into NIco.IconDir.
        /// </summary>
        /// <returns>NIco.IconDir</returns>
        public IconDir ToIconDir()
        {
            return new IconDir()
            {
                Reserved = Reserved,
                Type = Type,
                Count = Count
            };
        }
    }

    /// <summary>
    /// Presents a Group Icon Directory Entry.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 14)]
    public struct GroupIconDirEntry
    {
        public byte Width;          // Width, in pixels, of the image
        public byte Height;         // Height, in pixels, of the image
        public byte ColorCount;     // Number of colors in image (0 if >=8bpp)
        public byte Reserved;       // Reserved ( must be 0)
        public short Planes;         // Color Planes
        public short BitCount;       // Bits per pixel
        public int BytesInRes;     // How many bytes in this resource?
        public short ID;             // the ID

        /// <summary>
        /// Converts the current NIco.GroupIconDirEntry into NIco.IconDirEntry.
        /// </summary>
        /// <param name="id">The resource identifier.</param>
        /// <returns>NIco.IconDirEntry</returns>
        public IconDirEntry ToIconDirEntry(int imageOffiset)
        {
            return new IconDirEntry()
            {
                Width = Width,
                Height = Height,
                ColorCount = ColorCount,
                Reserved = Reserved,
                Planes = Planes,
                BitCount = BitCount,
                BytesInRes = BytesInRes,
                ImageOffset = imageOffiset
            };
        }
    }
}