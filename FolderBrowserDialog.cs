using ManagedWin32.Api;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace ManagedWin32
{
    /// <summary>
    /// Prompts the user to select a folder.
    /// </summary>
    public sealed class FolderBrowserDialog : CommonDialog
    {
        static IMalloc GetSHMalloc()
        {
            var ppMalloc = new IMalloc[1];
            Shell32.SHGetMalloc(ppMalloc);
            return ppMalloc[0];
        }
        
        FolderBrowserOptions _dialogOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBrowserDialog" /> class.
        /// </summary>
        public FolderBrowserDialog() { Reset(); }

        #region Methods
        /// <summary>
        /// Resets the properties of a common dialog to their default values.
        /// </summary>
        public override void Reset()
        {
            UseSpecialFolderRoot = true;
            RootSpecialFolder = Environment.SpecialFolder.Desktop;
            RootPath = string.Empty;
            Title = string.Empty;

            // default options
            _dialogOptions = FolderBrowserOptions.ShowEditBox
                             | FolderBrowserOptions.UseNewStyle
                             | FolderBrowserOptions.BrowseShares
                             | FolderBrowserOptions.ShowStatusText
                             | FolderBrowserOptions.ValidateResult;
        }

        /// <summary>
        /// Displays the folder browser dialog.
        /// </summary>
        /// <param name="hwndOwner">Handle to the window that owns the dialog box.</param>
        /// <returns>
        /// If the user clicks the OK button of the dialog that is displayed, true is returned; otherwise, false.
        /// </returns>
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            var result = false;

            IntPtr pidlRoot = IntPtr.Zero,
                pszPath = IntPtr.Zero,
                pidlSelected = IntPtr.Zero;

            SelectedPath = string.Empty;

            try
            {
                if (UseSpecialFolderRoot)
                    Shell32.SHGetFolderLocation(hwndOwner, (int)RootSpecialFolder, IntPtr.Zero, 0, out pidlRoot);
                else // RootType == Path
                {
                    uint iAttribute;
                    Shell32.SHParseDisplayName(RootPath, IntPtr.Zero, out pidlRoot, 0, out iAttribute);
                }

                var browseInfo = new BrowseInfo
                {
                    HwndOwner = hwndOwner,
                    Root = pidlRoot,
                    DisplayName = new string(' ', 256),
                    Title = Title,
                    Flags = _dialogOptions,
                    LParam = 0,
                    Callback = HookProc
                };

                // Show dialog
                pidlSelected = Shell32.SHBrowseForFolder(ref browseInfo);

                if (pidlSelected != IntPtr.Zero)
                {
                    result = true;

                    pszPath = Marshal.AllocHGlobal(260 * Marshal.SystemDefaultCharSize);
                    Shell32.SHGetPathFromIDList(pidlSelected, pszPath);

                    SelectedPath = Marshal.PtrToStringAuto(pszPath);
                }
            }
            finally // release all unmanaged resources
            {
                var malloc = GetSHMalloc();

                if (pidlRoot != IntPtr.Zero) 
                    malloc.Free(pidlRoot);

                if (pidlSelected != IntPtr.Zero) 
                    malloc.Free(pidlSelected);

                if (pszPath != IntPtr.Zero) 
                    Marshal.FreeHGlobal(pszPath);

                Marshal.ReleaseComObject(malloc);
            }

            return result;
        }

        bool GetOption(FolderBrowserOptions option) => _dialogOptions.HasFlag(option);

        void SetOption(FolderBrowserOptions option, bool value)
        {
            if (value) _dialogOptions |= option;
            else _dialogOptions &= ~option;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type of the root.
        /// </summary>
        /// <value>The type of the root.</value>
        public bool UseSpecialFolderRoot { get; set; }

        /// <summary>
        /// Gets or sets the root path.
        /// <remarks>Valid only if RootType is set to Path.</remarks>
        /// </summary>
        /// <value>The root path.</value>
        public string RootPath { get; set; }

        /// <summary>
        /// Gets or sets the root special folder.
        /// <remarks>Valid only if RootType is set to SpecialFolder.</remarks>
        /// </summary>
        /// <value>The root special folder.</value>
        public Environment.SpecialFolder RootSpecialFolder { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string SelectedPath { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether browsing files is allowed.
        /// </summary>
        public bool BrowseFiles
        {
            get { return GetOption(FolderBrowserOptions.BrowseFiles); }
            set { SetOption(FolderBrowserOptions.BrowseFiles, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show an edit box.
        /// </summary>
        public bool ShowEditBox
        {
            get { return GetOption(FolderBrowserOptions.ShowEditBox); }
            set { SetOption(FolderBrowserOptions.ShowEditBox, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether browsing shares is allowed.
        /// </summary>
        public bool BrowseShares
        {
            get { return GetOption(FolderBrowserOptions.BrowseShares); }
            set { SetOption(FolderBrowserOptions.BrowseShares, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show status text.
        /// </summary>
        public bool ShowStatusText
        {
            get { return GetOption(FolderBrowserOptions.ShowStatusText); }
            set { SetOption(FolderBrowserOptions.ShowStatusText, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the result.
        /// </summary>
        public bool ValidateResult
        {
            get { return GetOption(FolderBrowserOptions.ValidateResult); }
            set { SetOption(FolderBrowserOptions.ValidateResult, value); }
        }
        #endregion
    }
}