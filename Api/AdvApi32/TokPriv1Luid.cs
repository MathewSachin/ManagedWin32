using System.Runtime.InteropServices;

namespace ManagedWin32.Api
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TokPriv1Luid
    {
        public int Count;
        public long Luid;
        public int Attr;

        public TokPriv1Luid(int c, long l, int a)
        {
            Count = c;
            Luid = l;
            Attr = a;
        }
    }
}