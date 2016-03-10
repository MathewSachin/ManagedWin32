using ManagedWin32.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ManagedWin32
{
    public class Desktop : IDisposable
    {
        #region Constants
        const int STARTF_USESTDHANDLES = 0x00000100;
        const int STARTF_USESHOWWINDOW = 0x00000001;
        const int UOI_NAME = 2;
        const int STARTF_USEPOSITION = 0x00000004;
        const int NORMAL_PRIORITY_CLASS = 0x00000020;
        #endregion

        bool IsDisposed = false;

        #region Public Properties
        /// <summary>
        /// Gets if a desktop is open.
        /// </summary>
        public bool IsOpen => (DesktopHandle != IntPtr.Zero);

        /// <summary>
        /// Gets the name of a given desktop.
        /// </summary>
        /// <returns>If successful, the desktop name, otherwise, null.</returns>
        public string DesktopName
        {
            get
            {
                // get name.
                if (IsOpen) return null;

                // check its not a null pointer.
                // null pointers wont work.
                if (DesktopHandle == IntPtr.Zero) return null;

                // get the length of the name.
                int needed = 0;
                string name = String.Empty;
                User32.GetUserObjectInformation(DesktopHandle, UOI_NAME, IntPtr.Zero, 0, ref needed);

                // get the name.
                IntPtr ptr = Marshal.AllocHGlobal(needed);
                bool result = User32.GetUserObjectInformation(DesktopHandle, UOI_NAME, ptr, needed, ref needed);
                name = Marshal.PtrToStringAnsi(ptr);
                Marshal.FreeHGlobal(ptr);

                // something went wrong.
                if (!result) return null;

                return name;
            }
        }

        /// <summary>
        /// Gets a handle to the desktop, IntPtr.Zero if no desktop open.
        /// </summary>
        public IntPtr DesktopHandle { get; private set; }
        #endregion

        #region Construction/Destruction
        public Desktop(string name)
        {
            // make sure desktop doesnt already exist.
            if (Exists(name))
            {
                // open the desktop.
                DesktopHandle = User32.OpenDesktop(name, 0, true, DesktopAccessRights.AllRights);

                // something went wrong.
                if (DesktopHandle == IntPtr.Zero) throw new Exception();

                return;
            }

            // attempt to create desktop.
            DesktopHandle = User32.CreateDesktop(name, IntPtr.Zero, IntPtr.Zero, 0, DesktopAccessRights.AllRights, IntPtr.Zero);

            // something went wrong.
            if (DesktopHandle == IntPtr.Zero) throw new Exception();
        }

        // constructor is private to prevent invalid handles being passed to it.
        Desktop(IntPtr desktop) { DesktopHandle = desktop; }

        ~Desktop() { Dispose(); }

        static Desktop Open(string name) => new Desktop(User32.OpenDesktop(name, 0, true, DesktopAccessRights.AllRights));
        #endregion

        #region Methods
        /// <summary>
        /// Closes the handle to a desktop.
        /// </summary>
        /// <returns>True if an open handle was successfully closed.</returns>
        public bool Close()
        {
            // make sure object isnt disposed.
            CheckDisposed();

            // check there is a desktop open.
            if (DesktopHandle != IntPtr.Zero)
            {
                // close the desktop.
                bool result = User32.CloseDesktop(DesktopHandle);

                if (result) DesktopHandle = IntPtr.Zero;

                return result;
            }

            // no desktop was open, so desktop is closed.
            return true;
        }

        /// <summary>
        /// Switches input to the currently opened desktop.
        /// </summary>
        /// <returns>True if desktops were successfully switched.</returns>
        public bool Show()
        {
            // make sure object isnt disposed.
            CheckDisposed();

            // make sure there is a desktop to open.
            if (DesktopHandle == IntPtr.Zero) return false;

            // attempt to switch desktops.
            bool result = User32.SwitchDesktop(DesktopHandle);

            return result;
        }

        /// <summary>
        /// Enumerates the windows on a desktop.
        /// </summary>
        /// <param name="windows">Array of Desktop.Window objects to recieve windows.</param>
        /// <returns>A window colleciton if successful, otherwise null.</returns>
        public IEnumerable<WindowHandler> GetWindows()
        {
            // make sure object isnt disposed.
            CheckDisposed();

            // make sure a desktop is open.
            if (!IsOpen) yield break;

            List<WindowHandler> m_windows = new List<WindowHandler>();

            // get windows.
            if (!User32.EnumDesktopWindows(DesktopHandle, new EnumDesktopWindowsProc((hWnd, lParam) =>
                {
                    // add window handle to colleciton.
                    m_windows.Add(new WindowHandler(hWnd));

                    return true;
                }), IntPtr.Zero)) yield break;

            foreach (WindowHandler window in m_windows) yield return window;
        }

        /// <summary>
        /// Creates a new process in a desktop.
        /// </summary>
        /// <param name="path">Path to application.</param>
        /// <returns>The process object for the newly created process.</returns>
        public Process CreateProcess(string path)
        {
            // make sure object isnt disposed.
            CheckDisposed();

            // make sure a desktop is open.
            if (!IsOpen)
                return null;

            // set startup parameters.
            var si = new StartUpInfo();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = DesktopName;

            var pi = new ProcessInfo();

            // start the process.
            bool result = Kernel32.CreateProcess(null, path, IntPtr.Zero, IntPtr.Zero, true, NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref si, ref pi);

            // error?
            if (!result)
                return null;

            // Get the process.
            return Process.GetProcessById(pi.ProcessId);
        }

        /// <summary>
        /// Prepares a desktop for use.  For use only on newly created desktops, call straight after CreateDesktop.
        /// </summary>
        public void Prepare()
        {
            // make sure object isnt disposed.
            CheckDisposed();

            // make sure a desktop is open.
            if (IsOpen)
            {
                // load explorer.
                CreateProcess("explorer.exe");
            }
        }

        /// <summary>
        /// Gets an array of all the processes running on this desktop.
        /// </summary>
        public IEnumerable<Process> GetProcesses()
        {
            // get all processes.
            Process[] processes = Process.GetProcesses();

            // get the current desktop name.
            string name = DesktopName;

            // cycle through the processes.
            foreach (Process process in processes)
            {
                // check the threads of the process - are they in this one?
                foreach (ProcessThread pt in process.Threads)
                {
                    // check for a desktop name match.
                    if (new Desktop(User32.GetThreadDesktop(pt.Id)).DesktopName == name)
                    {
                        // found a match, add to list, and bail.
                        yield return process;
                        break;
                    }
                }
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Opens the current input desktop.
        /// </summary>
        /// <returns>True if the desktop was succesfully opened.</returns>
        public static Desktop CurrentInputDesktop
        {
            get { return new Desktop(User32.OpenInputDesktop(0, true, DesktopAccessRights.AllRights)); }
        }

        /// <summary>
        /// Enumerates all of the desktops.
        /// </summary>
        /// <param name="desktops">String array to recieve desktop names.</param>
        public static IEnumerable<Desktop> GetDesktops()
        {
            // attempt to enum desktops.
            IntPtr windowStation = User32.GetProcessWindowStation();

            // check we got a valid handle.
            if (windowStation == IntPtr.Zero) yield break;

            List<Desktop> desktops = new List<Desktop>();

            bool result = User32.EnumDesktops(windowStation, new EnumDesktopProc((lpszDesktop, lParam) =>
                {
                    desktops.Add(Open(lpszDesktop));

                    return true;
                }), IntPtr.Zero);

            // something went wrong.
            if (!result) yield break;

            foreach (Desktop desktop in desktops) yield return desktop;
        }

        /// <summary>
        /// Gets the desktop of the calling thread.
        /// </summary>
        /// <returns>Returns a Desktop object for the valling thread.</returns>
        /// <summary>
        /// Sets the desktop of the calling thread.
        /// NOTE: Function will fail if thread has hooks or windows in the current desktop.
        /// </summary>
        /// <param name="desktop">Desktop to put the thread in.</param>
        /// <returns>True if the threads desktop was successfully changed.</returns>
        public static Desktop Current
        {
            get { return new Desktop(User32.GetThreadDesktop(Thread.CurrentThread.ManagedThreadId)); }
            set
            {
                // set threads desktop.
                if (!value.IsOpen) return;

                User32.SetThreadDesktop(value.DesktopHandle);
            }
        }

        /// <summary>
        /// Opens the default desktop.
        /// </summary>
        /// <returns>If successful, a Desktop object, otherwise, null.</returns>
        public static Desktop DefaultDesktop => Open("Default");

        /// <summary>
        /// Checks if the specified desktop exists.
        /// </summary>
        /// <param name="name">The name of the desktop.</param>
        /// <param name="caseInsensitive">If the search is case INsensitive.</param>
        /// <returns>True if the desktop exists, otherwise false.</returns>
        public static bool Exists(string name, bool caseInsensitive = false)
        {
            // return true if desktop exists.
            foreach (Desktop desktop in GetDesktops())
            {
                // case insensitive, compare all in lower case.
                if (caseInsensitive) { if (desktop.ToString().ToLower() == name.ToLower()) return true; }
                else if (desktop.ToString() == name) return true;
            }

            return false;
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// Dispose Object.
        /// </summary>
        public void Dispose()
        {
            // dispose
            Dispose(true);

            // suppress finalisation
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose Object.
        /// </summary>
        /// <param name="disposing">True to dispose managed resources.</param>
        public virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                // dispose of managed resources,
                // close handles
                Close();
            }

            IsDisposed = true;
        }

        void CheckDisposed()
        {
            // check if disposed
            if (IsDisposed)
            {
                // object disposed, throw exception
                throw new ObjectDisposedException("");
            }
        }
        #endregion

        /// <summary>
        /// Gets the desktop name.
        /// </summary>
        public override string ToString() => DesktopName;
    }
}