namespace ManagedWin32.Api
{
    /// <summary>
    /// The state of the icon - can be set to
    /// hide the icon.
    /// </summary>
    public enum IconState
    {
        /// <summary>
        /// The icon is visible.
        /// </summary>
        Visible = 0x00,

        /// <summary>
        /// Hide the icon.
        /// </summary>
        Hidden = 0x01,

        /// <summary>
        /// The icon is shared.
        /// </summary>
        Shared = 0x02
    }
}