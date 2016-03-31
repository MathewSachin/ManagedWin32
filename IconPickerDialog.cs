using ManagedWin32.Api;
using Microsoft.Win32;
using System;
using System.Text;

namespace ManagedWin32
{
    public class IconPickerDialog : CommonDialog
    {
        const int  MAX_PATH = 260;

        public string FileName { get; set; }

        public int IconIndex { get; set; }

        protected override bool RunDialog(IntPtr OwnerWindow)
        {
            var pathBuffer = new StringBuilder(FileName, MAX_PATH);

            int i;

            var result = Shell32.SHPickIconDialog(OwnerWindow, pathBuffer, MAX_PATH, out i);

            if (!result)
                return false;

            FileName = Environment.ExpandEnvironmentVariables(pathBuffer.ToString());
            IconIndex = i;

            return true;
        }

        public override void Reset()
        {
            FileName = null;
            IconIndex = 0;
        }
    }
}