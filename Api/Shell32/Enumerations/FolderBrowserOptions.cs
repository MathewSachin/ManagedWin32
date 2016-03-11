using System;

namespace ManagedWin32.Api
{
    [Flags]
    public enum FolderBrowserOptions
    {
        /// <summary>
        ///     None.
        /// </summary>
        None = 0,

        /// <summary>
        ///     For finding a folder to start document searching
        /// </summary>
        FolderOnly = 0x0001,

        /// <summary>
        ///     For starting the Find Computer
        /// </summary>
        FindComputer = 0x0002,

        /// <summary>
        ///     Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and
        ///     one line if this flag is set.  Passing the message
        ///     BFFM_SETSTATUSTEXTA to the hwnd can set the rest of the text.
        ///     This is not used with BIF_USENEWUI and BROWSEINFO.lpszTitle gets
        ///     all three lines of text.
        /// </summary>
        ShowStatusText = 0x0004,

        ReturnAncestors = 0x0008,

        /// <summary>
        ///     Add an editbox to the dialog
        /// </summary>
        ShowEditBox = 0x0010,

        /// <summary>
        ///     insist on valid result (or CANCEL)
        /// </summary>
        ValidateResult = 0x0020,

        /// <summary>
        ///     Use the new dialog layout with the ability to resize
        ///     Caller needs to call OleInitialize() before using this API
        /// </summary>
        UseNewStyle = 0x0040,

        UseNewStyleWithEditBox = (UseNewStyle | ShowEditBox),

        /// <summary>
        ///     Allow URLs to be displayed or entered. (Requires BIF_USENEWUI)
        /// </summary>
        AllowUrls = 0x0080,

        /// <summary>
        ///     Add a UA hint to the dialog, in place of the edit box. May not be
        ///     combined with BIF_EDITBOX.
        /// </summary>
        ShowUsageHint = 0x0100,

        /// <summary>
        ///     Do not add the "New Folder" button to the dialog.  Only applicable
        ///     with BIF_NEWDIALOGSTYLE.
        /// </summary>
        HideNewFolderButton = 0x0200,

        /// <summary>
        ///     don't traverse target as shortcut
        /// </summary>
        GetShortcuts = 0x0400,

        /// <summary>
        ///     Browsing for Computers.
        /// </summary>
        BrowseComputers = 0x1000,

        /// <summary>
        ///     Browsing for Printers.
        /// </summary>
        BrowsePrinters = 0x2000,

        /// <summary>
        ///     Browsing for Everything
        /// </summary>
        BrowseFiles = 0x4000,

        /// <summary>
        ///     sharable resources displayed (remote shares, requires BIF_USENEWUI)
        /// </summary>
        BrowseShares = 0x8000
    }
}