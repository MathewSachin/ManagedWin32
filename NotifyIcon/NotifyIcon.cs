using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Threading;
using ManagedWin32.Api;
using Point = System.Drawing.Point;

namespace ManagedWin32
{
    static class Extensions
    {
        /// <summary>
        /// Checks a list of candidates for equality to a given
        /// reference value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value">The evaluated value.</param>
        /// <param name="Candidates">A liste of possible values that are
        /// regarded valid.</param>
        /// <returns>True if one of the submitted <paramref name="Candidates"/>
        /// matches the evaluated value. If the <paramref name="Candidates"/>
        /// parameter itself is null, too, the method returns false as well,
        /// which allows to check with null values, too.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="Candidates"/>
        /// is a null reference.</exception>
        public static bool Is<T>(this T Value, params T[] Candidates)
        {
            if (Candidates == null) return false;

            foreach (var t in Candidates) if (Value.Equals(t)) return true;

            return false;
        }
    }

    /// <summary>
    /// A WPF proxy to for a taskbar icon (NotifyIcon) that sits in the system's
    /// taskbar notification area ("system tray").
    /// </summary>
    public partial class NotifyIcon : FrameworkElement, IDisposable
    {
        #region Members
        /// <summary>
        /// Represents the current icon data.
        /// </summary>
        NotifyIconData iconData;

        /// <summary>
        /// Receives messages from the taskbar icon.
        /// </summary>
        readonly WindowMessageSink messageSink;

        /// <summary>
        /// An action that is being invoked if the <see cref="singleClickTimer"/> fires.
        /// </summary>
        Action singleClickTimerAction;

        /// <summary>
        /// A timer that is used to differentiate between single and double clicks.
        /// </summary>
        readonly Timer singleClickTimer;

        /// <summary>
        /// A timer that is used to close open balloon tooltips.
        /// </summary>
        readonly Timer balloonCloseTimer;

        /// <summary>
        /// Indicates whether the taskbar icon has been created or not.
        /// </summary>
        public bool IsTaskbarIconCreated { get; private set; }

        /// <summary>
        /// Indicates whether custom tooltips are supported, which depends
        /// on the OS. Windows Vista or higher is required in order to
        /// support this feature.
        /// </summary>
        public bool SupportsCustomToolTips { get { return messageSink.Version == NotifyIconVersion.Vista; } }

        /// <summary>
        /// Checks whether a non-tooltip popup is currently opened.
        /// </summary>
        bool IsPopupOpen
        {
            get
            {
                var popup = TrayPopupResolved;
                var menu = ContextMenu;
                var balloon = CustomBalloon;

                return popup != null && popup.IsOpen || menu != null && menu.IsOpen || balloon != null && balloon.IsOpen;
            }
        }

        double scalingFactor = double.NaN;
        #endregion

        #region Construction
        /// <summary>
        /// Inits the taskbar icon and registers a message listener
        /// in order to receive events from the taskbar area.
        /// </summary>
        public NotifyIcon()
        {
            //using dummy sink in design mode
            messageSink = IsDesignMode ? WindowMessageSink.CreateEmpty() : new WindowMessageSink(NotifyIconVersion.Win95);

            //init icon data structure
            iconData = NotifyIconData.CreateDefault(messageSink.MessageWindowHandle);

            //create the taskbar icon
            CreateTaskbarIcon();

            //register event listeners
            messageSink.MouseEventReceived += OnMouseEvent;
            messageSink.TaskbarCreated += () =>
            {
                IsTaskbarIconCreated = false;
                CreateTaskbarIcon();
            };

            messageSink.ChangeToolTipStateRequest += OnToolTipChange;
            messageSink.BalloonToolTipChanged += (visible) => Raise(visible ? TrayBalloonTipShownEvent : TrayBalloonTipClosedEvent);

            //init single click / balloon timers
            singleClickTimer = new Timer((state) =>
            {
                if (IsDisposed) return;

                //run action
                Action action = singleClickTimerAction;
                if (action != null)
                {
                    //cleanup action
                    singleClickTimerAction = null;

                    //switch to UI thread
                    Dispatcher.Invoke(action);
                }
            });

            balloonCloseTimer = new Timer((state) =>
            {
                if (IsDisposed) return;

                Dispatcher.Invoke(new Action(CloseBalloon));
            });

            //register listener in order to get notified when the application closes
            if (Application.Current != null) Application.Current.Exit += OnExit;
        }
        #endregion

        #region Custom Balloons
        /// <summary>
        /// Shows a custom control as a tooltip in the tray location.
        /// </summary>
        /// <param name="balloon"></param>
        /// <param name="animation">An optional animation for the popup.</param>
        /// <param name="timeout">The time after which the popup is being closed.
        /// Submit null in order to keep the balloon open inde
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="balloon"/>
        /// is a null reference.</exception>
        public void ShowCustomBalloon(UIElement balloon, PopupAnimation animation, int? timeout)
        {
            if (!Dispatcher.CheckAccess())
            {
                var action = new Action(() => ShowCustomBalloon(balloon, animation, timeout));
                Dispatcher.Invoke(DispatcherPriority.Normal, action);
                return;
            }

            if (balloon == null) throw new ArgumentNullException("balloon");
            if (timeout.HasValue && timeout < 500)
            {
                string msg = "Invalid timeout of {0} milliseconds. Timeout must be at least 500 ms";
                msg = String.Format(msg, timeout);
                throw new ArgumentOutOfRangeException("timeout", msg);
            }

            EnsureNotDisposed();

            //make sure we don't have an open balloon
            lock (this) CloseBalloon();

            //create an invisible popup that hosts the UIElement
            Popup popup = new Popup();
            popup.AllowsTransparency = true;

            //provide the popup with the taskbar icon's data context
            UpdateDataContext(popup, null, DataContext);

            //don't animate by default - devs can use attached
            //events or override
            popup.PopupAnimation = animation;

            //in case the balloon is cleaned up through routed events, the
            //control didn't remove the balloon from its parent popup when
            //if was closed the last time - just make sure it doesn't have
            //a parent that is a popup
            var parent = LogicalTreeHelper.GetParent(balloon) as Popup;
            if (parent != null) parent.Child = null;

            if (parent != null)
            {
                string msg = "Cannot display control [{0}] in a new balloon popup - that control already has a parent. You may consider creating new balloons every time you want to show one.";
                msg = String.Format(msg, balloon);
                throw new InvalidOperationException(msg);
            }

            popup.Child = balloon;

            //don't set the PlacementTarget as it causes the popup to become hidden if the
            //TaskbarIcon's parent is hidden, too...
            //popup.PlacementTarget = this;

            popup.Placement = PlacementMode.AbsolutePoint;
            popup.StaysOpen = true;

            Point position = AppBarInfo.GetTrayLocation();
            position = GetDeviceCoordinates(position);
            popup.HorizontalOffset = position.X - 1;
            popup.VerticalOffset = position.Y - 1;

            //store reference
            lock (this) CustomBalloon = popup;

            //assign this instance as an attached property
            SetParentTaskbarIcon(balloon, this);

            //fire attached event
            Raise(BalloonShowingEvent, balloon, this);

            //display item
            popup.IsOpen = true;

            //register timer to close the popup
            if (timeout.HasValue) balloonCloseTimer.Change(timeout.Value, Timeout.Infinite);
        }

        /// <summary>
        /// Resets the closing timeout, which effectively
        /// keeps a displayed balloon message open until
        /// it is either closed programmatically through
        /// <see cref="CloseBalloon"/> or due to a new
        /// message being displayed.
        /// </summary>
        public void ResetBalloonCloseTimer()
        {
            if (IsDisposed) return;

            //reset timer in any case
            lock (this) balloonCloseTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Closes the current <see cref="CustomBalloon"/>, if the
        /// property is set.
        /// </summary>
        public void CloseBalloon()
        {
            if (IsDisposed) return;

            if (!Dispatcher.CheckAccess())
            {
                Action action = CloseBalloon;
                Dispatcher.Invoke(DispatcherPriority.Normal, action);
                return;
            }

            lock (this)
            {
                //reset timer in any case
                balloonCloseTimer.Change(Timeout.Infinite, Timeout.Infinite);

                //reset old popup, if we still have one
                Popup popup = CustomBalloon;
                if (popup != null)
                {
                    UIElement element = popup.Child;

                    //announce closing
                    RoutedEventArgs eventArgs = Raise(BalloonClosingEvent, element, this);
                    if (!eventArgs.Handled)
                    {
                        //if the event was handled, clear the reference to the popup,
                        //but don't close it - the handling code has to manage this stuff now

                        //close the popup
                        popup.IsOpen = false;

                        //remove the reference of the popup to the balloon in case we want to reuse
                        //the balloon (then added to a new popup)
                        popup.Child = null;

                        //reset attached property
                        if (element != null) SetParentTaskbarIcon(element, null);
                    }

                    //remove custom balloon anyway
                    CustomBalloon = null;
                }
            }
        }
        #endregion

        /// <summary>
        /// Checks if a given <see cref="PopupActivationMode"/> is a match for an effectively pressed mouse button.
        /// </summary>
        static bool IsMatch(MouseEvent me, PopupActivationMode activationMode)
        {
            switch (activationMode)
            {
                case PopupActivationMode.LeftClick:
                    return me == MouseEvent.IconLeftMouseUp;
                case PopupActivationMode.RightClick:
                    return me == MouseEvent.IconRightMouseUp;
                case PopupActivationMode.LeftOrRightClick:
                    return me.Is(MouseEvent.IconLeftMouseUp, MouseEvent.IconRightMouseUp);
                case PopupActivationMode.LeftOrDoubleClick:
                    return me.Is(MouseEvent.IconLeftMouseUp, MouseEvent.IconDoubleClick);
                case PopupActivationMode.DoubleClick:
                    return me.Is(MouseEvent.IconDoubleClick);
                case PopupActivationMode.MiddleClick:
                    return me == MouseEvent.IconMiddleMouseUp;
                case PopupActivationMode.All:
                    //return true for everything except mouse movements
                    return me != MouseEvent.MouseMove;
                default:
                    throw new ArgumentOutOfRangeException("activationMode");
            }
        }

        /// <summary>
        /// Processes mouse events, which are bubbled
        /// through the class' routed events, trigger
        /// certain actions (e.g. show a popup), or
        /// both.
        /// </summary>
        /// <param name="me">Event flag.</param>
        void OnMouseEvent(MouseEvent me)
        {
            if (IsDisposed) return;

            switch (me)
            {
                case MouseEvent.MouseMove:
                    Raise(TrayMouseMoveEvent);
                    //immediately return - there's nothing left to evaluate
                    return;
                case MouseEvent.IconRightMouseDown:
                    Raise(TrayRightMouseDownEvent);
                    break;
                case MouseEvent.IconLeftMouseDown:
                    Raise(TrayLeftMouseDownEvent);
                    break;
                case MouseEvent.IconRightMouseUp:
                    Raise(TrayRightMouseUpEvent);
                    break;
                case MouseEvent.IconLeftMouseUp:
                    Raise(TrayLeftMouseUpEvent);
                    break;
                case MouseEvent.IconMiddleMouseDown:
                    Raise(TrayMiddleMouseDownEvent);
                    break;
                case MouseEvent.IconMiddleMouseUp:
                    Raise(TrayMiddleMouseUpEvent);
                    break;
                case MouseEvent.IconDoubleClick:
                    //cancel single click timer
                    singleClickTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    //bubble event
                    Raise(TrayMouseDoubleClickEvent);
                    ExecuteIfEnabled(DoubleClickCommand, DoubleClickCommandParameter, DoubleClickCommandTarget ?? this);
                    break;
                case MouseEvent.BalloonToolTipClicked:
                    Raise(TrayBalloonTipClickedEvent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("me", "Missing handler for mouse event flag: " + me);
            }

            //get mouse coordinates
            //physical cursor position is supported for Vista and above
            Point cursorPosition = (messageSink.Version == NotifyIconVersion.Vista) ? User32.PhysicalCursorPosition : User32.CursorPosition;

            cursorPosition = GetDeviceCoordinates(cursorPosition);

            bool isLeftClickCommandInvoked = false;

            //show popup, if requested
            if (IsMatch(me, PopupActivation))
            {
                if (me == MouseEvent.IconLeftMouseUp)
                {
                    //show popup once we are sure it's not a double click
                    singleClickTimerAction = () =>
                    {
                        ExecuteIfEnabled(LeftClickCommand, LeftClickCommandParameter, LeftClickCommandTarget ?? this);
                        ShowTrayPopup(cursorPosition);
                    };
                    singleClickTimer.Change(User32.DoubleClickTime, Timeout.Infinite);
                    isLeftClickCommandInvoked = true;
                }
                //show popup immediately
                else ShowTrayPopup(cursorPosition);
            }

            //show context menu, if requested
            if (IsMatch(me, MenuActivation))
            {
                if (me == MouseEvent.IconLeftMouseUp)
                {
                    //show context menu once we are sure it's not a double click
                    singleClickTimerAction = () =>
                    {
                        ExecuteIfEnabled(LeftClickCommand, LeftClickCommandParameter, LeftClickCommandTarget ?? this);
                        ShowContextMenu(cursorPosition);
                    };
                    singleClickTimer.Change(User32.DoubleClickTime, Timeout.Infinite);
                    isLeftClickCommandInvoked = true;
                }
                //show context menu immediately
                else ShowContextMenu(cursorPosition);
            }

            //make sure the left click command is invoked on mouse clicks
            if (me == MouseEvent.IconLeftMouseUp && !isLeftClickCommandInvoked)
            {
                //show context menu once we are sure it's not a double click
                singleClickTimerAction = () => ExecuteIfEnabled(LeftClickCommand, LeftClickCommandParameter, LeftClickCommandTarget ?? this);
                singleClickTimer.Change(User32.DoubleClickTime, Timeout.Infinite);
            }
        }

        #region ToolTips
        /// <summary>
        /// Displays a custom tooltip, if available. This method is only
        /// invoked for Windows Vista and above.
        /// </summary>
        /// <param name="visible">Whether to show or hide the tooltip.</param>
        void OnToolTipChange(bool visible)
        {
            //if we don't have a tooltip, there's nothing to do here...
            if (TrayToolTipResolved == null) return;

            if (visible)
            {
                //ignore if we are already displaying something down there
                if (IsPopupOpen) return;

                if (Raise(PreviewTrayToolTipOpenEvent).Handled) return;

                TrayToolTipResolved.IsOpen = true;

                //raise attached event first
                if (TrayToolTip != null) Raise(ToolTipOpenedEvent, TrayToolTip);

                //bubble routed event
                Raise(TrayToolTipOpenEvent);
            }
            else
            {
                if (Raise(PreviewTrayToolTipCloseEvent).Handled) return;

                //raise attached event first
                if (TrayToolTip != null) Raise(ToolTipCloseEvent, TrayToolTip);

                TrayToolTipResolved.IsOpen = false;

                //bubble event
                Raise(TrayToolTipCloseEvent);
            }
        }

        /// <summary>
        /// Creates a <see cref="ToolTip"/> control that either
        /// wraps the currently set <see cref="TrayToolTip"/>
        /// control or the <see cref="ToolTipText"/> string.<br/>
        /// If <see cref="TrayToolTip"/> itself is already
        /// a <see cref="ToolTip"/> instance, it will be used directly.
        /// </summary>
        /// <remarks>We use a <see cref="ToolTip"/> rather than
        /// <see cref="Popup"/> because there was no way to prevent a
        /// popup from causing cyclic open/close commands if it was
        /// placed under the mouse. ToolTip internally uses a Popup of
        /// its own, but takes advance of Popup's internal <see cref="UIElement.IsHitTestVisible"/>
        /// property which prevents this issue.</remarks>
        void CreateCustomToolTip()
        {
            //check if the item itself is a tooltip
            ToolTip tt = TrayToolTip as ToolTip;

            if (tt == null && TrayToolTip != null)
            {
                //create an invisible wrapper tooltip that hosts the UIElement
                tt = new ToolTip();
                tt.Placement = PlacementMode.Mouse;

                //do *not* set the placement target, as it causes the popup to become hidden if the
                //TaskbarIcon's parent is hidden, too. At runtime, the parent can be resolved through
                //the ParentTaskbarIcon attached dependency property:
                //tt.PlacementTarget = this;

                //make sure the tooltip is invisible
                tt.HasDropShadow = false;
                tt.BorderThickness = new Thickness(0);
                tt.Background = System.Windows.Media.Brushes.Transparent;

                //setting the 
                tt.StaysOpen = true;
                tt.Content = TrayToolTip;
            }
            else if (tt == null && !String.IsNullOrEmpty(ToolTipText))
            {
                //create a simple tooltip for the ToolTipText string
                tt = new ToolTip();
                tt.Content = ToolTipText;
            }

            //the tooltip explicitly gets the DataContext of this instance.
            //If there is no DataContext, the TaskbarIcon assigns itself
            if (tt != null) UpdateDataContext(tt, null, DataContext);

            //store a reference to the used tooltip
            TrayToolTipResolved = tt;
        }

        /// <summary>
        /// Sets tooltip settings for the class depending on defined
        /// dependency properties and OS support.
        /// </summary>
        void WriteToolTipSettings()
        {
            const IconDataMembers flags = IconDataMembers.Tip;
            iconData.ToolTipText = ToolTipText;

            if (messageSink.Version == NotifyIconVersion.Vista)
            {
                //we need to set a tooltip text to get tooltip events from the
                //taskbar icon
                if (String.IsNullOrEmpty(iconData.ToolTipText) && TrayToolTipResolved != null)
                {
                    //if we have not tooltip text but a custom tooltip, we
                    //need to set a dummy value (we're displaying the ToolTip control, not the string)
                    iconData.ToolTipText = "ToolTip";
                }
            }

            //update the tooltip text
            WriteIconData(ref iconData, NotifyCommand.Modify, flags);
        }
        #endregion

        #region Custom Popup
        /// <summary>
        /// Creates a <see cref="ToolTip"/> control that either
        /// wraps the currently set <see cref="TrayToolTip"/>
        /// control or the <see cref="ToolTipText"/> string.<br/>
        /// If <see cref="TrayToolTip"/> itself is already
        /// a <see cref="ToolTip"/> instance, it will be used directly.
        /// </summary>
        /// <remarks>We use a <see cref="ToolTip"/> rather than
        /// <see cref="Popup"/> because there was no way to prevent a
        /// popup from causing cyclic open/close commands if it was
        /// placed under the mouse. ToolTip internally uses a Popup of
        /// its own, but takes advance of Popup's internal <see cref="UIElement.IsHitTestVisible"/>
        /// property which prevents this issue.</remarks>
        void CreatePopup()
        {
            //check if the item itself is a popup
            Popup popup = TrayPopup as Popup;

            if (popup == null && TrayPopup != null)
            {
                //create an invisible popup that hosts the UIElement
                popup = new Popup();
                popup.AllowsTransparency = true;

                //don't animate by default - devs can use attached
                //events or override
                popup.PopupAnimation = PopupAnimation.None;

                //the CreateRootPopup method outputs binding errors in the debug window because
                //it tries to bind to "Popup-specific" properties in case they are provided by the child.
                //We don't need that so just assign the control as the child.
                popup.Child = TrayPopup;

                //do *not* set the placement target, as it causes the popup to become hidden if the
                //TaskbarIcon's parent is hidden, too. At runtime, the parent can be resolved through
                //the ParentTaskbarIcon attached dependency property:
                //popup.PlacementTarget = this;

                popup.Placement = PlacementMode.AbsolutePoint;
                popup.StaysOpen = false;
            }

            //the popup explicitly gets the DataContext of this instance.
            //If there is no DataContext, the TaskbarIcon assigns itself
            if (popup != null) UpdateDataContext(popup, null, DataContext);

            //store a reference to the used tooltip
            TrayPopupResolved = popup;
        }

        /// <summary>
        /// Displays the <see cref="TrayPopup"/> control if
        /// it was set.
        /// </summary>
        void ShowTrayPopup(Point cursorPosition)
        {
            if (IsDisposed) return;

            //raise preview event no matter whether popup is currently set
            //or not (enables client to set it on demand)
            if (Raise(PreviewTrayPopupOpenEvent).Handled) return;

            if (TrayPopup != null)
            {
                //use absolute position, but place the popup centered above the icon
                TrayPopupResolved.Placement = PlacementMode.AbsolutePoint;
                TrayPopupResolved.HorizontalOffset = cursorPosition.X;
                TrayPopupResolved.VerticalOffset = cursorPosition.Y;

                //open popup
                TrayPopupResolved.IsOpen = true;

                IntPtr handle = IntPtr.Zero;
                if (TrayPopupResolved.Child != null)
                {
                    //try to get a handle on the popup itself (via its child)
                    HwndSource source = (HwndSource)PresentationSource.FromVisual(TrayPopupResolved.Child);
                    if (source != null) handle = source.Handle;
                }

                //if we don't have a handle for the popup, fall back to the message sink
                if (handle == IntPtr.Zero) handle = messageSink.MessageWindowHandle;

                //activate either popup or message sink to track deactivation.
                //otherwise, the popup does not close if the user clicks somewhere else
                WindowHandler.ForegroundWindow = new WindowHandler(handle);

                //raise attached event - item should never be null unless developers
                //changed the CustomPopup directly...
                if (TrayPopup != null) Raise(PopupOpenedEvent, TrayPopup);

                //bubble routed event
                Raise(TrayPopupOpenEvent);
            }
        }
        #endregion

        #region Context Menu
        /// <summary>
        /// Displays the <see cref="ContextMenu"/> if it was set.
        /// </summary>
        void ShowContextMenu(Point cursorPosition)
        {
            if (IsDisposed) return;

            //raise preview event no matter whether context menu is currently set
            //or not (enables client to set it on demand)
            if (Raise(PreviewTrayContextMenuOpenEvent).Handled) return;

            if (ContextMenu != null)
            {
                //use absolute positioning. We need to set the coordinates, or a delayed opening
                //(e.g. when left-clicked) opens the context menu at the wrong place if the mouse
                //is moved!
                ContextMenu.Placement = PlacementMode.AbsolutePoint;
                ContextMenu.HorizontalOffset = cursorPosition.X;
                ContextMenu.VerticalOffset = cursorPosition.Y;
                ContextMenu.IsOpen = true;

                IntPtr handle = IntPtr.Zero;

                //try to get a handle on the context itself
                HwndSource source = (HwndSource)PresentationSource.FromVisual(ContextMenu);
                if (source != null) handle = source.Handle;

                //if we don't have a handle for the popup, fall back to the message sink
                if (handle == IntPtr.Zero) handle = messageSink.MessageWindowHandle;

                //activate the context menu or the message window to track deactivation - otherwise, the context menu
                //does not close if the user clicks somewhere else. With the message window
                //fallback, the context menu can't receive keyboard events - should not happen though
                WindowHandler.ForegroundWindow = new WindowHandler(handle);

                //bubble event
                Raise(TrayContextMenuOpenEvent);
            }
        }
        #endregion

        #region Balloon Tips
        /// <summary>
        /// Displays a balloon tip with the specified title,
        /// text, and icon in the taskbar for the specified time period.
        /// </summary>
        /// <param name="title">The title to display on the balloon tip.</param>
        /// <param name="message">The text to display on the balloon tip.</param>
        /// <param name="symbol">A symbol that indicates the severity.</param>
        public void ShowBalloonTip(string title, string message, BalloonIcon symbol)
        {
            BalloonFlags flg;

            switch (symbol)
            {
                case BalloonIcon.None:
                    flg = BalloonFlags.None;
                    break;
                case BalloonIcon.Info:
                    flg = BalloonFlags.Info;
                    break;
                case BalloonIcon.Warning:
                    flg = BalloonFlags.Warning;
                    break;
                case BalloonIcon.Error:
                    flg = BalloonFlags.Error;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("symbol");
            }

            lock (this) ShowBalloonTip(title, message, flg, IntPtr.Zero);
        }

        /// <summary>
        /// Displays a balloon tip with the specified title,
        /// text, and a custom icon in the taskbar for the specified time period.
        /// </summary>
        /// <param name="title">The title to display on the balloon tip.</param>
        /// <param name="message">The text to display on the balloon tip.</param>
        /// <param name="customIcon">A custom icon.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="customIcon"/>
        /// is a null reference.</exception>
        public void ShowBalloonTip(string title, string message, Icon customIcon)
        {
            if (customIcon == null) throw new ArgumentNullException("customIcon");

            lock (this) ShowBalloonTip(title, message, BalloonFlags.User, customIcon.Handle);
        }

        /// <summary>
        /// Invokes <see cref="WinApi.Shell_NotifyIcon"/> in order to display
        /// a given balloon ToolTip.
        /// </summary>
        /// <param name="title">The title to display on the balloon tip.</param>
        /// <param name="message">The text to display on the balloon tip.</param>
        /// <param name="flags">Indicates what icon to use.</param>
        /// <param name="balloonIconHandle">A handle to a custom icon, if any, or <see cref="IntPtr.Zero"/>.</param>
        void ShowBalloonTip(string title, string message, BalloonFlags flags, IntPtr balloonIconHandle)
        {
            EnsureNotDisposed();

            iconData.BalloonText = message ?? String.Empty;
            iconData.BalloonTitle = title ?? String.Empty;

            iconData.BalloonFlags = flags;
            iconData.CustomBalloonIconHandle = balloonIconHandle;
            WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Info | IconDataMembers.Icon);
        }

        /// <summary>
        /// Hides a balloon ToolTip, if any is displayed.
        /// </summary>
        public void HideBalloonTip()
        {
            EnsureNotDisposed();

            //reset balloon by just setting the info to an empty string
            iconData.BalloonText = iconData.BalloonTitle = String.Empty;
            WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Info);
        }
        #endregion

        #region Create / Remove Taskbar Icon
        /// <summary>
        /// Creates the taskbar icon. This message is invoked during initialization,
        /// if the taskbar is restarted, and whenever the icon is displayed.
        /// </summary>
        void CreateTaskbarIcon()
        {
            lock (this)
            {
                if (!IsTaskbarIconCreated)
                {
                    const IconDataMembers members = IconDataMembers.Message | IconDataMembers.Icon | IconDataMembers.Tip;

                    //write initial configuration
                    //return if we couldn't create the icon - we can assume this is because explorer is not running (yet!)
                    //-> try a bit later again rather than throwing an exception. Typically, if the windows
                    // shell is being loaded later, this method is being reinvoked from OnTaskbarCreated
                    // (we could also retry after a delay, but that's currently YAGNI)
                    if (!WriteIconData(ref iconData, NotifyCommand.Add, members)) return;

                    //set to most recent version
                    SetVersion();
                    messageSink.Version = (NotifyIconVersion)iconData.VersionOrTimeout;

                    IsTaskbarIconCreated = true;
                }
            }
        }

        /// <summary>
        /// Sets the version flag for the <see cref="iconData"/>.
        /// </summary>
        void SetVersion()
        {
            iconData.VersionOrTimeout = (uint)NotifyIconVersion.Vista;
            bool status = Shell32.Shell_NotifyIcon(NotifyCommand.SetVersion, ref iconData);

            if (!status)
            {
                iconData.VersionOrTimeout = (uint)NotifyIconVersion.Win2000;
                status = WriteIconData(ref iconData, NotifyCommand.SetVersion);
            }

            if (!status)
            {
                iconData.VersionOrTimeout = (uint)NotifyIconVersion.Win95;
                status = WriteIconData(ref iconData, NotifyCommand.SetVersion);
            }

            if (!status) Debug.Fail("Could not set version");
        }

        /// <summary>
        /// Closes the taskbar icon if required.
        /// </summary>
        void RemoveTaskbarIcon()
        {
            lock (this)
            {
                //make sure we didn't schedule a creation

                if (IsTaskbarIconCreated)
                {
                    WriteIconData(ref iconData, NotifyCommand.Delete, IconDataMembers.Message);
                    IsTaskbarIconCreated = false;
                }
            }
        }
        #endregion

        /// <summary>
        /// Recalculates OS coordinates in order to support WPFs coordinate
        /// system if OS scaling (DPIs) is not 100%.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        Point GetDeviceCoordinates(Point point)
        {
            if (double.IsNaN(scalingFactor))
            {
                //calculate scaling factor in order to support non-standard DPIs
                var presentationSource = PresentationSource.FromVisual(this);
                if (presentationSource == null) scalingFactor = 1;
                else
                {
                    var transform = presentationSource.CompositionTarget.TransformToDevice;
                    scalingFactor = 1 / transform.M11;
                }
            }

            //on standard DPI settings, just return the point
            if (scalingFactor == 1.0) return point;

            return new Point() { X = (int)(point.X * scalingFactor), Y = (int)(point.Y * scalingFactor) };
        }

        #region Dispose / Exit
        /// <summary>
        /// Set to true as soon as <c>Dispose</c> has been invoked.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Checks if the object has been disposed and
        /// raises a <see cref="ObjectDisposedException"/> in case
        /// the <see cref="IsDisposed"/> flag is true.
        /// </summary>
        void EnsureNotDisposed() { if (IsDisposed) throw new ObjectDisposedException(Name ?? GetType().FullName); }

        /// <summary>
        /// Disposes the class if the application exits.
        /// </summary>
        void OnExit(object sender, EventArgs e) { Dispose(); }

        /// <summary>
        /// This destructor will run only if the <see cref="Dispose()"/>
        /// method does not get called. This gives this base class the
        /// opportunity to finalize.
        /// <para>
        /// Important: Do not provide destructors in types derived from
        /// this class.
        /// </para>
        /// </summary>
        ~NotifyIcon() { Dispose(false); }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <remarks>This method is not virtual by design. Derived classes
        /// should override <see cref="Dispose(bool)"/>.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closes the tray and releases all resources.
        /// </summary>
        /// <summary>
        /// <c>Dispose(bool disposing)</c> executes in two distinct scenarios.
        /// If disposing equals <c>true</c>, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// </summary>
        /// <param name="disposing">If disposing equals <c>false</c>, the method
        /// has been called by the runtime from inside the finalizer and you
        /// should not reference other objects. Only unmanaged resources can
        /// be disposed.</param>
        /// <remarks>Check the <see cref="IsDisposed"/> property to determine whether
        /// the method has already been called.</remarks>
        void Dispose(bool disposing)
        {
            //don't do anything if the component is already disposed
            if (IsDisposed || !disposing) return;

            lock (this)
            {
                IsDisposed = true;

                //deregister application event listener
                if (Application.Current != null) Application.Current.Exit -= OnExit;

                //stop timers
                singleClickTimer.Dispose();
                balloonCloseTimer.Dispose();

                //dispose message sink
                messageSink.Dispose();

                //remove icon
                RemoveTaskbarIcon();
            }
        }
        #endregion
    }
}