namespace ManagedWin32.Api
{
    /// <summary>
    /// Specifies the boot mode in which the system was started.
    /// </summary>
    public enum BootMode
    {
        /// <summary>The computer was started in the standard boot mode. This mode uses the normal drivers and settings for the system.</summary>
        Normal,
        /// <summary>The computer was started in safe mode without network support. This mode uses a limited drivers and settings profile.</summary>
        FailSafe,
        /// <summary>The computer was started in safe mode with network support. This mode uses a limited drivers and settings profile, and loads the services needed to start networking.</summary>
        FailSafeWithNetwork
    }
}