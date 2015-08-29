using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ManagedWin32.Api;

namespace ManagedWin32
{
    public class Library : IDisposable
    {
        public IntPtr Handle { get; private set; }

        public static bool IsLoaded(string libraryName)
        {
            var process = Process.GetCurrentProcess();
            return process.Modules.Cast<ProcessModule>().
                Any(m => string.Compare(m.ModuleName, libraryName, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        #region Factory
        Library(IntPtr Handle)
        {
            if (Handle == IntPtr.Zero)
                switch ((Win32Error)Marshal.GetLastWin32Error())
                {
                    case Win32Error.FileNotFound:
                        throw new FileNotFoundException();
                    case Win32Error.BadFormat:
                        throw new ArgumentException("The file is not a valid win32 executable or dll.");
                    default:
                        throw new Exception("Failed to Load the Dll");
                }

            this.Handle = Handle;
        }

        public Library(string Path) : this(Kernel32.LoadLibrary(Path)) { FileName = Path; }

        public Library(string Path, LoadLibraryFlags Flags)
            : this(Kernel32.LoadLibraryEx(Path, IntPtr.Zero, Flags)) { FileName = Path; }

        public void Dispose()
        {
            Kernel32.FreeLibrary(Handle);
            Handle = IntPtr.Zero;
        }
        #endregion

        public T FindFunction<T>(string Name) where T : DelegateConstraint
        {
            if (!typeof(T).IsSubclassOf(typeof(Delegate))) throw new ArgumentException(typeof(T).Name);

            var ptr = Kernel32.GetProcAddress(Handle, Name);

            if (ptr == IntPtr.Zero) throw new EntryPointNotFoundException(Name + " was not found in " + Path.GetFileName(FileName));

            return (T)(object)Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
        }

        public bool HasMethod(string Name) { return Kernel32.GetProcAddress(Handle, Name) != IntPtr.Zero; }

        #region Resources
        public LibraryResource FindResource(IntPtr ResourceID, ResourceType RType)
        {
            return new LibraryResource(Kernel32.FindResource(Handle, ResourceID, RType), Handle, RType, ResourceID);
        }

        public LibraryResource[] EnumerateResources(ResourceType RType)
        {
            List<LibraryResource> FoundResources = new List<LibraryResource>();

            EnumResNameProc Callback = (h, t, name, l) =>
            {
                FoundResources.Add(FindResource(name, RType));

                return true;
            };

            Kernel32.EnumResourceNames(Handle, RType, Callback, IntPtr.Zero);

            return FoundResources.ToArray();
        }

        public bool HasResource(ResourceType RType) { return EnumerateResources(RType).Length != 0; }
        #endregion

        public string FileName { get; private set; }
    }

    public class LibraryResource
    {
        public IntPtr Handle { get; private set; }

        IntPtr LibraryHandle;

        public IntPtr ResourceId { get; private set; }

        public ResourceType ResourceType { get; private set; }

        public LibraryResource(IntPtr Handle, IntPtr LibraryHandle, ResourceType RType, IntPtr ResourceId)
        {
            this.Handle = Handle;
            this.LibraryHandle = LibraryHandle;
            ResourceType = RType;
            this.ResourceId = ResourceId;

            if (Handle == IntPtr.Zero || LibraryHandle == IntPtr.Zero)
                throw new Exception();
        }

        public int Size { get { return (int)Kernel32.SizeofResource(LibraryHandle, Handle); } }

        public byte[] Data
        {
            get
            {
                IntPtr hRes = Kernel32.LoadResource(LibraryHandle, Handle);
                IntPtr LockedRes = Kernel32.LockResource(hRes);

                byte[] buffer = new byte[Size];
                Marshal.Copy(LockedRes, buffer, 0, Size);

                return buffer;
            }
        }
    }
}