using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BitmapInfoHeader
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public BitmapImageCompression biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;

        public BitmapInfoHeader(Stream stream)
        {
            this = new BitmapInfoHeader();
            this.Read(stream);
        }

        public unsafe void Read(Stream stream)
        {
            byte[] array = new byte[sizeof(BitmapInfoHeader)];
            stream.Read(array, 0, array.Length);
            fixed (byte* pData = array)
                this = *(BitmapInfoHeader*)pData;
        }

        public unsafe void Write(Stream stream)
        {
            byte[] array = new byte[sizeof(BitmapInfoHeader)];
            fixed (BitmapInfoHeader* ptr = &this)
                Marshal.Copy((IntPtr)ptr, array, 0, sizeof(BitmapInfoHeader));
            stream.Write(array, 0, sizeof(BitmapInfoHeader));
        }
    }
}