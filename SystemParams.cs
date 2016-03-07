namespace ManagedWin32.Api
{
    public class SystemParams
    {
        public static BootMode BootMode => (BootMode)User32.GetSystemMetrics(SystemMetrics.CLEANBOOT);

        public static bool IsNetworkConnected => (User32.GetSystemMetrics(SystemMetrics.NETWORK) & 1) != 0;

        public static int ScreenWidth => User32.GetSystemMetrics(SystemMetrics.CXSCREEN);

        public static int ScreenHeight => User32.GetSystemMetrics(SystemMetrics.CYSCREEN);
    }
}