using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWin32.Api
{
    public static class AdvApi32
    {
        public const int SE_PRIVILEGE_ENABLED = 0x00000002,
            TOKEN_QUERY = 0x00000008,
            TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        #region UserName
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        static extern bool GetUserName([Out] StringBuilder lpBuffer, ref int nSize);

        public static string UserName
        {
            get
            {
                int Size = 256;
                StringBuilder sb = new StringBuilder(256);
                GetUserName(sb, ref Size);
                return sb.ToString(0, Size);
            }
        }
        #endregion

        [DllImport("secur32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, SetLastError = true)]
        public static extern byte GetUserNameEx(int format, [Out] StringBuilder domainName, ref uint domainNameLen);
    }
}
