using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ManagedWin32.Api;

namespace ManagedWin32
{
    public class ProcessMemory : IDisposable
    {
        public Process Process { get; private set; }

        IntPtr ProcessHandle = IntPtr.Zero;

        public ProcessMemory(Process Process) { this.Process = Process; }

        public void Open()
        {
            ProcessHandle = Kernel32.OpenProcess(ProcessAccess.Read | ProcessAccess.Write | ProcessAccess.Operation, 1, (uint)Process.Id);
        }

        public void Dispose()
        {
            Kernel32.CloseHandle(ProcessHandle);
            Process = null;
            ProcessHandle = IntPtr.Zero;
        }

        public byte[] Read(IntPtr MemoryAddress, uint bytesToRead, out int bytesRead)
        {
            byte[] buffer = new byte[bytesToRead];

            IntPtr ptrBytesRead;
            Kernel32.ReadProcessMemory(ProcessHandle, MemoryAddress, buffer, bytesToRead, out ptrBytesRead);

            bytesRead = ptrBytesRead.ToInt32();

            return buffer;
        }

        public int Write(IntPtr MemoryAddress, byte[] bytesToWrite)
        {
            IntPtr ptrBytesWritten;
            Kernel32.WriteProcessMemory(ProcessHandle, MemoryAddress, bytesToWrite, (uint)bytesToWrite.Length, out ptrBytesWritten);

            return ptrBytesWritten.ToInt32();
        }
    }
}