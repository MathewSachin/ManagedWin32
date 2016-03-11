using System;
using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [ComImport, Guid("00000002-0000-0000-c000-000000000046"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMalloc
    {
        [PreserveSig]
        IntPtr Alloc(int cb);

        [PreserveSig]
        IntPtr Realloc(IntPtr pv, int cb);

        [PreserveSig]
        void Free(IntPtr pv);

        [PreserveSig]
        int GetSize(IntPtr pv);

        [PreserveSig]
        int DidAlloc(IntPtr pv);

        [PreserveSig]
        void HeapMinimize();
    }
}