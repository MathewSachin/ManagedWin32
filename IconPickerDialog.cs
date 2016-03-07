using ManagedWin32.Api;
using Microsoft.Win32;
using System;
using System.Text;

namespace ManagedWin32
{
    public class IconPickerDialog : CommonDialog
    {
        const int  MAX_PATH = 260;

        public string FileName { get; set; } = null;

        public int IconIndex { get; set; } = 0;

        protected override bool RunDialog(IntPtr OwnerWindow)
        {
            var PathBuffer = new StringBuilder(FileName, MAX_PATH);
            int i;

            bool Result = Shell32.SHPickIconDialog(OwnerWindow, PathBuffer, MAX_PATH, out i);
            if (Result)
            {
                FileName = Environment.ExpandEnvironmentVariables(PathBuffer.ToString());
                IconIndex = i;
            }

            return Result;
        }

        public override void Reset()
        {
            FileName = null;
            IconIndex = 0;
        }
    }
}