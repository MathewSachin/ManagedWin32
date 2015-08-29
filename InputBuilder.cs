using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ManagedWin32.Api;

namespace ManagedWin32
{
    #region Enums
    /// <summary>
    /// Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
    /// </summary>
    [Flags]
    enum KeyboardFlag
    {
        /// <summary>
        /// KEYEVENTF_EXTENDEDKEY = 0x0001 (If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).)
        /// </summary>
        ExtendedKey = 0x0001,

        /// <summary>
        /// KEYEVENTF_KEYUP = 0x0002 (If specified, the key is being released. If not specified, the key is being pressed.)
        /// </summary>
        KeyUp = 0x0002,

        /// <summary>
        /// KEYEVENTF_UNICODE = 0x0004 (If specified, wScan identifies the key and wVk is ignored.)
        /// </summary>
        Unicode = 0x0004,

        /// <summary>
        /// KEYEVENTF_SCANCODE = 0x0008 (Windows 2000/XP: If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section.)
        /// </summary>
        ScanCode = 0x0008,
    }

    /// <summary>
    /// Specifies the type of the input event. This member can be one of the following values. 
    /// </summary>
    enum InputType
    {
        /// <summary>
        /// INPUT_MOUSE = 0x00 (The event is a mouse event. Use the mi structure of the union.)
        /// </summary>
        Mouse = 0,

        /// <summary>
        /// INPUT_KEYBOARD = 0x01 (The event is a keyboard event. Use the ki structure of the union.)
        /// </summary>
        Keyboard = 1,

        /// <summary>
        /// INPUT_HARDWARE = 0x02 (Windows 95/98/Me: The event is from input hardware other than a keyboard or mouse. Use the hi structure of the union.)
        /// </summary>
        Hardware = 2,
    }

    /// <summary>
    /// The set of MouseFlags for use in the Flags property of the <see cref="MOUSEINPUT"/> structure. (See: http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx)
    /// </summary>
    [Flags]
    enum MouseFlag
    {
        /// <summary>
        /// Specifies that movement occurred.
        /// </summary>
        Move = 0x0001,

        /// <summary>
        /// Specifies that the left button was pressed.
        /// </summary>
        LeftDown = 0x0002,

        /// <summary>
        /// Specifies that the left button was released.
        /// </summary>
        LeftUp = 0x0004,

        /// <summary>
        /// Specifies that the right button was pressed.
        /// </summary>
        RightDown = 0x0008,

        /// <summary>
        /// Specifies that the right button was released.
        /// </summary>
        RightUp = 0x0010,

        /// <summary>
        /// Specifies that the middle button was pressed.
        /// </summary>
        MiddleDown = 0x0020,

        /// <summary>
        /// Specifies that the middle button was released.
        /// </summary>
        MiddleUp = 0x0040,

        /// <summary>
        /// Windows 2000/XP: Specifies that an X button was pressed.
        /// </summary>
        XDown = 0x0080,

        /// <summary>
        /// Windows 2000/XP: Specifies that an X button was released.
        /// </summary>
        XUp = 0x0100,

        /// <summary>
        /// Windows NT/2000/XP: Specifies that the wheel was moved, if the mouse has a wheel. The amount of movement is specified in mouseData. 
        /// </summary>
        VerticalWheel = 0x0800,

        /// <summary>
        /// Specifies that the wheel was moved horizontally, if the mouse has a wheel. The amount of movement is specified in mouseData. Windows 2000/XP:  Not supported.
        /// </summary>
        HorizontalWheel = 0x1000,

        /// <summary>
        /// Windows 2000/XP: Maps coordinates to the entire desktop. Must be used with MOUSEEVENTF_ABSOLUTE.
        /// </summary>
        VirtualDesk = 0x4000,

        /// <summary>
        /// Specifies that the dx and dy members contain normalized absolute coordinates. If the flag is not set, dxand dy contain relative data (the change in position since the last reported position). This flag can be set, or not set, regardless of what kind of mouse or other pointing device, if any, is connected to the system. For further information about relative mouse motion, see the following Remarks section.
        /// </summary>
        Absolute = 0x8000,
    }

    public enum MouseButton { LeftButton, MiddleButton, RightButton, }
    #endregion

    /// <summary>
    /// A helper class for building a list of <see cref="INPUT"/> messages ready to be sent to the native Windows API.
    /// </summary>
    public class WindowsInputBuilder
    {
        /// <summary>
        /// The public list of <see cref="INPUT"/> messages being built by this instance.
        /// </summary>
        readonly List<INPUT> InputList = new List<INPUT>();

        public const int MouseWheelClickSize = 120;

        public void Clear() { InputList.Clear(); }

        public int Count { get { return InputList.Count; } }

        /// <returns>True on Success</returns>
        public bool Simulate(bool ClearAfterSimulation = true)
        {
            if (Count > 0)
            {
                uint NoOfSucesses = User32.SendInput((uint)Count, InputList.ToArray(), Marshal.SizeOf(typeof(INPUT)));

                if (NoOfSucesses == Count)
                {
                    if (ClearAfterSimulation) Clear();
                    return true;
                }
                else return false;
            }
            else return true;
        }

        #region Key
        /// <summary>
        /// Determines if the <see cref="KeyCode"/> is an ExtendedKey
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <returns>true if the key code is an extended key; otherwise, false.</returns>
        /// <remarks>
        /// The extended keys consist of the ALT and CTRL keys on the right-hand side of the keyboard; the INS, DEL, HOME, END, PAGE UP, PAGE DOWN, and arrow keys in the clusters to the left of the numeric keypad; the NUM LOCK key; the BREAK (CTRL+PAUSE) key; the PRINT SCRN key; and the divide (/) and ENTER keys in the numeric keypad.
        /// 
        /// See http://msdn.microsoft.com/en-us/library/ms646267(v=vs.85).aspx Section "Extended-Key Flag"
        /// </remarks>
        public static bool IsExtendedKey(KeyCode keyCode)
        {
            return keyCode == KeyCode.MENU ||
                keyCode == KeyCode.LMENU ||
                keyCode == KeyCode.RMENU ||
                keyCode == KeyCode.CONTROL ||
                keyCode == KeyCode.RCONTROL ||
                keyCode == KeyCode.INSERT ||
                keyCode == KeyCode.DELETE ||
                keyCode == KeyCode.HOME ||
                keyCode == KeyCode.END ||
                keyCode == KeyCode.PRIOR ||
                keyCode == KeyCode.NEXT ||
                keyCode == KeyCode.RIGHT ||
                keyCode == KeyCode.UP ||
                keyCode == KeyCode.LEFT ||
                keyCode == KeyCode.DOWN ||
                keyCode == KeyCode.NUMLOCK ||
                keyCode == KeyCode.CANCEL ||
                keyCode == KeyCode.SNAPSHOT ||
                keyCode == KeyCode.DIVIDE;
        }

        /// <summary>
        /// Adds a key down to the list of <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="keyCode">The <see cref="KeyCode"/>.</param>        
        public void AddKeyDown(KeyCode keyCode)
        {
            var down =
                new INPUT
                    {
                        Type = (uint)InputType.Keyboard,
                        Data =
                            {
                                Keyboard =
                                    new KEYBDINPUT
                                        {
                                            KeyCode = (ushort)keyCode,
                                            Scan = 0,
                                            Flags = IsExtendedKey(keyCode) ? (uint)KeyboardFlag.ExtendedKey : 0,
                                            Time = 0,
                                            ExtraInfo = IntPtr.Zero
                                        }
                            }
                    };

            InputList.Add(down);
        }

        /// <summary>
        /// Adds a key up to the list of <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="keyCode">The <see cref="KeyCode"/>.</param>        
        public void AddKeyUp(KeyCode keyCode)
        {
            var up =
                new INPUT
                    {
                        Type = (uint)InputType.Keyboard,
                        Data =
                            {
                                Keyboard =
                                    new KEYBDINPUT
                                        {
                                            KeyCode = (ushort)keyCode,
                                            Scan = 0,
                                            Flags = (uint)(IsExtendedKey(keyCode)
                                                                  ? KeyboardFlag.KeyUp | KeyboardFlag.ExtendedKey
                                                                  : KeyboardFlag.KeyUp),
                                            Time = 0,
                                            ExtraInfo = IntPtr.Zero
                                        }
                            }
                    };

            InputList.Add(up);
        }

        /// <summary>
        /// Adds a key press to the list of <see cref="INPUT"/> messages which is equivalent to a key down followed by a key up.
        /// </summary>
        /// <param name="keyCode">The <see cref="KeyCode"/>.</param>        
        public void AddKeyPress(KeyCode keyCode)
        {
            AddKeyDown(keyCode);
            AddKeyUp(keyCode);
        }
        #endregion

        public void KeysPress(IEnumerable<KeyCode> keyCodes) { foreach (var key in keyCodes) AddKeyPress(key); }

        /// <summary>
        /// Simulates a modified keystroke where there are multiple modifiers and multiple keys like CTRL-ALT-K-C where CTRL and ALT are the modifierKeys and K and C are the keys.
        /// The flow is Modifiers KeyDown in order, Keys Press in order, Modifiers KeyUp in reverse order.
        /// </summary>
        /// <param name="modifierKeyCodes">The list of modifier keys</param>
        /// <param name="keyCodes">The list of keys to simulate</param>
        public void ModifiedKeyStroke(IEnumerable<KeyCode> modifierKeyCodes, IEnumerable<KeyCode> keyCodes)
        {
            foreach (var key in modifierKeyCodes) AddKeyDown(key);

            KeysPress(keyCodes);

            foreach (var key in modifierKeyCodes.Reverse()) AddKeyUp(key);
        }

        #region Characters
        /// <summary>
        /// Adds the character to the list of <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="character">The <see cref="System.Char"/> to be added to the list of <see cref="INPUT"/> messages.</param>        
        public void AddCharacter(char character)
        {
            ushort scanCode = character;

            var down = new INPUT
                           {
                               Type = (uint)InputType.Keyboard,
                               Data =
                                   {
                                       Keyboard =
                                           new KEYBDINPUT
                                               {
                                                   KeyCode = 0,
                                                   Scan = scanCode,
                                                   Flags = (uint)KeyboardFlag.Unicode,
                                                   Time = 0,
                                                   ExtraInfo = IntPtr.Zero
                                               }
                                   }
                           };

            var up = new INPUT
                         {
                             Type = (uint)InputType.Keyboard,
                             Data =
                                 {
                                     Keyboard =
                                         new KEYBDINPUT
                                             {
                                                 KeyCode = 0,
                                                 Scan = scanCode,
                                                 Flags = (uint)(KeyboardFlag.KeyUp | KeyboardFlag.Unicode),
                                                 Time = 0,
                                                 ExtraInfo = IntPtr.Zero
                                             }
                                 }
                         };

            // Handle extended keys:
            // If the scan code is preceded by a prefix byte that has the value 0xE0 (224),
            // we need to include the KEYEVENTF_EXTENDEDKEY flag in the Flags property. 
            if ((scanCode & 0xFF00) == 0xE000)
            {
                down.Data.Keyboard.Flags |= (uint)KeyboardFlag.ExtendedKey;
                up.Data.Keyboard.Flags |= (uint)KeyboardFlag.ExtendedKey;
            }

            InputList.Add(down);
            InputList.Add(up);
        }

        /// <summary>
        /// Adds all of the characters in the specified <see cref="IEnumerable{T}"/> of <see cref="char"/>.
        /// </summary>
        /// <param name="characters">The characters to add.</param>        
        public void AddCharacters(IEnumerable<char> characters) { foreach (var character in characters) AddCharacter(character); }

        /// <summary>
        /// Adds the characters in the specified <see cref="string"/>.
        /// </summary>
        /// <param name="characters">The string of <see cref="char"/> to add.</param>        
        public void AddCharacters(params char[] characters) { AddCharacters(characters); }
        #endregion

        #region MouseMovement
        /// <summary>
        /// Moves the mouse relative to its current position.
        /// </summary>     
        public void AddRelativeMouseMovement(int x, int y)
        {
            var movement = new INPUT { Type = (uint)InputType.Mouse };
            movement.Data.Mouse.Flags = (uint)MouseFlag.Move;
            movement.Data.Mouse.X = x;
            movement.Data.Mouse.Y = y;

            InputList.Add(movement);
        }

        /// <summary>
        /// Move the mouse to an absolute position.
        /// </summary>        
        public void AddAbsoluteMouseMovement(int absoluteX, int absoluteY)
        {
            var movement = new INPUT { Type = (uint)InputType.Mouse };
            movement.Data.Mouse.Flags = (uint)(MouseFlag.Move | MouseFlag.Absolute);
            movement.Data.Mouse.X = absoluteX;
            movement.Data.Mouse.Y = absoluteY;

            InputList.Add(movement);
        }

        /// <summary>
        /// Move the mouse to the absolute position on the virtual desktop.
        /// </summary>
        public void AddAbsoluteMouseMovementOnVirtualDesktop(int absoluteX, int absoluteY)
        {
            var movement = new INPUT { Type = (uint)InputType.Mouse };
            movement.Data.Mouse.Flags = (uint)(MouseFlag.Move | MouseFlag.Absolute | MouseFlag.VirtualDesk);
            movement.Data.Mouse.X = absoluteX;
            movement.Data.Mouse.Y = absoluteY;

            InputList.Add(movement);
        }
        #endregion

        #region MouseButton
        /// <summary>
        /// Adds a mouse button down for the specified button.
        /// </summary>
        public void AddMouseButtonDown(MouseButton button)
        {
            var buttonDown = new INPUT { Type = (uint)InputType.Mouse };

            MouseFlag flg;

            switch (button)
            {
                case MouseButton.MiddleButton:
                    flg = MouseFlag.MiddleDown;
                    break;

                case MouseButton.RightButton:
                    flg = MouseFlag.RightDown;
                    break;

                case MouseButton.LeftButton:
                default:
                    flg = MouseFlag.LeftDown;
                    break;
            };

            buttonDown.Data.Mouse.Flags = (uint)flg;

            InputList.Add(buttonDown);
        }

        /// <summary>
        /// Adds a mouse button down for the specified button.
        /// </summary>
        public void AddMouseXButtonDown(int xButtonId)
        {
            var buttonDown = new INPUT { Type = (uint)InputType.Mouse };
            buttonDown.Data.Mouse.Flags = (uint)MouseFlag.XDown;
            buttonDown.Data.Mouse.MouseData = (uint)xButtonId;
            InputList.Add(buttonDown);
        }

        /// <summary>
        /// Adds a mouse button up for the specified button.
        /// </summary>
        public void AddMouseButtonUp(MouseButton button)
        {
            var buttonUp = new INPUT { Type = (uint)InputType.Mouse };

            MouseFlag flg;

            switch (button)
            {
                case MouseButton.MiddleButton:
                    flg = MouseFlag.MiddleUp;
                    break;

                case MouseButton.RightButton:
                    flg = MouseFlag.RightUp;
                    break;

                case MouseButton.LeftButton:
                default:
                    flg = MouseFlag.LeftUp;
                    break;
            }

            buttonUp.Data.Mouse.Flags = (uint)(flg);
            InputList.Add(buttonUp);
        }

        /// <summary>
        /// Adds a mouse button up for the specified button.
        /// </summary>
        public void AddMouseXButtonUp(int xButtonId)
        {
            var buttonUp = new INPUT { Type = (uint)InputType.Mouse };
            buttonUp.Data.Mouse.Flags = (uint)MouseFlag.XUp;
            buttonUp.Data.Mouse.MouseData = (uint)xButtonId;
            InputList.Add(buttonUp);
        }
        #endregion

        #region Click
        /// <summary>
        /// Adds a single click of the specified button.
        /// </summary>
        public void AddMouseButtonClick(MouseButton button)
        {
            AddMouseButtonDown(button);
            AddMouseButtonUp(button);
        }

        /// <summary>
        /// Adds a single click of the specified button.
        /// </summary>
        public void AddMouseXButtonClick(int xButtonId)
        {
            AddMouseXButtonDown(xButtonId);
            AddMouseXButtonUp(xButtonId);
        }

        /// <summary>
        /// Adds a double click of the specified button.
        /// </summary>
        public void AddMouseButtonDoubleClick(MouseButton button)
        {
            AddMouseButtonClick(button);
            AddMouseButtonClick(button);
        }

        /// <summary>
        /// Adds a double click of the specified button.
        /// </summary>
        public void AddMouseXButtonDoubleClick(int xButtonId)
        {
            AddMouseXButtonClick(xButtonId);
            AddMouseXButtonClick(xButtonId);
        }
        #endregion

        #region Scroll
        /// <summary>
        /// Scroll the vertical mouse wheel by the specified amount.
        /// </summary>
        public void AddMouseVerticalWheelScroll(int scrollAmount)
        {
            var scroll = new INPUT { Type = (uint)InputType.Mouse };
            scroll.Data.Mouse.Flags = (uint)MouseFlag.VerticalWheel;
            scroll.Data.Mouse.MouseData = (uint)scrollAmount;

            InputList.Add(scroll);
        }

        /// <summary>
        /// Scroll the horizontal mouse wheel by the specified amount.
        /// </summary>
        public void AddMouseHorizontalWheelScroll(int scrollAmount)
        {
            var scroll = new INPUT { Type = (uint)InputType.Mouse };
            scroll.Data.Mouse.Flags = (uint)MouseFlag.HorizontalWheel;
            scroll.Data.Mouse.MouseData = (uint)scrollAmount;

            InputList.Add(scroll);
        }
        #endregion

        #region Validation
        /// <summary>
        /// Determines whether the specified key is up or down by calling the GetKeyState function. (See: http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        /// </summary>
        /// <param name="keyCode">The <see cref="KeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is down; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The key status returned from this function changes as a thread reads key messages from its message queue. The status does not reflect the interrupt-level state associated with the hardware. Use the GetAsyncKeyState function to retrieve that information. 
        /// An application calls GetKeyState in response to a keyboard-input message. This function retrieves the state of the key when the input message was generated. 
        /// To retrieve state information for all the virtual keys, use the GetKeyboardState function. 
        /// An application can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for Bthe nVirtKey parameter. This gives the status of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. An application can also use the following virtual-key code constants as values for nVirtKey to distinguish between the left and right instances of those keys. 
        /// VK_LSHIFT
        /// VK_RSHIFT
        /// VK_LCONTROL
        /// VK_RCONTROL
        /// VK_LMENU
        /// VK_RMENU
        /// 
        /// These left- and right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        public static bool IsKeyDown(KeyCode keyCode) { return User32.GetKeyState(keyCode) < 0; }

        /// <summary>
        /// Determines whether the specified key is up or downby calling the <see cref="User32.GetKeyState"/> function. (See: http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        /// </summary>
        /// <param name="keyCode">The <see cref="KeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is up; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The key status returned from this function changes as a thread reads key messages from its message queue. The status does not reflect the interrupt-level state associated with the hardware. Use the GetAsyncKeyState function to retrieve that information. 
        /// An application calls GetKeyState in response to a keyboard-input message. This function retrieves the state of the key when the input message was generated. 
        /// To retrieve state information for all the virtual keys, use the GetKeyboardState function. 
        /// An application can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for Bthe nVirtKey parameter. This gives the status of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. An application can also use the following virtual-key code constants as values for nVirtKey to distinguish between the left and right instances of those keys. 
        /// VK_LSHIFT
        /// VK_RSHIFT
        /// VK_LCONTROL
        /// VK_RCONTROL
        /// VK_LMENU
        /// VK_RMENU
        /// 
        /// These left- and right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        public static bool IsKeyUp(KeyCode keyCode) { return !IsKeyDown(keyCode); }

        /// <summary>
        /// Determines whether the physical key is up or down at the time the function is called regardless of whether the application thread has read the keyboard event from the message pump by calling the <see cref="User32.GetAsyncKeyState"/> function. (See: http://msdn.microsoft.com/en-us/library/ms646293(VS.85).aspx)
        /// </summary>
        /// <param name="keyCode">The <see cref="KeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is down; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The GetAsyncKeyState function works with mouse buttons. However, it checks on the state of the physical mouse buttons, not on the logical mouse buttons that the physical buttons are mapped to. For example, the call GetAsyncKeyState(VK_LBUTTON) always returns the state of the left physical mouse button, regardless of whether it is mapped to the left or right logical mouse button. You can determine the system's current mapping of physical mouse buttons to logical mouse buttons by calling 
        /// Copy CodeGetSystemMetrics(SM_SWAPBUTTON) which returns TRUE if the mouse buttons have been swapped.
        /// 
        /// Although the least significant bit of the return value indicates whether the key has been pressed since the last query, due to the pre-emptive multitasking nature of Windows, another application can call GetAsyncKeyState and receive the "recently pressed" bit instead of your application. The behavior of the least significant bit of the return value is retained strictly for compatibility with 16-bit Windows applications (which are non-preemptive) and should not be relied upon.
        /// 
        /// You can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for the vKey parameter. This gives the state of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. 
        /// 
        /// Windows NT/2000/XP: You can use the following virtual-key code constants as values for vKey to distinguish between the left and right instances of those keys. 
        /// 
        /// Code Meaning 
        /// VK_LSHIFT Left-shift key. 
        /// VK_RSHIFT Right-shift key. 
        /// VK_LCONTROL Left-control key. 
        /// VK_RCONTROL Right-control key. 
        /// VK_LMENU Left-menu key. 
        /// VK_RMENU Right-menu key. 
        /// 
        /// These left- and right-distinguishing constants are only available when you call the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        public static bool IsHardwareKeyDown(KeyCode keyCode) { return User32.GetAsyncKeyState(keyCode) < 0; }

        /// <summary>
        /// Determines whether the physical key is up or down at the time the function is called regardless of whether the application thread has read the keyboard event from the message pump by calling the <see cref="User32.GetAsyncKeyState"/> function. (See: http://msdn.microsoft.com/en-us/library/ms646293(VS.85).aspx)
        /// </summary>
        /// <param name="keyCode">The <see cref="KeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is up; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The GetAsyncKeyState function works with mouse buttons. However, it checks on the state of the physical mouse buttons, not on the logical mouse buttons that the physical buttons are mapped to. For example, the call GetAsyncKeyState(VK_LBUTTON) always returns the state of the left physical mouse button, regardless of whether it is mapped to the left or right logical mouse button. You can determine the system's current mapping of physical mouse buttons to logical mouse buttons by calling 
        /// Copy CodeGetSystemMetrics(SM_SWAPBUTTON) which returns TRUE if the mouse buttons have been swapped.
        /// 
        /// Although the least significant bit of the return value indicates whether the key has been pressed since the last query, due to the pre-emptive multitasking nature of Windows, another application can call GetAsyncKeyState and receive the "recently pressed" bit instead of your application. The behavior of the least significant bit of the return value is retained strictly for compatibility with 16-bit Windows applications (which are non-preemptive) and should not be relied upon.
        /// 
        /// You can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for the vKey parameter. This gives the state of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. 
        /// 
        /// Windows NT/2000/XP: You can use the following virtual-key code constants as values for vKey to distinguish between the left and right instances of those keys. 
        /// 
        /// Code Meaning 
        /// VK_LSHIFT Left-shift key. 
        /// VK_RSHIFT Right-shift key. 
        /// VK_LCONTROL Left-control key. 
        /// VK_RCONTROL Right-control key. 
        /// VK_LMENU Left-menu key. 
        /// VK_RMENU Right-menu key. 
        /// 
        /// These left- and right-distinguishing constants are only available when you call the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        public static bool IsHardwareKeyUp(KeyCode keyCode) { return !IsHardwareKeyDown(keyCode); }

        /// <summary>
        /// Determines whether the toggling key is toggled on (in-effect) or not by calling the <see cref="User32.GetKeyState"/> function.  (See: http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        /// </summary>
        /// <param name="keyCode">The <see cref="KeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the toggling key is toggled on (in-effect); otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The key status returned from this function changes as a thread reads key messages from its message queue. The status does not reflect the interrupt-level state associated with the hardware. Use the GetAsyncKeyState function to retrieve that information. 
        /// An application calls GetKeyState in response to a keyboard-input message. This function retrieves the state of the key when the input message was generated. 
        /// To retrieve state information for all the virtual keys, use the GetKeyboardState function. 
        /// An application can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for the nVirtKey parameter. This gives the status of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. An application can also use the following virtual-key code constants as values for nVirtKey to distinguish between the left and right instances of those keys. 
        /// VK_LSHIFT
        /// VK_RSHIFT
        /// VK_LCONTROL
        /// VK_RCONTROL
        /// VK_LMENU
        /// VK_RMENU
        /// 
        /// These left- and right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        public static bool IsTogglingKeyInEffect(KeyCode keyCode) { return (User32.GetKeyState(keyCode) & 0x01) == 0x01; }
        #endregion
    }
}