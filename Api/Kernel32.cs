using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ManagedWin32.Api
{
    public static class Kernel32
    {
        public const int MAX_PATH = 260;

        public static bool IsIntResource(IntPtr lpszName) { return (((uint)lpszName >> 16) == 0); }

        #region CurrentProcess
        [DllImport("kernel32.dll", ExactSpelling = true)]
        static extern IntPtr GetCurrentProcess();

        public static IntPtr CurrentProcess { get { return GetCurrentProcess(); } }
        #endregion

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);
        
        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes, bool bInheritHandles, int dwCreationFlags, IntPtr lpEnvironment,
            string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, ref PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll")]
        public static extern int GetThreadId(IntPtr thread);

        [DllImport("kernel32.dll")]
        public static extern int GetProcessId(IntPtr process);

        [DllImport("kernel32.dll")]
        public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, [Out] StringBuilder lpBuffer, int nSize, IntPtr Arguments);

        [DllImport("kernel32.dll")]
        public static extern void SetLastError(uint dwErrCode);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

        [DllImport("KERNEL32.DLL")]
        public unsafe static extern void CopyMemory(void* dest, void* src, int length);

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public unsafe static extern void CopyMemory(RGBQuad* dest, byte* src, int cb);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern bool CopyFile(string src, string dst, bool failIfExists);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern bool DeleteFile(string path);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern int ExpandEnvironmentStrings(string lpSrc, [Out] StringBuilder lpDst, int nSize);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false)]
        internal static extern int GetComputerName([Out] StringBuilder nameBuffer, ref int bufferSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern bool GetDiskFreeSpaceEx(string drive, out long freeBytesForUser, out long totalBytes, out long freeBytes);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern int GetDriveType(string drive);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern int GetEnvironmentVariable(string lpName, [Out] StringBuilder lpValue, int size);

        [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
        internal static extern int GetLogicalDrives();

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern int GetSystemDirectory([Out] StringBuilder sb, int length);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern uint GetTempFileName(string tmpPath, string prefix, uint uniqueIdOrZero, [Out] StringBuilder tmpFileName);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false)]
        internal static extern uint GetTempPath(int bufferLen, [Out] StringBuilder buffer);

        [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
        internal static extern bool IsWow64Process([In] IntPtr hSourceProcessHandle, out bool isWow64);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern bool MoveFile(string src, string dst);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern int GetWindowsDirectory([Out] StringBuilder sb, int length);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern bool RemoveDirectory(string path);

        [DllImport("kernel32.dll", CharSet = CharSet.None, EntryPoint = "RtlZeroMemory", ExactSpelling = false, SetLastError = true)]
        internal static extern void ZeroMemory(IntPtr address, UIntPtr length);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        internal static extern bool SetEnvironmentVariable(string lpName, string lpValue);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern bool UpdateResource(IntPtr hUpdate, ResourceType lpType, IntPtr pName, ushort wLanguage, byte[] lpData, uint cbData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UpdateResource(IntPtr hUpdate, ResourceType lpType, uint lpName, ushort wLanguage, byte[] lpData, uint cbData);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibraryEx(string path, IntPtr hFile, LoadLibraryFlags flags);

        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, IntPtr resourceID, ResourceType type);

        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, string resourceID, ResourceType type);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool EnumResourceNames(IntPtr hModule, ResourceType pType, EnumResNameProc callback, IntPtr param);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool EnumResourceTypes(IntPtr hModule, EnumResTypeProc callback, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccess dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32", SetLastError = true)]
        public static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32", SetLastError = true)]
        public static extern int ResumeThread(IntPtr hThread);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

        [DllImport("kernel32", SetLastError = true)]
        public static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, uint uuchMax);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);        
    }
}
