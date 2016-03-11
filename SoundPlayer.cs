using System;
using System.Runtime.InteropServices;

namespace ManagedWin32
{
    public enum SoundFlags
    {
        Sync = 0,

        Async = 1,

        NoDefault = 2,

        Memory = 4,

        Loop = 8,

        Purge = 64,

        FileName = 0x20000,

        NoStop = 16

    }

    public class SoundPlayer
    {        
        [DllImport("winmm.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        public static extern bool PlaySound(string soundName, IntPtr hmod, SoundFlags SoundFlags);

        [DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(byte[] ptrToSound, UIntPtr hmod, uint fdwSound);
    }
}
