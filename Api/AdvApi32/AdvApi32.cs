using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWin32.Api
{
    public static class AdvApi32
    {
        const string DllName = "advapi32.dll";

        public const int SE_PRIVILEGE_ENABLED = 0x00000002,
            TOKEN_QUERY = 0x00000008,
            TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        [DllImport(DllName)]
        public static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport(DllName)]
        public static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport(DllName)]
        public static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        #region UserName
        [DllImport(DllName)]
        static extern bool GetUserName([Out] StringBuilder lpBuffer, ref int nSize);

        [Obsolete("Use System.Environment.UserName instead.")]
        public static string UserName
        {
            get
            {
                var Size = 256;
                var sb = new StringBuilder(Size);
                GetUserName(sb, ref Size);
                return sb.ToString(0, Size);
            }
        }
        #endregion

        [Obsolete("Use System.Environment.DomainName instead.")]
        [DllImport("secur32.dll", CharSet = CharSet.Unicode)]
        public static extern byte GetUserNameEx(int format, [Out] StringBuilder domainName, ref uint domainNameLen);
    }
}
