using System;
using System.ComponentModel;
using System.Text;
using ManagedWin32.Api;
using Microsoft.Win32;

namespace ManagedWin32
{
    public class IconPickerDialog : CommonDialog
    {
        const int  MAX_PATH = 260;

        [DefaultValue(default(string))]
        public string FileName { get; set; }

        [DefaultValue(0)]
        public int IconIndex { get; set; }

        protected override bool RunDialog(IntPtr OwnerWindow)
        {
            var PathBuffer = new StringBuilder(FileName, MAX_PATH);
            int i;

            bool Result = User32.SHPickIconDialog(OwnerWindow, PathBuffer, MAX_PATH, out i);
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