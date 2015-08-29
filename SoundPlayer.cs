using System;
using System.Runtime.InteropServices;

namespace ManagedWin32
{
    public enum SoundFlags
    {
        SYNC = 0,

        ASYNC = 1,

        NODEFAULT = 2,

        MEMORY = 4,

        LOOP = 8,

        PURGE = 64,

        FILENAME = 0x20000,

        NOSTOP = 16

    }

    class SoundPlayer
    {        
        [DllImport("winmm.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        public static extern bool PlaySound(string soundName, IntPtr hmod, SoundFlags SoundFlags);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(byte[] ptrToSound, UIntPtr hmod, uint fdwSound);
    }
}
