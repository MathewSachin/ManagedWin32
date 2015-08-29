namespace ManagedWin32.Api
{
    public class SystemParams
    {
        public static BootMode BootMode { get { return (BootMode)User32.GetSystemMetrics(SystemMetrics.CLEANBOOT); } }

        public static bool IsNetworkConnected { get { return (User32.GetSystemMetrics(SystemMetrics.NETWORK) & 1) != 0; } }

        public static int ScreenWidth { get { return User32.GetSystemMetrics(SystemMetrics.CXSCREEN); } }

        public static int ScreenHeight { get { return User32.GetSystemMetrics(SystemMetrics.CYSCREEN); } }
    }
}