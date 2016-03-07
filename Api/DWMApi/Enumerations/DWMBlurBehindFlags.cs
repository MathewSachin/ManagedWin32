using System;

namespace ManagedWin32.Api
{
    [Flags]
    public enum DWMBlurbehindFlags
    {
        /// <summary>
        /// Flag Transparency Enabled
        /// </summary>
        Enable = 1,

        /// <summary>
        /// Flag Region
        /// </summary>
        BlurRegion = 2,

        /// <summary>
        /// Flag Transition on maximized
        /// </summary>
        TransitionOnMaximized = 4
    }
}