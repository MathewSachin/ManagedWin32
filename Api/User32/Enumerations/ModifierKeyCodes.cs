using System;

namespace ManagedWin32.Api
{
    /// <summary>
    /// Modifier Key Codes
    /// </summary>
    [Flags]
    public enum ModifierKeyCodes
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }
}