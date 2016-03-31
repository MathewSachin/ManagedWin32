using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            if (!User32.IsWindow(hWnd)) throw new ArgumentException("The Specified Handle is not a Window.", nameof(hWnd));
            this.Handle = hWnd;
        }

        public WindowHandler(Window Window) : this(new WindowInteropHelper(Window).Handle) { }
        #endregion

        #region Properties
        public IntPtr Handle { get; }

        public string Title
        {
            get { return Handle == DesktopWindow.Handle ? "Desktop" : User32.GetWindowText(Handle); }
            set { User32.SetWindowText(Handle, value); }
        }

        public string Module
        {
            get
            {
                var module = new StringBuilder(256);
                User32.GetWindowModuleFileName(Handle, module, 256);
                return module.ToString();
            }
        }

        public bool IsMaximized => User32.IsZoomed(Handle);

        public bool IsMinimized => User32.IsIconic(Handle);

        public bool IsForegroundWindow => Handle == User32.GetForegroundWindow();

        public bool IsUntitled => string.IsNullOrEmpty(Title);

        /// <summary>
        /// Sets this Window Object's visibility
        /// </summary>
        public bool IsVisible
        {
            get { return User32.IsWindowVisible(Handle); }
            set
            {
                //show the window
                if (value) 
                    User32.ShowWindowAsync(Handle, ShowWindowFlags.Normal);

                //hide the window
                else Hide();
            }
        }

        public WindowHandler Parent => new WindowHandler(User32.GetParent(Handle));

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
                var tid = User32.GetWindowThreadProcessId(Handle, out pid);
                
                foreach (var t in from ProcessThread t in Process.GetProcessById(pid).Threads where t.Id == tid select t)
                    return t;

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
                var res = User32.GetWindow(Handle, GetWindowEnum.Next);
                return res == IntPtr.Zero ? null : new WindowHandler(res);
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
                var res = User32.GetWindow(Handle, GetWindowEnum.Previous);
                return res == IntPtr.Zero ? null : new WindowHandler(res);
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

        public static WindowHandler DesktopWindow => new WindowHandler(User32.GetDesktopWindow());

        public static IEnumerable<WindowHandler> Enumerate()
        {
            var list = new List<WindowHandler>();

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
            set { User32.SetWindowPos(Handle, IntPtr.Zero, value.X, value.Y, 0, 0, SetWindowPositionFlags.NoSize | SetWindowPositionFlags.NoZOrder); }
        }
        
        //Override ToString() 
        public override string ToString()
        {
            //return the title if it has one, if not return the process name
            return Title.Length > 0 ? Title : Process.ProcessName;
        }

        public override bool Equals(object obj) => (obj is WindowHandler) && Handle == ((WindowHandler) obj).Handle;
        
        public Size Size
        {
            get
            {
                var rect = new RECT();
                User32.GetWindowRect(Handle, ref rect);

                return new Size
                {
                    Width = rect.Right - rect.Left,
                    Height = rect.Bottom - rect.Top
                };
            }
            set { User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, value.Width, value.Height, SetWindowPositionFlags.NoMove | SetWindowPositionFlags.NoZOrder | SetWindowPositionFlags.ShowWindow); }
        }
        
        #region Window States
        public void Minimize() => User32.ShowWindowAsync(Handle, ShowWindowFlags.Minimize);

        public void Restore() => User32.ShowWindowAsync(Handle, ShowWindowFlags.Restore);

        public void Maximize() => User32.ShowWindowAsync(Handle, ShowWindowFlags.Maximize);

        public void Hide() => User32.ShowWindowAsync(Handle, ShowWindowFlags.Hide);

        public void Close() => User32.SendMessage(Handle, WindowsMessage.Close, 0, 0);
        #endregion
    }
}