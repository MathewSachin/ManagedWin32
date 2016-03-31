using System;
using System.Windows;
using System.Windows.Interop;
using ManagedWin32.Api;

namespace ManagedWin32
{
    public class KeyboardHook : IDisposable
    {
        #region Fields
        WindowInteropHelper host;
        bool IsDisposed;
        int Identifier;
        Random Randomizer = new Random(DateTime.Now.Millisecond);

        public Window Window { get; private set; }

        public KeyCode Key { get; }

        public ModifierKeyCodes Modifiers { get; }
        #endregion
        
        public KeyboardHook(Window Window, KeyCode Key, ModifierKeyCodes Modifiers)
        {
            this.Key = Key;
            this.Modifiers = Modifiers;

            this.Window = Window;
            host = new WindowInteropHelper(Window);

            Identifier = Randomizer.Next();

            User32.RegisterHotKey(host.Handle, Identifier, Modifiers, Key);

            ComponentDispatcher.ThreadPreprocessMessage += ProcessMessage;
        }

        void ProcessMessage(ref MSG msg, ref bool handled)
        {
            if ((msg.message == (int)WindowsMessage.HOTKEY) 
                && (msg.wParam.ToInt32() == Identifier))
                Triggered?.Invoke();
        }

        public event Action Triggered;

        public void Dispose()
        {
            if (!IsDisposed)
            {
                ComponentDispatcher.ThreadPreprocessMessage -= ProcessMessage;

                User32.UnregisterHotKey(host.Handle, Identifier);
                Window = null;
                host = null;
            }
            IsDisposed = true;
        }
    }
}