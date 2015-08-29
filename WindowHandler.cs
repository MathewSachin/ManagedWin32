using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using ManagedWin32.Api;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace ManagedWin32
{
    public class WindowHandler
    {
        #region Constructors
        /// <summary>
        /// Constructs a Window Object
        /// </summary>
        /// <param name="hWnd">Handle</param>
        public WindowHandler(IntPtr hWnd)
        {
            if (!User32.IsWindow(hWnd)) throw new ArgumentException("The Specified Handle is not a Window.", "hWnd");
            this.Handle = hWnd;
        }

        public WindowHandler(Window Window) : this(new WindowInteropHelper(Window).Handle) { }
        #endregion

        #region Properties
        public IntPtr Handle { get; private set; }

        public string Title
        {
            get
            {
                if (Handle == DesktopWindow.Handle) return "Desktop";

                StringBuilder title = new StringBuilder(User32.GetWindowTextLength(Handle) + 1);
                User32.GetWindowText(Handle, title, title.Capacity);
                return title.ToString();
            }
            set { User32.SetWindowText(Handle, value); }
        }

        public string Module
        {
            get
            {
                StringBuilder module = new StringBuilder(256);
                User32.GetWindowModuleFileName(Handle, module, 256);
                return module.ToString();
            }
        }

        public bool IsMaximized { get { return User32.IsZoomed(Handle); } }

        public bool IsMinimized { get { return User32.IsIconic(Handle); } }

        public bool IsForegroundWindow { get { return Handle == User32.GetForegroundWindow(); } }

        public bool IsUntitled { get { return String.IsNullOrEmpty(Title); } }

        /// <summary>
        /// Sets this Window Object's visibility
        /// </summary>
        public bool IsVisible
        {
            get { return User32.IsWindowVisible(Handle); }
            set
            {
                //show the window
                if (value == true) User32.ShowWindowAsync(Handle, ShowWindowFlags.Normal);

                //hide the window
                else Hide();
            }
        }

        public WindowHandler Parent { get { return new WindowHandler(User32.GetParent(Handle)); } }

        public bool IsEnabled
        {
            get { return User32.IsWindowEnabled(Handle); }
            set { User32.EnableWindow(Handle, value); }
        }

        public Process Process
        {
            get
            {
                int pid;
                User32.GetWindowThreadProcessId(Handle, out pid);
                return Process.GetProcessById(pid);
            }
        }

        public ProcessThread Thread
        {
            get
            {
                int pid;
                int tid = User32.GetWindowThreadProcessId(Handle, out pid);
                foreach (ProcessThread t in Process.GetProcessById(pid).Threads)
                    if (t.Id == tid) return t;
                throw new Exception("Thread not found");
            }
        }

        /// <summary>
        /// Get the window that is below this window in the Z order,
        /// or null if this is the lowest window.
        /// </summary>
        public WindowHandler WindowBelow
        {
            get
            {
                IntPtr res = User32.GetWindow(Handle, GetWindowEnum.Next);
                if (res == IntPtr.Zero) return null;
                return new WindowHandler(res);
            }
        }

        /// <summary>
        /// Get the window that is above this window in the Z order,
        /// or null, if this is the foreground window.
        /// </summary>
        public WindowHandler WindowAbove
        {
            get
            {
                IntPtr res = User32.GetWindow(Handle, GetWindowEnum.Previous);
                if (res == IntPtr.Zero) return null;
                return new WindowHandler(res);
            }
        }
        #endregion

        public static WindowHandler ForegroundWindow
        {
            get { return new WindowHandler(User32.GetForegroundWindow()); }
            set
            {
                if (value.IsForegroundWindow) return;

                IntPtr ThreadID1 = User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), IntPtr.Zero),
                    ThreadID2 = User32.GetWindowThreadProcessId(value.Handle, IntPtr.Zero);

                if (ThreadID1 != ThreadID2)
                {
                    User32.AttachThreadInput(ThreadID1, ThreadID2, 1);
                    User32.SetForegroundWindow(value.Handle);
                    User32.AttachThreadInput(ThreadID1, ThreadID2, 0);
                }
                else User32.SetForegroundWindow(value.Handle);

                User32.ShowWindowAsync(value.Handle, value.IsMinimized ? ShowWindowFlags.Restore : ShowWindowFlags.Normal);
            }
        }

        public static WindowHandler DesktopWindow { get { return new WindowHandler(User32.GetDesktopWindow()); } }

        public static IEnumerable<WindowHandler> Enumerate()
        {
            List<WindowHandler> list = new List<WindowHandler>();

            User32.EnumWindows((hWnd, lParam) =>
                {
                    list.Add(new WindowHandler(hWnd));

                    return true;
                }, IntPtr.Zero);

            return list;
        }

        public bool PlaceBelow(WindowHandler wnd)
        {
            return User32.SetWindowPos(Handle, wnd.Handle, 0, 0, 0, 0, SetWindowPositionFlags.NoMove | SetWindowPositionFlags.NoSize);
        }

        public Point Location
        {
            set
            {
                User32.SetWindowPos(Handle, IntPtr.Zero, value.X, value.Y, 0, 0, SetWindowPositionFlags.NoSize | SetWindowPositionFlags.NoZOrder);
            }
        }
        
        //Override ToString() 
        public override string ToString()
        {
            //return the title if it has one, if not return the process name
            return Title.Length > 0 ? Title : Process.ProcessName;
        }

        public override bool Equals(object obj)
        {
            return (obj is WindowHandler) ? Handle == (obj as WindowHandler).Handle : false;
        }
        
        public Size Size
        {
            get
            {
                var rect = new RECT();
                User32.GetWindowRect(Handle, ref rect);

                return new Size()
                {
                    Width = rect.Right - rect.Left,
                    Height = rect.Bottom - rect.Top
                };
            }
            set { User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, value.Width, value.Height, SetWindowPositionFlags.NoMove | SetWindowPositionFlags.NoZOrder | SetWindowPositionFlags.ShowWindow); }
        }
        
        #region Window States
        public void Minimize() { User32.ShowWindowAsync(Handle, ShowWindowFlags.Minimize); }

        public void Restore() { User32.ShowWindowAsync(Handle, ShowWindowFlags.Restore); }

        public void Maximize() { User32.ShowWindowAsync(Handle, ShowWindowFlags.Maximize); }

        public void Hide() { User32.ShowWindowAsync(Handle, ShowWindowFlags.Hide); }

        public void Close() { User32.SendMessage(Handle, WindowsMessage.Close, 0, 0); }
        #endregion
    }
}