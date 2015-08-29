using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using ManagedWin32.Api;
using System.Runtime.InteropServices;
using Point = System.Drawing.Point;

namespace ManagedWin32
{
    public class AppBarInfo
    {
        const int ABM_GETTASKBARPOS = 0x00000005;

        APPBARDATA m_data;

        public ScreenEdge Edge { get { return (ScreenEdge)m_data.uEdge; } }

        public Rectangle WorkArea
        {
            get
            {
                var rc = new RECT();
                IntPtr rawRect = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
                bool bResult = User32.SystemParametersInfo(SystemInfoParamsAction.GETWORKAREA, 0, rawRect, 0);
                rc = (RECT)Marshal.PtrToStructure(rawRect, rc.GetType());

                if (bResult)
                {
                    Marshal.FreeHGlobal(rawRect);
                    return new Rectangle(rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top);
                }

                return new Rectangle(0, 0, 0, 0);
            }
        }

        public void GetPosition(string strClassName, string strWindowName)
        {
            m_data = new APPBARDATA();
            m_data.cbSize = (UInt32)Marshal.SizeOf(m_data.GetType());

            IntPtr hWnd = User32.FindWindow(strClassName, strWindowName);

            if (hWnd != IntPtr.Zero)
            {
                UInt32 uResult = Shell32.SHAppBarMessage(ABM_GETTASKBARPOS, ref m_data);

                if (uResult != 1) throw new Exception("Failed to communicate with the given AppBar");
            }
            else throw new Exception("Failed to find an AppBar that matched the given criteria");
        }

        public void GetSystemTaskBarPosition() { GetPosition("Shell_TrayWnd", null); }

        /// <summary>
        /// Gets the position of the system tray.
        /// </summary>
        /// <returns>Tray coordinates.</returns>
        public static Point GetTrayLocation()
        {
            var info = new AppBarInfo();
            info.GetSystemTaskBarPosition();

            var rcWorkArea = info.WorkArea;

            int x = 0, y = 0;
            if (info.Edge == ScreenEdge.Left)
            {
                x = rcWorkArea.Left + 2;
                y = rcWorkArea.Bottom;
            }
            else if (info.Edge == ScreenEdge.Bottom)
            {
                x = rcWorkArea.Right;
                y = rcWorkArea.Bottom;
            }
            else if (info.Edge == ScreenEdge.Top)
            {
                x = rcWorkArea.Right;
                y = rcWorkArea.Top;
            }
            else if (info.Edge == ScreenEdge.Right)
            {
                x = rcWorkArea.Right;
                y = rcWorkArea.Bottom;
            }

            return new Point { X = x, Y = y };
        }
    }

    ///<summary>
    /// Supported icons for the tray's balloon messages.
    ///</summary>
    public enum BalloonIcon { None, Info, Warning, Error }

    /// <summary>
    /// Defines flags that define when a popup is being displyed.
    /// </summary>
    public enum PopupActivationMode { LeftClick, RightClick, DoubleClick, LeftOrRightClick, LeftOrDoubleClick, MiddleClick, All }

    /// <summary>
    /// Contains declarations of WPF dependency properties and events.
    /// </summary>
    partial class NotifyIcon
    {
        /// <summary>
        /// Category name that is set on designer properties.
        /// </summary>
        public const string CategoryName = "NotifyIcon";

        static DependencyProperty RegisterDP<T>(string Name, PropertyChangedCallback Callback = null, T Default = default(T))
        {
            return DependencyProperty.Register(Name, typeof(T), typeof(NotifyIcon), Callback == null ? new FrameworkPropertyMetadata(Default)
                : new FrameworkPropertyMetadata(Default, Callback));
        }

        /// <summary>
        /// Checks whether the <see cref="FrameworkElement.DataContextProperty"/>
        ///  is bound or not.
        /// </summary>
        /// <param name="element">The element to be checked.</param>
        /// <returns>True if the data context property is being managed by a
        /// binding expression.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="element"/>
        /// is a null reference.</exception>
        public static bool IsDataContextDataBound(FrameworkElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            return element.GetBindingExpression(FrameworkElement.DataContextProperty) != null;
        }

        public static bool IsDesignMode
        {
            get { return (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue; }
        }

        #region WriteIconData
        public static readonly object SyncRoot = new object();

        /// <summary>
        /// Updates the taskbar icons with data provided by a given
        /// <see cref="NotifyIconData"/> instance.
        /// </summary>
        /// <param name="data">Configuration settings for the NotifyIcon.</param>
        /// <param name="command">Operation on the icon (e.g. delete the icon).</param>
        /// <param name="flags">Defines which members of the <paramref name="data"/>
        /// structure are set.</param>
        /// <returns>True if the data was successfully written.</returns>
        /// <remarks>See Shell_NotifyIcon documentation on MSDN for details.</remarks>
        public static bool WriteIconData(ref NotifyIconData data, NotifyCommand command, IconDataMembers? flags = null)
        {
            //do nothing if in design mode
            if (IsDesignMode) return true;

            if (flags.HasValue) data.ValidMembers = flags.Value;
            lock (SyncRoot) return Shell32.Shell_NotifyIcon(command, ref data);
        }
        #endregion

        /// <summary>
        /// Executes a given command if its <see cref="ICommand.CanExecute"/> method
        /// indicates it can run.
        /// </summary>
        /// <param name="command">The command to be executed, or a null reference.</param>
        /// <param name="commandParameter">An optional parameter that is associated with
        /// the command.</param>
        /// <param name="target">The target element on which to raise the command.</param>
        public static void ExecuteIfEnabled(ICommand command, object commandParameter, IInputElement target)
        {
            if (command == null) return;

            RoutedCommand rc = command as RoutedCommand;
            if (rc != null)
            {
                //routed commands work on a target
                if (rc.CanExecute(commandParameter, target)) rc.Execute(commandParameter, target);
            }
            else if (command.CanExecute(commandParameter)) command.Execute(commandParameter);
        }

        /// <summary>
        /// Registers properties.
        /// </summary>
        static NotifyIcon()
        {
            //register change listener for the Visibility property
            VisibilityProperty.OverrideMetadata(typeof(NotifyIcon),
                new PropertyMetadata(Visibility.Visible, (d, e) => (d as NotifyIcon).OnVisibilityPropertyChanged(e)));

            //register change listener for the DataContext property 
            DataContextProperty.OverrideMetadata(typeof(NotifyIcon),
                new FrameworkPropertyMetadata(new PropertyChangedCallback((d, e) => (d as NotifyIcon).OnDataContextPropertyChanged(e))));

            //register change listener for the ContextMenu property
            ContextMenuProperty.OverrideMetadata(typeof(NotifyIcon),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(
                    (d, e) => (d as NotifyIcon).OnContextMenuPropertyChanged(e))));
        }

        #region Popup Controls

        #region TrayPopupResolved
        /// <summary>
        /// TrayPopupResolved Read-Only Dependency Property
        /// </summary>
        static readonly DependencyPropertyKey TrayPopupResolvedPropertyKey
            = DependencyProperty.RegisterReadOnly("TrayPopupResolved", typeof(Popup), typeof(NotifyIcon),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// A read-only dependency property that returns the <see cref="Popup"/>
        /// that is being displayed in the taskbar area based on a user action.
        /// </summary>
        public static readonly DependencyProperty TrayPopupResolvedProperty = TrayPopupResolvedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the TrayPopupResolved property. Returns
        /// a <see cref="Popup"/> which is either the
        /// <see cref="TrayPopup"/> control itself or a
        /// <see cref="Popup"/> control that contains the
        /// <see cref="TrayPopup"/>.
        /// </summary>
        [Category(CategoryName)]
        public Popup TrayPopupResolved
        {
            get { return (Popup)GetValue(TrayPopupResolvedProperty); }
            protected set { SetValue(TrayPopupResolvedPropertyKey, value); }
        }
        #endregion

        #region TrayToolTipResolved
        /// <summary>
        /// TrayToolTipResolved Read-Only Dependency Property
        /// </summary>
        static readonly DependencyPropertyKey TrayToolTipResolvedPropertyKey
            = DependencyProperty.RegisterReadOnly("TrayToolTipResolved", typeof(ToolTip), typeof(NotifyIcon),
                new FrameworkPropertyMetadata(null));


        /// <summary>
        /// A read-only dependency property that returns the <see cref="ToolTip"/>
        /// that is being displayed.
        /// </summary>
        public static readonly DependencyProperty TrayToolTipResolvedProperty = TrayToolTipResolvedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the TrayToolTipResolved property. Returns 
        /// a <see cref="ToolTip"/> control that was created
        /// in order to display either <see cref="TrayToolTip"/>
        /// or <see cref="ToolTipText"/>.
        /// </summary>
        [Category(CategoryName)]
        [Browsable(true)]
        [Bindable(true)]
        public ToolTip TrayToolTipResolved
        {
            get { return (ToolTip)GetValue(TrayToolTipResolvedProperty); }
            protected set { SetValue(TrayToolTipResolvedPropertyKey, value); }
        }
        #endregion

        #region CustomBalloon
        /// <summary>
        /// CustomBalloon Read-Only Dependency Property
        /// </summary>
        static readonly DependencyPropertyKey CustomBalloonPropertyKey
            = DependencyProperty.RegisterReadOnly("CustomBalloon", typeof(Popup), typeof(NotifyIcon),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Maintains a currently displayed custom balloon.
        /// </summary>
        public static readonly DependencyProperty CustomBalloonProperty = CustomBalloonPropertyKey.DependencyProperty;

        /// <summary>
        /// A custom popup that is being displayed in the tray area in order
        /// to display messages to the user.
        /// </summary>
        public Popup CustomBalloon
        {
            get { return (Popup)GetValue(CustomBalloonProperty); }
            protected set { SetValue(CustomBalloonPropertyKey, value); }
        }
        #endregion

        #endregion

        #region Dependency Properties

        #region Icon property / IconSource dependency property
        Icon icon;

        /// <summary>
        /// Gets or sets the icon to be displayed. This is not a
        /// dependency property - if you want to assign the property
        /// through XAML, please use the <see cref="IconSource"/>
        /// dependency property.
        /// </summary>
        [Browsable(false)]
        public Icon Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                iconData.IconHandle = value == null ? IntPtr.Zero : icon.Handle;

                WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Icon);
            }
        }

        /// <summary>
        /// Resolves an image source and updates the <see cref="Icon" /> property accordingly.
        /// </summary>
        public static readonly DependencyProperty IconSourceProperty = RegisterDP<ImageSource>("IconSource", (d, e) => (d as NotifyIcon).OnIconSourcePropertyChanged(e));

        /// <summary>
        /// A property wrapper for the <see cref="IconSourceProperty"/>
        /// dependency property:<br/>
        /// Resolves an image source and updates the <see cref="Icon" /> property accordingly.
        /// </summary>
        [Category(CategoryName)]
        [Description("Sets the displayed taskbar icon.")]
        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="IconSourceProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="IconSource"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        void OnIconSourcePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            ImageSource newValue = (ImageSource)e.NewValue;

            //resolving the ImageSource at design time is unlikely to work
            if (!IsDesignMode)
            {
                Uri uri = new Uri(newValue.ToString());
                StreamResourceInfo streamInfo = Application.GetResourceStream(uri);

                if (streamInfo == null)
                {
                    string msg = "The supplied image source '{0}' could not be resolved.";
                    msg = String.Format(msg, newValue);
                    throw new ArgumentException(msg);
                }

                Icon = new Icon(streamInfo.Stream);
            }
        }
        #endregion

        #region ToolTipText dependency property
        /// <summary>
        /// A tooltip text that is being displayed if no custom <see cref="ToolTip"/>
        /// was set or if custom tooltips are not supported.
        /// </summary>
        public static readonly DependencyProperty ToolTipTextProperty = RegisterDP("ToolTipText",
            (d, e) => (d as NotifyIcon).OnToolTipTextPropertyChanged(e), string.Empty);

        /// <summary>
        /// A property wrapper for the <see cref="ToolTipTextProperty"/> dependency property:<br/>
        /// A tooltip text that is being displayed if no custom <see cref="ToolTip"/>
        /// was set or if custom tooltips are not supported.
        /// </summary>
        [Category(CategoryName)]
        [Description("Alternative to a fully blown ToolTip, which is only displayed on Vista and above.")]
        public string ToolTipText
        {
            get { return (string)GetValue(ToolTipTextProperty); }
            set { SetValue(ToolTipTextProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="ToolTipTextProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="ToolTipText"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        void OnToolTipTextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //do not touch tooltips if we have a custom tooltip element
            if (TrayToolTip == null)
            {
                ToolTip currentToolTip = TrayToolTipResolved;

                //if we don't have a wrapper tooltip for the tooltip text, create it now
                if (currentToolTip == null) CreateCustomToolTip();
                //if we have a wrapper tooltip that shows the old tooltip text, just update content
                else currentToolTip.Content = e.NewValue;
            }

            WriteToolTipSettings();
        }
        #endregion

        #region TrayToolTip dependency property
        /// <summary>
        /// A custom UI element that is displayed as a tooltip if the user hovers over the taskbar icon.
        /// Works only with Vista and above. Accordingly, you should make sure that
        /// the <see cref="ToolTipText"/> property is set as well.
        /// </summary>
        public static readonly DependencyProperty TrayToolTipProperty = RegisterDP<UIElement>("TrayToolTip", (d, e) => (d as NotifyIcon).OnTrayToolTipPropertyChanged(e));

        /// <summary>
        /// A property wrapper for the <see cref="TrayToolTipProperty"/>
        /// dependency property:<br/>
        /// A custom UI element that is displayed as a tooltip if the user hovers over the taskbar icon.
        /// Works only with Vista and above. Accordingly, you should make sure that
        /// the <see cref="ToolTipText"/> property is set as well.
        /// </summary>
        [Category(CategoryName)]
        [Description("Custom UI element that is displayed as a tooltip. Only on Vista and above")]
        public UIElement TrayToolTip
        {
            get { return (UIElement)GetValue(TrayToolTipProperty); }
            set { SetValue(TrayToolTipProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="TrayToolTipProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="TrayToolTip"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        void OnTrayToolTipPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //recreate tooltip control
            CreateCustomToolTip();

            //remove the taskbar icon reference from the previously used element
            if (e.OldValue != null) SetParentTaskbarIcon((DependencyObject)e.OldValue, null);
            //set this taskbar icon as a reference to the new tooltip element
            if (e.NewValue != null) SetParentTaskbarIcon((DependencyObject)e.NewValue, this);

            //update tooltip settings - needed to make sure a string is set, even
            //if the ToolTipText property is not set. Otherwise, the event that
            //triggers tooltip display is never fired.
            WriteToolTipSettings();
        }
        #endregion

        #region TrayPopup dependency property
        /// <summary>
        /// A control that is displayed as a popup when the taskbar icon is clicked.
        /// </summary>
        public static readonly DependencyProperty TrayPopupProperty = RegisterDP<UIElement>("TrayPopup", (d, e) => (d as NotifyIcon).OnTrayPopupPropertyChanged(e));

        /// <summary>
        /// A property wrapper for the <see cref="TrayPopupProperty"/>
        /// dependency property:<br/>
        /// A control that is displayed as a popup when the taskbar icon is clicked.
        /// </summary>
        [Category(CategoryName)]
        [Description("Displayed as a Popup if the user clicks on the taskbar icon.")]
        public UIElement TrayPopup
        {
            get { return (UIElement)GetValue(TrayPopupProperty); }
            set { SetValue(TrayPopupProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="TrayPopupProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="TrayPopup"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        void OnTrayPopupPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //remove the taskbar icon reference from the previously used element
            if (e.OldValue != null) SetParentTaskbarIcon((DependencyObject)e.OldValue, null);

            //set this taskbar icon as a reference to the new tooltip element
            if (e.NewValue != null) SetParentTaskbarIcon((DependencyObject)e.NewValue, this);

            //create a pop
            CreatePopup();
        }
        #endregion

        #region MenuActivation dependency property
        /// <summary>
        /// Defines what mouse events display the context menu.
        /// Defaults to <see cref="PopupActivationMode.RightClick"/>.
        /// </summary>
        public static readonly DependencyProperty MenuActivationProperty = RegisterDP("MenuActivation", Default: PopupActivationMode.RightClick);

        /// <summary>
        /// A property wrapper for the <see cref="MenuActivationProperty"/>
        /// dependency property:<br/>
        /// Defines what mouse events display the context menu.
        /// Defaults to <see cref="PopupActivationMode.RightClick"/>.
        /// </summary>
        [Category(CategoryName)]
        [Description("Defines what mouse events display the context menu.")]
        public PopupActivationMode MenuActivation
        {
            get { return (PopupActivationMode)GetValue(MenuActivationProperty); }
            set { SetValue(MenuActivationProperty, value); }
        }
        #endregion

        #region PopupActivation dependency property
        /// <summary>
        /// Defines what mouse events trigger the <see cref="TrayPopup" />.
        /// Default is <see cref="PopupActivationMode.LeftClick" />.
        /// </summary>
        public static readonly DependencyProperty PopupActivationProperty = RegisterDP("PopupActivation", Default: PopupActivationMode.LeftClick);

        /// <summary>
        /// A property wrapper for the <see cref="PopupActivationProperty"/>
        /// dependency property:<br/>
        /// Defines what mouse events trigger the <see cref="TrayPopup" />.
        /// Default is <see cref="PopupActivationMode.LeftClick" />.
        /// </summary>
        [Category(CategoryName)]
        [Description("Defines what mouse events display the TaskbarIconPopup.")]
        public PopupActivationMode PopupActivation
        {
            get { return (PopupActivationMode)GetValue(PopupActivationProperty); }
            set { SetValue(PopupActivationProperty, value); }
        }
        #endregion

        #region Visibility dependency property override
        /// <summary>
        /// Handles changes of the <see cref="UIElement.VisibilityProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="Visibility"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        void OnVisibilityPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Visibility newValue = (Visibility)e.NewValue;

            //update
            if (newValue == Visibility.Visible) CreateTaskbarIcon();
            else RemoveTaskbarIcon();
        }
        #endregion

        #region DataContext dependency property override / target update
        /// <summary>
        /// Updates the <see cref="FrameworkElement.DataContextProperty"/> of a given
        /// <see cref="FrameworkElement"/>. This method only updates target elements
        /// that do not already have a data context of their own, and either assigns
        /// the <see cref="FrameworkElement.DataContext"/> of the NotifyIcon, or the
        /// NotifyIcon itself, if no data context was assigned at all.
        /// </summary>
        void UpdateDataContext(FrameworkElement target, object oldDataContextValue, object newDataContextValue)
        {
            //if there is no target or it's data context is determined through a binding
            //of its own, keep it
            if (target == null || IsDataContextDataBound(target)) return;

            //if the target's data context is the NotifyIcon's old DataContext or the NotifyIcon itself,
            //update it
            if (ReferenceEquals(this, target.DataContext) || Equals(oldDataContextValue, target.DataContext))
            {
                //assign own data context, if available. If there is no data
                //context at all, assign NotifyIcon itself.
                target.DataContext = newDataContextValue ?? this;
            }
        }

        /// <summary>
        /// Handles changes of the <see cref="FrameworkElement.DataContextProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="FrameworkElement.DataContext"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        void OnDataContextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            object newValue = e.NewValue, oldValue = e.OldValue;

            //replace custom data context for ToolTips, Popup, and ContextMenu
            UpdateDataContext(TrayPopupResolved, oldValue, newValue);
            UpdateDataContext(TrayToolTipResolved, oldValue, newValue);
            UpdateDataContext(ContextMenu, oldValue, newValue);
        }
        #endregion

        #region ContextMenu dependency property override
        /// <summary>
        /// Releases the old and updates the new <see cref="ContextMenu"/> property
        /// in order to reflect both the NotifyIcon's <see cref="FrameworkElement.DataContext"/>
        /// property and have the <see cref="ParentTaskbarIconProperty"/> assigned.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        void OnContextMenuPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //remove the taskbar icon reference from the previously used element
            if (e.OldValue != null) SetParentTaskbarIcon((DependencyObject)e.OldValue, null);

            //set this taskbar icon as a reference to the new tooltip element
            if (e.NewValue != null) SetParentTaskbarIcon((DependencyObject)e.NewValue, this);

            UpdateDataContext((ContextMenu)e.NewValue, null, DataContext);
        }
        #endregion

        #region DoubleClickCommand dependency property
        /// <summary>
        /// Associates a command that is being executed if the tray icon is being
        /// double clicked.
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandProperty = RegisterDP<ICommand>("DoubleClickCommand");

        /// <summary>
        /// A property wrapper for the <see cref="DoubleClickCommandProperty"/>
        /// dependency property:<br/>
        /// Associates a command that is being executed if the tray icon is being
        /// double clicked.
        /// </summary>
        [Category(CategoryName)]
        [Description("A command that is being executed if the tray icon is being double-clicked.")]
        public ICommand DoubleClickCommand
        {
            get { return (ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }
        #endregion

        #region DoubleClickCommandParameter dependency property
        /// <summary>
        /// Command parameter for the <see cref="DoubleClickCommand"/>.
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandParameterProperty = RegisterDP<object>("DoubleClickCommandParameter");

        /// <summary>
        /// A property wrapper for the <see cref="DoubleClickCommandParameterProperty"/>
        /// dependency property:<br/>
        /// Command parameter for the <see cref="DoubleClickCommand"/>.
        /// </summary>
        [Category(CategoryName)]
        [Description("Parameter to submit to the DoubleClickCommand when the user double clicks on the NotifyIcon.")]
        public object DoubleClickCommandParameter
        {
            get { return GetValue(DoubleClickCommandParameterProperty); }
            set { SetValue(DoubleClickCommandParameterProperty, value); }
        }
        #endregion

        #region DoubleClickCommandTarget dependency property
        /// <summary>
        /// The target of the command that is fired if the notify icon is double clicked.
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandTargetProperty = RegisterDP<IInputElement>("DoubleClickCommandTarget");

        /// <summary>
        /// A property wrapper for the <see cref="DoubleClickCommandTargetProperty"/>
        /// dependency property:<br/>
        /// The target of the command that is fired if the notify icon is double clicked.
        /// </summary>
        [Category(CategoryName)]
        [Description("The target of the command that is fired if the notify icon is double clicked.")]
        public IInputElement DoubleClickCommandTarget
        {
            get { return (IInputElement)GetValue(DoubleClickCommandTargetProperty); }
            set { SetValue(DoubleClickCommandTargetProperty, value); }
        }
        #endregion

        #region LeftClickCommand dependency property
        /// <summary>
        /// Associates a command that is being executed if the tray icon is being
        /// double clicked.
        /// </summary>
        public static readonly DependencyProperty LeftClickCommandProperty = RegisterDP<ICommand>("LeftClickCommand");

        /// <summary>
        /// A property wrapper for the <see cref="LeftClickCommandProperty"/>
        /// dependency property:<br/>
        /// Associates a command that is being executed if the tray icon is being
        /// left-clicked.
        /// </summary>
        [Category(CategoryName)]
        [Description("A command that is being executed if the tray icon is being left-clicked.")]
        public ICommand LeftClickCommand
        {
            get { return (ICommand)GetValue(LeftClickCommandProperty); }
            set { SetValue(LeftClickCommandProperty, value); }
        }
        #endregion

        #region LeftClickCommandParameter dependency property
        /// <summary>
        /// Command parameter for the <see cref="LeftClickCommand"/>.
        /// </summary>
        public static readonly DependencyProperty LeftClickCommandParameterProperty = RegisterDP<object>("LeftClickCommandParameter");

        /// <summary>
        /// A property wrapper for the <see cref="LeftClickCommandParameterProperty"/>
        /// dependency property:<br/>
        /// Command parameter for the <see cref="LeftClickCommand"/>.
        /// </summary>
        [Category(CategoryName)]
        [Description("The target of the command that is fired if the notify icon is clicked with the left mouse button.")]
        public object LeftClickCommandParameter
        {
            get { return GetValue(LeftClickCommandParameterProperty); }
            set { SetValue(LeftClickCommandParameterProperty, value); }
        }
        #endregion

        #region LeftClickCommandTarget dependency property
        /// <summary>
        /// The target of the command that is fired if the notify icon is clicked.
        /// </summary>
        public static readonly DependencyProperty LeftClickCommandTargetProperty = RegisterDP<IInputElement>("LeftClickCommandTarget");

        /// <summary>
        /// A property wrapper for the <see cref="LeftClickCommandTargetProperty"/>
        /// dependency property:<br/>
        /// The target of the command that is fired if the notify icon is clicked.
        /// </summary>
        [Category(CategoryName)]
        [Description("The target of the command that is fired if the notify icon is clicked with the left mouse button.")]
        public IInputElement LeftClickCommandTarget
        {
            get { return (IInputElement)GetValue(LeftClickCommandTargetProperty); }
            set { SetValue(LeftClickCommandTargetProperty, value); }
        }
        #endregion

        #endregion

        #region Events

        RoutedEventArgs Raise(RoutedEvent REvent)
        {
            var args = new RoutedEventArgs(REvent);
            RaiseEvent(args);
            return args;
        }

        #region TrayLeftMouseDown
        /// <summary>
        /// TrayLeftMouseDown Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayLeftMouseDownEvent = EventManager.RegisterRoutedEvent(
            "TrayLeftMouseDown",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Occurs when the user presses the left mouse button.
        /// </summary>
        [Category(CategoryName)]
        public event RoutedEventHandler TrayLeftMouseDown
        {
            add { AddHandler(TrayLeftMouseDownEvent, value); }
            remove { RemoveHandler(TrayLeftMouseDownEvent, value); }
        }
        #endregion

        #region TrayRightMouseDown
        /// <summary>
        /// TrayRightMouseDown Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayRightMouseDownEvent =
            EventManager.RegisterRoutedEvent("TrayRightMouseDown",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Occurs when the presses the right mouse button.
        /// </summary>
        public event RoutedEventHandler TrayRightMouseDown
        {
            add { AddHandler(TrayRightMouseDownEvent, value); }
            remove { RemoveHandler(TrayRightMouseDownEvent, value); }
        }
        #endregion

        #region TrayMiddleMouseDown
        /// <summary>
        /// TrayMiddleMouseDown Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayMiddleMouseDownEvent =
            EventManager.RegisterRoutedEvent("TrayMiddleMouseDown",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Occurs when the user presses the middle mouse button.
        /// </summary>
        public event RoutedEventHandler TrayMiddleMouseDown
        {
            add { AddHandler(TrayMiddleMouseDownEvent, value); }
            remove { RemoveHandler(TrayMiddleMouseDownEvent, value); }
        }
        #endregion

        #region TrayLeftMouseUp
        /// <summary>
        /// TrayLeftMouseUp Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayLeftMouseUpEvent = EventManager.RegisterRoutedEvent("TrayLeftMouseUp",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Occurs when the user releases the left mouse button.
        /// </summary>
        public event RoutedEventHandler TrayLeftMouseUp
        {
            add { AddHandler(TrayLeftMouseUpEvent, value); }
            remove { RemoveHandler(TrayLeftMouseUpEvent, value); }
        }
        #endregion

        #region TrayRightMouseUp
        /// <summary>
        /// TrayRightMouseUp Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayRightMouseUpEvent = EventManager.RegisterRoutedEvent("TrayRightMouseUp",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Occurs when the user releases the right mouse button.
        /// </summary>
        public event RoutedEventHandler TrayRightMouseUp
        {
            add { AddHandler(TrayRightMouseUpEvent, value); }
            remove { RemoveHandler(TrayRightMouseUpEvent, value); }
        }
        #endregion

        #region TrayMiddleMouseUp
        /// <summary>
        /// TrayMiddleMouseUp Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayMiddleMouseUpEvent = EventManager.RegisterRoutedEvent(
            "TrayMiddleMouseUp",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Occurs when the user releases the middle mouse button.
        /// </summary>
        public event RoutedEventHandler TrayMiddleMouseUp
        {
            add { AddHandler(TrayMiddleMouseUpEvent, value); }
            remove { RemoveHandler(TrayMiddleMouseUpEvent, value); }
        }
        #endregion

        #region TrayMouseDoubleClick
        /// <summary>
        /// TrayMouseDoubleClick Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayMouseDoubleClickEvent =
            EventManager.RegisterRoutedEvent("TrayMouseDoubleClick",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Occurs when the user double-clicks the taskbar icon.
        /// </summary>
        public event RoutedEventHandler TrayMouseDoubleClick
        {
            add { AddHandler(TrayMouseDoubleClickEvent, value); }
            remove { RemoveHandler(TrayMouseDoubleClickEvent, value); }
        }
        #endregion

        #region TrayMouseMove
        /// <summary>
        /// TrayMouseMove Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayMouseMoveEvent = EventManager.RegisterRoutedEvent("TrayMouseMove",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Occurs when the user moves the mouse over the taskbar icon.
        /// </summary>
        public event RoutedEventHandler TrayMouseMove
        {
            add { AddHandler(TrayMouseMoveEvent, value); }
            remove { RemoveHandler(TrayMouseMoveEvent, value); }
        }
        #endregion

        #region TrayBalloonTipShown
        /// <summary>
        /// TrayBalloonTipShown Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayBalloonTipShownEvent =
            EventManager.RegisterRoutedEvent("TrayBalloonTipShown",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Occurs when a balloon ToolTip is displayed.
        /// </summary>
        public event RoutedEventHandler TrayBalloonTipShown
        {
            add { AddHandler(TrayBalloonTipShownEvent, value); }
            remove { RemoveHandler(TrayBalloonTipShownEvent, value); }
        }
        #endregion

        #region TrayBalloonTipClosed
        /// <summary>
        /// TrayBalloonTipClosed Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayBalloonTipClosedEvent =
            EventManager.RegisterRoutedEvent("TrayBalloonTipClosed",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Occurs when a balloon ToolTip was closed.
        /// </summary>
        public event RoutedEventHandler TrayBalloonTipClosed
        {
            add { AddHandler(TrayBalloonTipClosedEvent, value); }
            remove { RemoveHandler(TrayBalloonTipClosedEvent, value); }
        }
        #endregion

        #region TrayBalloonTipClicked
        /// <summary>
        /// TrayBalloonTipClicked Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayBalloonTipClickedEvent =
            EventManager.RegisterRoutedEvent("TrayBalloonTipClicked",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Occurs when the user clicks on a balloon ToolTip.
        /// </summary>
        public event RoutedEventHandler TrayBalloonTipClicked
        {
            add { AddHandler(TrayBalloonTipClickedEvent, value); }
            remove { RemoveHandler(TrayBalloonTipClickedEvent, value); }
        }
        #endregion

        #region TrayContextMenuOpen (and PreviewTrayContextMenuOpen)
        /// <summary>
        /// TrayContextMenuOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayContextMenuOpenEvent =
            EventManager.RegisterRoutedEvent("TrayContextMenuOpen",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Bubbled event that occurs when the context menu of the taskbar icon is being displayed.
        /// </summary>
        public event RoutedEventHandler TrayContextMenuOpen
        {
            add { AddHandler(TrayContextMenuOpenEvent, value); }
            remove { RemoveHandler(TrayContextMenuOpenEvent, value); }
        }

        /// <summary>
        /// PreviewTrayContextMenuOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent PreviewTrayContextMenuOpenEvent =
            EventManager.RegisterRoutedEvent("PreviewTrayContextMenuOpen",
                RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Tunneled event that occurs when the context menu of the taskbar icon is being displayed.
        /// </summary>
        public event RoutedEventHandler PreviewTrayContextMenuOpen
        {
            add { AddHandler(PreviewTrayContextMenuOpenEvent, value); }
            remove { RemoveHandler(PreviewTrayContextMenuOpenEvent, value); }
        }
        #endregion

        #region TrayPopupOpen (and PreviewTrayPopupOpen)
        /// <summary>
        /// TrayPopupOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayPopupOpenEvent = EventManager.RegisterRoutedEvent("TrayPopupOpen",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Bubbled event that occurs when the custom popup is being opened.
        /// </summary>
        public event RoutedEventHandler TrayPopupOpen
        {
            add { AddHandler(TrayPopupOpenEvent, value); }
            remove { RemoveHandler(TrayPopupOpenEvent, value); }
        }

        /// <summary>
        /// PreviewTrayPopupOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent PreviewTrayPopupOpenEvent =
            EventManager.RegisterRoutedEvent("PreviewTrayPopupOpen",
                RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Tunneled event that occurs when the custom popup is being opened.
        /// </summary>
        public event RoutedEventHandler PreviewTrayPopupOpen
        {
            add { AddHandler(PreviewTrayPopupOpenEvent, value); }
            remove { RemoveHandler(PreviewTrayPopupOpenEvent, value); }
        }
        #endregion

        #region TrayToolTipOpen (and PreviewTrayToolTipOpen)
        /// <summary>
        /// TrayToolTipOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayToolTipOpenEvent = EventManager.RegisterRoutedEvent("TrayToolTipOpen",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Bubbled event that occurs when the custom ToolTip is being displayed.
        /// </summary>
        public event RoutedEventHandler TrayToolTipOpen
        {
            add { AddHandler(TrayToolTipOpenEvent, value); }
            remove { RemoveHandler(TrayToolTipOpenEvent, value); }
        }

        /// <summary>
        /// PreviewTrayToolTipOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent PreviewTrayToolTipOpenEvent =
            EventManager.RegisterRoutedEvent("PreviewTrayToolTipOpen",
                RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Tunneled event that occurs when the custom ToolTip is being displayed.
        /// </summary>
        public event RoutedEventHandler PreviewTrayToolTipOpen
        {
            add { AddHandler(PreviewTrayToolTipOpenEvent, value); }
            remove { RemoveHandler(PreviewTrayToolTipOpenEvent, value); }
        }
        #endregion

        #region TrayToolTipClose (and PreviewTrayToolTipClose)
        /// <summary>
        /// TrayToolTipClose Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayToolTipCloseEvent = EventManager.RegisterRoutedEvent("TrayToolTipClose",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Bubbled event that occurs when a custom tooltip is being closed.
        /// </summary>
        public event RoutedEventHandler TrayToolTipClose
        {
            add { AddHandler(TrayToolTipCloseEvent, value); }
            remove { RemoveHandler(TrayToolTipCloseEvent, value); }
        }

        /// <summary>
        /// PreviewTrayToolTipClose Routed Event
        /// </summary>
        public static readonly RoutedEvent PreviewTrayToolTipCloseEvent =
            EventManager.RegisterRoutedEvent("PreviewTrayToolTipClose",
                RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Tunneled event that occurs when a custom tooltip is being closed.
        /// </summary>
        public event RoutedEventHandler PreviewTrayToolTipClose
        {
            add { AddHandler(PreviewTrayToolTipCloseEvent, value); }
            remove { RemoveHandler(PreviewTrayToolTipCloseEvent, value); }
        }
        #endregion

        #endregion

        #region Attached Events
        public RoutedEventArgs Raise(RoutedEvent REvent, UIElement Target, NotifyIcon Source = null)
        {
            if (Target == null) return null;

            var args = new RoutedEventArgs(REvent, Source);
            Target.RaiseEvent(args);
            return args;
        }

        #region PopupOpened
        /// <summary>
        /// PopupOpened Attached Routed Event
        /// </summary>
        public static readonly RoutedEvent PopupOpenedEvent = EventManager.RegisterRoutedEvent("PopupOpened",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Adds a handler for the PopupOpened attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddPopupOpenedHandler(IInputElement element, RoutedEventHandler handler) { element.AddHandler(PopupOpenedEvent, handler); }

        /// <summary>
        /// Removes a handler for the PopupOpened attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemovePopupOpenedHandler(IInputElement element, RoutedEventHandler handler) { element.RemoveHandler(PopupOpenedEvent, handler); }
        #endregion

        #region ToolTipOpened
        /// <summary>
        /// ToolTipOpened Attached Routed Event
        /// </summary>
        public static readonly RoutedEvent ToolTipOpenedEvent = EventManager.RegisterRoutedEvent("ToolTipOpened",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Adds a handler for the ToolTipOpened attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddToolTipOpenedHandler(IInputElement element, RoutedEventHandler handler)
        {
            element.AddHandler(ToolTipOpenedEvent, handler);
        }

        /// <summary>
        /// Removes a handler for the ToolTipOpened attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemoveToolTipOpenedHandler(IInputElement element, RoutedEventHandler handler)
        {
            element.RemoveHandler(ToolTipOpenedEvent, handler);
        }
        #endregion

        #region ToolTipClose
        /// <summary>
        /// ToolTipClose Attached Routed Event
        /// </summary>
        public static readonly RoutedEvent ToolTipCloseEvent = EventManager.RegisterRoutedEvent("ToolTipClose",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Adds a handler for the ToolTipClose attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddToolTipCloseHandler(IInputElement element, RoutedEventHandler handler)
        {
            element.AddHandler(ToolTipCloseEvent, handler);
        }

        /// <summary>
        /// Removes a handler for the ToolTipClose attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemoveToolTipCloseHandler(IInputElement element, RoutedEventHandler handler)
        {
            element.RemoveHandler(ToolTipCloseEvent, handler);
        }
        #endregion

        #region BalloonShowing
        /// <summary>
        /// BalloonShowing Attached Routed Event
        /// </summary>
        public static readonly RoutedEvent BalloonShowingEvent = EventManager.RegisterRoutedEvent("BalloonShowing",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Adds a handler for the BalloonShowing attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddBalloonShowingHandler(IInputElement element, RoutedEventHandler handler)
        {
            element.AddHandler(BalloonShowingEvent, handler);
        }

        /// <summary>
        /// Removes a handler for the BalloonShowing attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemoveBalloonShowingHandler(IInputElement element, RoutedEventHandler handler)
        {
            element.RemoveHandler(BalloonShowingEvent, handler);
        }
        #endregion

        #region BalloonClosing
        /// <summary>
        /// BalloonClosing Attached Routed Event
        /// </summary>
        public static readonly RoutedEvent BalloonClosingEvent = EventManager.RegisterRoutedEvent("BalloonClosing",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotifyIcon));

        /// <summary>
        /// Adds a handler for the BalloonClosing attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddBalloonClosingHandler(IInputElement element, RoutedEventHandler handler)
        {
            element.AddHandler(BalloonClosingEvent, handler);
        }

        /// <summary>
        /// Removes a handler for the BalloonClosing attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemoveBalloonClosingHandler(IInputElement element, RoutedEventHandler handler)
        {
            element.RemoveHandler(BalloonClosingEvent, handler);
        }
        #endregion

        #endregion

        #region ParentTaskbarIcon
        /// <summary>
        /// An attached property that is assigned to displayed UI elements (balloos, tooltips, context menus), and
        /// that can be used to bind to this control. The attached property is being derived, so binding is
        /// quite straightforward:
        /// <code>
        /// <TextBlock Text="{Binding RelativeSource={RelativeSource Self}, Path=(tb:NotifyIcon.ParentTaskbarIcon).ToolTipText}" />
        /// </code>
        /// </summary>  
        public static readonly DependencyProperty ParentTaskbarIconProperty =
            DependencyProperty.RegisterAttached("ParentTaskbarIcon", typeof(NotifyIcon), typeof(NotifyIcon),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the ParentTaskbarIcon property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static NotifyIcon GetParentTaskbarIcon(DependencyObject d) { return (NotifyIcon)d.GetValue(ParentTaskbarIconProperty); }

        /// <summary>
        /// Sets the ParentTaskbarIcon property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetParentTaskbarIcon(DependencyObject d, NotifyIcon value) { d.SetValue(ParentTaskbarIconProperty, value); }
        #endregion
    }
}