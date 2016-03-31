using System;

namespace ManagedWin32.Api
{
    [Flags]
    public enum PowerDataAccessor
    {
        /// <summary>
        /// Check for overrides on AC power settings.
        /// </summary>
        ACCESS_AC_POWER_SETTING_INDEX = 0x0,

        /// <summary>
        /// Check for overrides on DC power settings.
        /// </summary>
        ACCESS_DC_POWER_SETTING_INDEX = 0x1,
        
        /// <summary>
        /// Check for restrictions on specific power schemes.
        /// </summary>
        ACCESS_SCHEME = 0x10,
        
        /// <summary>
        /// Check for restrictions on active power schemes.
        /// </summary>
        ACCESS_ACTIVE_SCHEME = 0x13,
        
        /// <summary>
        /// Check for restrictions on creating or restoring power schemes.
        /// </summary>
        ACCESS_CREATE_SCHEME = 0x14
    }
}