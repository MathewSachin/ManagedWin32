﻿using System;
using System.Runtime.InteropServices;
using System.Text;

// Check info about [Out] StringBuilder

namespace ManagedWin32.Api
{
    // TODO: Make internals public
    public static class Kernel32
    {
        public const int MAX_PATH = 260;

        const string DllName = "kernel32.dll";

        public static bool IsIntResource(IntPtr lpszName) => ((uint)lpszName >> 16) == 0;

        #region CurrentProcess
        [DllImport(DllName)]
        static extern IntPtr GetCurrentProcess();

        public static IntPtr CurrentProcess => GetCurrentProcess();
        #endregion

        [DllImport(DllName)]
        public static extern int GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);
        
        [DllImport(DllName)]
        public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes, bool bInheritHandles, int dwCreationFlags, IntPtr lpEnvironment,
            string lpCurrentDirectory, ref StartUpInfo lpStartupInfo, ref ProcessInfo lpProcessInformation);

        [DllImport(DllName)]
        public static extern int GetThreadId(IntPtr thread);

        [DllImport(DllName)]
        public static extern int GetProcessId(IntPtr process);

        [DllImport(DllName)]
        public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, [Out] StringBuilder lpBuffer, int nSize, IntPtr Arguments);

        [DllImport(DllName)]
        public static extern void SetLastError(uint dwErrCode);

        [DllImport(DllName)]
        public static extern Int32 CloseHandle(IntPtr hObject);

        [DllImport(DllName, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

        // TODO: Check Validity
        [DllImport(DllName)]
        public unsafe static extern void CopyMemory(void* dest, void* src, int length);

        // TODO: Check Validity
        [DllImport(DllName, EntryPoint = "RtlMoveMemory")]
        public unsafe static extern void CopyMemory(RGBQuad* dest, byte* src, int cb);

        [DllImport(DllName)]
        public static extern bool CopyFile(string src, string dst, bool failIfExists);

        [DllImport(DllName)]
        public static extern bool DeleteFile(string path);

        [DllImport(DllName)]
        internal static extern int ExpandEnvironmentStrings(string lpSrc, [Out] StringBuilder lpDst, int nSize);

        [DllImport(DllName)]
        internal static extern int GetComputerName([Out] StringBuilder nameBuffer, ref int bufferSize);

        [DllImport(DllName)]
        public static extern uint GetCurrentProcessId();

        [DllImport(DllName)]
        public static extern IntPtr GetCurrentThread();

        [DllImport(DllName)]
        public static extern bool GetDiskFreeSpaceEx(string drive, out long freeBytesForUser, out long totalBytes, out long freeBytes);

        [DllImport(DllName)]
        public static extern int GetDriveType(string drive);

        [DllImport(DllName)]
        internal static extern int GetEnvironmentVariable(string lpName, [Out] StringBuilder lpValue, int size);

        [DllImport(DllName)]
        public static extern int GetLogicalDrives();

        [DllImport(DllName)]
        internal static extern int GetSystemDirectory([Out] StringBuilder sb, int length);

        [DllImport(DllName)]
        internal static extern uint GetTempFileName(string tmpPath, string prefix, uint uniqueIdOrZero, [Out] StringBuilder tmpFileName);

        [DllImport(DllName)]
        internal static extern uint GetTempPath(int bufferLen, [Out] StringBuilder buffer);

        [DllImport(DllName)]
        public static extern bool IsWow64Process([In] IntPtr hSourceProcessHandle, out bool isWow64);

        [DllImport(DllName)]
        public static extern bool MoveFile(string src, string dst);

        [DllImport(DllName)]
        internal static extern int GetWindowsDirectory([Out] StringBuilder sb, int length);

        [DllImport(DllName)]
        public static extern bool RemoveDirectory(string path);

        [DllImport(DllName, EntryPoint = "RtlZeroMemory")]
        public static extern void ZeroMemory(IntPtr address, UIntPtr length);

        [DllImport(DllName)]
        public static extern bool SetEnvironmentVariable(string lpName, string lpValue);

        [DllImport(DllName)]
        public static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        [DllImport(DllName)]
        public static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        [DllImport(DllName)]
        public static extern bool UpdateResource(IntPtr hUpdate, ResourceType lpType, IntPtr pName, ushort wLanguage, byte[] lpData, uint cbData);

        [DllImport(DllName)]
        public static extern bool UpdateResource(IntPtr hUpdate, ResourceType lpType, uint lpName, ushort wLanguage, byte[] lpData, uint cbData);

        [DllImport(DllName)]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport(DllName)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport(DllName)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport(DllName)]
        public static extern IntPtr LoadLibraryEx(string path, IntPtr hFile, LoadLibraryFlags flags);

        [DllImport(DllName)]
        public static extern IntPtr FindResource(IntPtr hModule, IntPtr resourceID, ResourceType type);

        [DllImport(DllName)]
        public static extern IntPtr FindResource(IntPtr hModule, string resourceID, ResourceType type);

        [DllImport(DllName)]
        public static extern bool EnumResourceNames(IntPtr hModule, ResourceType pType, EnumResNameProc callback, IntPtr param);

        [DllImport(DllName)]
        public static extern bool EnumResourceTypes(IntPtr hModule, EnumResTypeProc callback, IntPtr lParam);

        [DllImport(DllName)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport(DllName)]
        public static extern IntPtr LockResource(IntPtr hResData);

        [DllImport(DllName)]
        public static extern int SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport(DllName)]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);

        [DllImport(DllName)]
        public static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesWritten);

        [DllImport(DllName)]
        public static extern IntPtr OpenProcess(ProcessAccess dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport(DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AttachConsole(uint dwProcessId);

        [DllImport(DllName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        [DllImport(DllName)]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport(DllName)]
        public static extern uint SuspendThread(IntPtr hThread);

        [DllImport(DllName)]
        public static extern int ResumeThread(IntPtr hThread);

        [DllImport(DllName)]
        public static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

        [DllImport(DllName)]
        public static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, uint uuchMax);

        [DllImport(DllName)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);        
    }
}